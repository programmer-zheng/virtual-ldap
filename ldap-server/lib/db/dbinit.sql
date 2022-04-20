CREATE DATABASE IF NOT EXISTS vldap default charset utf8 COLLATE utf8_general_ci;
GRANT ALL PRIVILEGES ON vldap.* to "vldap"@"localhost" identified by "vldapPass";
GRANT ALL PRIVILEGES ON vldap.* to "vldap"@"%" identified by "vldapPass";
flush privileges;
USE `vldap`;

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for user_credentials
-- ----------------------------
DROP TABLE IF EXISTS `user_credentials`;
CREATE TABLE `user_credentials` (
  `uid` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `otpsecret` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

SET FOREIGN_KEY_CHECKS = 1;
