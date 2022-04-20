module.exports = {
  ldap: {
    // every one minute request server api to refresh departments and users
    cronTime: '*/1 * * * *',
    // LDAP serve port, it is a insecure port, please connect with ldap://
    listenPort: 389,
    // Base DN will be o=rongguang,dc=njrgrj,dc=com
    // Groups base DN will be ou=Groups,o=rongguang,dc=njrgrj,dc=com
    // Users base DN will be ou=People,o=rongguang,dc=njrgrj,dc=com
    rootDN: 'dc=njrgrj,dc=com',
    organization: 'rongguang',
    // Admins who can search or modify directory
    admins: [
      {
        // Bind DN will be cn=renfang,dc=njrgrj,dc=com
        commonName: 'renfang',
        password: 'Renfang@123#',
        canModifyEntry: false,
      },
      {
        // Bind DN will be cn=coding,dc=njrgrj,dc=com
        commonName: 'coding',
        password: 'codingnet',
        canModifyEntry: false,
      },
      {
        // Bind DN will be cn=coding,dc=njrgrj,dc=com
        commonName: 'njrgrj-common',
        password: 'R&g@Njrgrj#893',
        canModifyEntry: false,
      }
    ]
  },
  // Provider for providen account service
  // 钉钉配置
  provider: {
    serverHost: 'http://ldap-server.njrgrj.com',
    LdapRequestToken: 'FV*X@2rl&y@k2BQ7',
    name: 'dingtalk',
  },
  // Custom groups, base DN will be ou=CustomGroups,ou=Groups,o=Example,dc=example,dc=com
  //自定义群组/部门
  customGroups: [
    // {
    //   // DN will be ou=Jenkins Admins,ou=CustomGroups,ou=Groups,o=Example,dc=example,dc=com
    //   // name: 'Jenkins Admins',
    //   name: 'Jenkins Admins',
    //   // User with these mails will be added to the group
    //   // members: [ 'jenkins@example.com' ],
    //   members: [ 'jenkins'],
    // }
  ]
}
