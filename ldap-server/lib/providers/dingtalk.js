const axios = require('axios');
const log = require('log').get('dingtalk-provider');
const pinyin = require('pinyin');

const {
  makeGroupEntry,
  makePersonEntry,
  makeOrganizationUnitEntry,
  addMemberToGroup,
} = require('../utilities/ldap');
const {
  saveCacheToFile,
  loadCacheFromFile,
} = require('../utilities/cache');

let serverHost = '';
let ldapRequestToken = '';
let appKey = '';
let appSecret = '';
let accessToken = '';

let allLDAPUsers = [];
let allLDAPOrgUnits = [];
let allLDAPGroups = [];
let allLDAPEntries = [];

function parseName(name) {
  // 如果名字没有空格，以中文处理，第一个字为姓，其他为名
  // 如果有空格，以英文处理，最后一个单词为姓，其他为名
  let givenName = name.substr(1);
  let sn = name.substr(0, 1);
  if (name.indexOf(' ') > 0) {
    const parts = name.split(' ');
    sn = parts.pop();
    givenName = parts.join(' ');
  }

  return { givenName, sn };
}
async function GetServerDept() {
  const apiUrl = serverHost + "/departments";
  const ret = await axios.get(apiUrl, { headers: { 'token': ldapRequestToken } }).catch(e => {
    console.error("server api(/departments) could not be request,please check server api");
    return null;
  });
  if (ret && ret.data) {
    return ret.data;
  }
  return null;
}
async function GetServerUsers(department) {
  const apiUrl = serverHost + "/deptusers?deptid=" + department.id;
  const ret = await axios.get(apiUrl, { headers: { 'token': ldapRequestToken } }).catch(e => {
    console.error(department.id + " get server users error", e);
    return null;
  });
  if (ret && ret.data) {

    ret.data.forEach(u => {
      u.firstDepartment = department;
    });
    return ret.data;
  }
  return null;
}

/*
{
  '100560627': {
    ext: '{"faceCount":"92"}',
    createDeptGroup: true,
    name: 'Product & Dev / 产品技术',
    id: 100560627,
    autoAddUser: true,
    parentid: 111865024,
    dn: 'ou=Product & Dev / 产品技术, ou=全员, o=LongBridge, dc=longbridge-inc, dc=com'
  },
  */
async function fetchAllDepartments() {
  let allDeps = loadCacheFromFile('dingtalk_groups.json');
  if (!allDeps) {
    const deps = await GetServerDept();
    if (!deps) {
      return [];
    }
    console.info('Got', deps.length, 'departments');

    const depsMap = {
      '1': {
        name: 'Staff',
        id: 1,
        parentid: null,
      },
    };
    deps.forEach(d => {
      d.name = d.name.replace(/ \/ /g, ' - ').replace(/\//g, '&').trim();
      depsMap[d.id] = d;
    });

    allDeps = Object.values(depsMap);
    const allDepNames = {};
    allDeps.forEach(v => {
      let name = v.name;
      let idx = 2;
      while (allDepNames[name]) {
        name = v.name + idx;
        idx++;
      }
      allDepNames[name] = 1;
      v.name = name;
    })

    //saveCacheToFile('dingtalk_groups.json', allDeps);
  }

  const depsMap = {};
  allDeps.forEach(d => { depsMap[d.id] = d; });
  allDeps.forEach(d => {
    let obj = d;
    let dn = [`ou=${obj.name}`];
    while (obj.parentid) {
      obj = depsMap[obj.parentid];
      dn.push(`ou=${obj.name}`);
    }
    d.dn = dn.join(',');
  });

  return allDeps;
}


async function fetchAllUsers(departments) {
  let allUsers = loadCacheFromFile('dingtalk_users.json');
  if (!allUsers && departments.length > 0) {
    allUsers = [];
    for (let i = 0; i < departments.length; ++i) {
      // 取消钉钉接口查询人员信息，采用服务器返回接口中的信息
      const deptUsers = await GetServerUsers(departments[i]);

      if (deptUsers) {
        allUsers.push(...deptUsers);
      }
    }

    console.log('department users load complete,load：' + allUsers.length + ' user data');

    //saveCacheToFile('dingtalk_users.json', allUsers);
  } else {
    allUsers = [];
  }

  return allUsers;
}

async function setupProvider(config) {
  //设置服务器地址
  serverHost = config.serverHost;
  ldapRequestToken = config.LdapRequestToken;
  // appKey = config.appKey;
  // appSecret = config.appSecret;
  await reloadFromDingtalkServer();
}

async function reloadFromDingtalkServer() {
  console.log("init department and user data");
  // await getToken();

  // 获取所有部门
  let allDepartments = await fetchAllDepartments();

  // 映射到 organizationalUnit
  const allDepartmentsMap = {};
  allLDAPOrgUnits = allDepartments.map(d => {
    allDepartmentsMap[d.id] = d;
    return makeOrganizationUnitEntry(d.dn, d.name, {
      groupid: d.id,
    });
  });

  // 映射到 groupOfNames
  const allLDAPGroupsMap = [];
  allLDAPGroups = allDepartments.map(d => {
    const g = makeGroupEntry(d.dn, d.name, [], {
      groupid: d.id,
    });
    allLDAPGroupsMap[d.id] = g;
    return g;
  });

  Object.values(allDepartmentsMap).forEach(dep => {
    if (dep.parentid) {
      const parentDep = allDepartmentsMap[dep.parentid];
      addMemberToGroup(allLDAPGroupsMap[dep.id], allLDAPGroupsMap[parentDep.id]);
    }
  })

  // 按部门获取所有员工
  const allUsers = await fetchAllUsers(allDepartments);

  const allUsersMap = {};
  allLDAPUsers = allUsers.filter(u => {

    if (!allUsersMap[u.userid]) {
      allUsersMap[u.userid] = 1;
      return u.active;
    }
    return false;
  }).filter(u => {
    if (!(u.userName)) {
      console.warn('Incorrect user missing username', u);
      return false;
    }
    return true;
  }).map(u => {
    // const mail = (u.orgEmail || u.email).toLowerCase();

    // const dn = `mail=${mail},${u.firstDepartment.dn}`;


    // const { givenName, sn } = parseName(u.name);//拆分人员姓名，返回姓氏 人名（英文采用最后一个单词作为姓）
    // const py = u.remark || (pinyin(u.name, { style: pinyin.STYLE_NORMAL }).join(''));
    const py = u.userName;

    // const dn = `mobile=${u.mobile},${u.firstDepartment.dn}`;
    const dn = `username=${py},${u.firstDepartment.dn}`;

    // 映射到 iNetOrgPerson
    const personEntry = makePersonEntry(dn, {
      title: u.position,
      mobileTelephoneNumber: u.mobile,
      uid: py,
      sAMAccountName: py,
      userPassword: u.password,
      cn: u.name,
      // givenName,
      // sn,
      // mail,
      // mobile: u.mobile,
      avatarurl: u.avatar,
      remark: u.remark
    });
    // console.debug('Sync person entry:', personEntry.dn)

    // 将用户加到组里
    u.department.forEach(depId => {
      let parentDep = allDepartmentsMap[depId];
      while (parentDep && parentDep.id) {
        addMemberToGroup(personEntry, allLDAPGroupsMap[parentDep.id]);
        parentDep = allDepartmentsMap[parentDep.parentid];
      }
    })

    return personEntry;
  });

  const tempallLDAPEntries = [].concat(allLDAPGroups, allLDAPOrgUnits, allLDAPUsers);
  if (tempallLDAPEntries.length > 0) {
    allLDAPEntries = tempallLDAPEntries;
  }
}

function getAllLDAPEntries() {
  return allLDAPEntries;
}

function reloadEntriesFromProvider() {
  console.info('Reload entries from Dingtalk');
  reloadFromDingtalkServer();
}


module.exports = {
  setupProvider,
  getAllLDAPEntries,
  reloadEntriesFromProvider,
};
