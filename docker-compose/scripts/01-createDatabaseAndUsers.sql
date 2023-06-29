CREATE DATABASE IF NOT EXISTS VirtualLdap default charset utf8 COLLATE utf8_general_ci;
CREATE USER 'vldap'@'localhost' IDENTIFIED BY 'vldappassword';
CREATE USER 'vldap'@'%' IDENTIFIED BY 'vldappassword';
GRANT ALL PRIVILEGES ON VirtualLdap. * TO 'vldap'@'localhost';
GRANT ALL PRIVILEGES ON VirtualLdap. * TO 'vldap'@'%';


flush privileges;