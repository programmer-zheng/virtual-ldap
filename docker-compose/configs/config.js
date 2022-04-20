module.exports = {
  ldap: {
    // every one minute request server api to refresh departments and users
    cronTime: '*/1 * * * *',
    // LDAP serve port, it is a insecure port, please connect with ldap://
    listenPort: 389,
    // Base DN will be o=demo,dc=example,dc=com
    // Groups base DN will be ou=Groups,o=demo,dc=example,dc=com
    // Users base DN will be ou=People,o=demo,dc=example,dc=com
    rootDN: 'dc=example,dc=com',
    organization: 'demo',
    // Admins who can search or modify directory
    admins: [
      {
        // Bind DN will be cn=jenkins,dc=example,dc=com
        commonName: 'jenkins',
        password: 'jenkinsAdminPassword',
        canModifyEntry: false,
      },
      {
        // Bind DN will be cn=example-common,dc=example,dc=com
        commonName: 'example-common',
        password: 'example#123',
        canModifyEntry: false,
      }
    ]
  },
  // Provider for providen account service
  // 钉钉配置
  provider: {
    serverHost: 'http://ldap_backend',
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
