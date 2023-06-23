const axios = require('axios');
const ldap = require('ldapjs');
const CronJob = require('cron').CronJob;
const md5 = require('md5');
const {
  ldap: ldapConfig,
  provider: providerConfig,
} = require('./config');
const { parseDN, parseFilter } = ldap;
const {
  getBaseEntries,
  getRootDN,
  getOrganizationBaseDN,
  validateAdminPassword,
} = require('./utilities/ldap');
const {
  createProvider,
  getProviderLDAPEntries,
  reloadEntriesFromProvider,
} = require('./utilities/provider');
const {
  getLDAPCustomGroupEntries,
} = require('./utilities/custom_groups');


let server = ldap.createServer();

// equalsDN for comparing dn in case-insensitive
function equalsDN(a, b) {
  if (a == null || b == null) {
    return false;
  }
  a = parseDN((a instanceof String ? a : a.toString()).toLowerCase());
  b = parseDN((b instanceof String ? b : b.toString()).toLowerCase());
  return a.equals(b);
}

function getPersonMatchedDN(reqDN) {
  const personFilter = parseFilter('(objectclass=inetOrgPerson)');
  const [matchedUser] = getProviderLDAPEntries().filter(entry => {
    return equalsDN(reqDN, entry.dn) && personFilter.matches(entry.attributes);
  });
  return matchedUser;
}

function authorize(req, res, next) {
  // console.debug('===> authorize info');
  // console.debug('req type', req.constructor.name);
  // console.debug('reqDN', (req.baseObject || '').toString());
  // console.debug('bindDN', req.connection.ldap.bindDN.toString());
  // console.debug('====');
  const bindDN = req.connection.ldap.bindDN;
  const rootDN = parseDN(getRootDN());

  // person can search itself
  if (req instanceof ldap.SearchRequest) {
    if (equalsDN(bindDN, req.baseObject)) {
      return next();
    }
  }

  // root admin can do everything
  if (equalsDN(bindDN.parent(), rootDN)) {
    return next();
  }

  return next(new ldap.InsufficientAccessRightsError());
}


// DSE
server.search('', authorize, function (req, res, next) {
  var baseObject = {
    dn: '',
    structuralObjectClass: 'OpenLDAProotDSE',
    configContext: 'cn=config',
    attributes: {
      objectclass: ['top', 'OpenLDAProotDSE'],
      namingContexts: [getRootDN()],
      supportedLDAPVersion: ['3'],
      subschemaSubentry: ['cn=Subschema']
    }
  };
  console.info("scope " + req.scope + " filter " + req.filter + " baseObject " + req.baseObject);
  if ('base' == req.scope
    && '(objectclass=*)' == req.filter.toString()
    && req.baseObject == '') {
    res.send(baseObject);
  }

  //console.info('scope: ' + req.scope);
  //console.info('filter: ' + req.filter.toString());
  //console.info('attributes: ' + req.attributes);
  res.end();
  return next();
});


server.search('cn=Subschema', authorize, function (req, res, next) {
  var schema = {
    dn: 'cn=Subschema',
    attributes: {
      objectclass: ['top', 'subentry', 'subschema', 'extensibleObject'],
      cn: ['Subschema']
    }
  };
  res.send(schema);
  res.end();
  return next();
});


server.search(getRootDN(), authorize, (req, res, next) => {
  console.info("search req scope:" + req.scope + ",filter:" + req.filter + ", baseObject:" + req.baseObject);
  // console.log('sizeLimit', req.sizeLimit, 'timeLimit', req.timeLimit);
  const reqDN = parseDN(req.baseObject.toString().toLowerCase());
  console.debug("search req dn:", reqDN.toString().toLowerCase());
  const comparators = {
    'one': (objDN) => {
      return equalsDN(reqDN, objDN.parent());
    },
    'sub': (objDN) => {
      return equalsDN(reqDN, objDN) || reqDN.parentOf(objDN);
    },
    'base': (objDN) => {
      return equalsDN(reqDN, objDN);
    },
  }

  const comparator = comparators[req.scope];
  if (comparator) {
    [
      ...getBaseEntries(),
      ...getProviderLDAPEntries(),
      ...getLDAPCustomGroupEntries(),
    ].filter(entry => {
      return comparator(parseDN(entry.dn.toLowerCase())) && req.filter.matches(entry.attributes);
    }).forEach(entry => {
      // console.debug('send entry', entry.dn);
      res.send(entry);
    });
  }

  res.end();
  return next();
});

server.modify(getOrganizationBaseDN(), authorize, async (req, res, next) => {

  return next(new ldap.InsufficientAccessRightsError());

});

server.bind(getRootDN(), async (req, res, next) => {
  const reqDN = parseDN(req.dn.toString().toLowerCase());
  console.debug('bind req', reqDN.toString())
  let aaa = getRootDN().toLowerCase();
  console.debug('getRootDN', aaa)
  console.debug('parseDN', parseDN(aaa).toString().toLowerCase())
  console.debug('reqDN.parent', reqDN.parent().toString().toLowerCase())

  if (parseDN(getRootDN().toLowerCase()).equals(reqDN.parent())) {
    // admins
    const username = reqDN.rdns[0].attrs.cn.value;
    // 验证绑定的管理员密码
    if (validateAdminPassword(username, req.credentials)) {
      res.end();
      return next();
    } else {
      return next(new ldap.InvalidCredentialsError());
    }

  } else if (parseDN(getOrganizationBaseDN().toLowerCase()).parentOf(reqDN)) {
    // users
    const matchedUser = getPersonMatchedDN(reqDN);
    if (matchedUser) {
      const apiUrl = providerConfig.serverHost + "/validateuser";
      const postData = { username: matchedUser.attributes.uid, password: req.credentials };
      await axios.post(apiUrl, postData, { headers: { 'Content-Type': 'application/json', 'token': providerConfig.LdapRequestToken } })
        .then(ret => {
          if (ret.data && ret.data.success === true) {
            res.end();
            return next();
          } else {
            return next(new ldap.InvalidCredentialsError(ret.data.msg));
          }
        })
        .catch(e => {
          console.error("request validate user api to check password error", e);
          //当请求api失败时，使用本地密码进行匹配（本地密码不一定是最新的）
          const pwd = md5(req.credentials).toLowerCase();
          console.debug('nodejs md5:' + pwd)
          console.debug('ldapserver md5:' + matchedUser.attributes.userPassword)
          if (pwd == matchedUser.attributes.userPassword.toLowerCase()) {
            res.end();
            return next();
          }
        });
      return next(new ldap.InvalidCredentialsError());

    } else {
      console.warn('user is not found:', reqDN);
      return next(new ldap.InvalidCredentialsError());
    }
  }

  return next(new ldap.InsufficientAccessRightsError());
});

function runVirtualLDAPServer() {
  server.listen(ldapConfig.listenPort, async function () {
    console.info('virtual-ldap starting...')
    await createProvider();
    console.info('virtual-ldap listening at ' + server.url);
  });

  // reload data from server every hour
  new CronJob(ldapConfig.cronTime, () => {
    reloadEntriesFromProvider();
  }, null, true, 'Asia/Shanghai').start();
}

module.exports = {
  runVirtualLDAPServer,
};
