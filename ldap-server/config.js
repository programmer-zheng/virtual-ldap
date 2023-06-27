module.exports = {
  ldap: {
    // 每分钟请求一次后端接口，以刷新部门及人员信息，可修改cron表达式改为需要的时间
    cronTime: '*/1 * * * *',
    // LDAP serve port, it is a insecure port, please connect with ldap://
    listenPort: 389,
    // Base DN will be： o=Example,dc=ldap,dc=com
    // Groups base DN will be： ou=Groups,o=Example,dc=ldap,dc=com
    // Users base DN will be： ou=People,o=Example,dc=ldap,dc=com
    rootDN: 'dc=ldap,dc=com',
    organization: 'Example',
    // 管理员，用于连接ldap服务
    admins: [
      {
        // Bind DN will be：cn=ldap,dc=ldap,dc=com
        commonName: 'ldap',
        password: 'ldap',
        canModifyEntry: false,
      },
      {
        // Bind DN will be：cn=jenkins,dc=ldap,dc=com
        commonName: 'jenkins',
        password: 'jenkins',
        canModifyEntry: false,
      },
    ]
  },
  // 请修改以下后端地址及ldap请求token，确保与后端服务一致
  provider: {
    serverHost: 'http://localhost:7103',
    LdapRequestToken: 'FV*X@2rl&y@k2BQ7',
    name: 'dingtalk'
  },
  // Custom groups, base DN will be ou=CustomGroups,ou=Groups,o=Example,dc=ldap,dc=com
  customGroups: [
    {
      // DN will be ou=Jenkins Admins,ou=CustomGroups,ou=Groups,o=Example,dc=ldap,dc=com
      name: 'Jenkins Admins',
      // User with these mails will be added to the group
      members: ['jenkins@example.com'],
    }
  ]
}
