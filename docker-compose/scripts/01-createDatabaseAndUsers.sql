CREATE DATABASE IF NOT EXISTS dingdingsync default charset utf8 COLLATE utf8_general_ci;
CREATE USER 'vldap'@'localhost' IDENTIFIED BY 'vldappassword';
CREATE USER 'vldap'@'%' IDENTIFIED BY 'vldappassword';
GRANT ALL PRIVILEGES ON dingdingsync. * TO 'vldap'@'localhost';
GRANT ALL PRIVILEGES ON dingdingsync. * TO 'vldap'@'%';


flush privileges;