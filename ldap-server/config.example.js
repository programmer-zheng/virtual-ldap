module.exports = {
  ldap: {
    // every one minute request server api to refresh departments and users
    cronTime: '*/1 * * * *',
    // LDAP serve port, it is a insecure port, please connect with ldap://
    listenPort: 389,
    // Base DN will be o=Example,dc=example,dc=com
    // Groups base DN will be ou=Groups,o=Example,dc=example,dc=com
    // Users base DN will be ou=People,o=Example,dc=example,dc=com
    rootDN: 'dc=example,dc=com',
    organization: 'Example',
    // Admins who can search or modify directory
    admins: [
      {
        // Bind DN will be cn=keycloak,dc=example,dc=com
        commonName: 'keycloak',
        password: 'keycloak',
        canModifyEntry: false,
      },
      {
        commonName: 'jenkins',
        password: 'jenkins',
        canModifyEntry: false,
      },
    ]
  },
  // Provider for providen account service
  provider: {
    serverHost: 'http://www.ldap.com:7103',
    LdapRequestToken: 'FV*X@2rl&y@k2BQ7',
    name: 'dingtalk'
  },
  // Custom groups, base DN will be ou=CustomGroups,ou=Groups,o=Example,dc=example,dc=com
  customGroups: [
    {
      // DN will be ou=Jenkins Admins,ou=CustomGroups,ou=Groups,o=Example,dc=example,dc=com
      name: 'Jenkins Admins',
      // User with these mails will be added to the group
      members: ['jenkins@example.com'],
    }
  ]
}
