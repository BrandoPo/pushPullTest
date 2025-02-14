CREATE DATABASE  IF NOT EXISTS `cocktails`  ;
USE `cocktails`;

DROP TABLE IF EXISTS `drinks`;

CREATE TABLE `drinks` (
  `drinks` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

LOCK TABLES `drinks` WRITE;
INSERT INTO `drinks` VALUES ('Manhatten'),('Martini'),('Rum and coke');
UNLOCK TABLES;

