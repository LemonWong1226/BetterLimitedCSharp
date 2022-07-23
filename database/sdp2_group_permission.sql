-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: sdp2
-- ------------------------------------------------------
-- Server version	8.0.28

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `group_permission`
--

DROP TABLE IF EXISTS `group_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `group_permission` (
  `department` int NOT NULL,
  `position` int NOT NULL,
  `group_name` varchar(50) NOT NULL,
  `permission` json DEFAULT NULL,
  PRIMARY KEY (`department`,`position`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `group_permission`
--

LOCK TABLES `group_permission` WRITE;
/*!40000 ALTER TABLE `group_permission` DISABLE KEYS */;
INSERT INTO `group_permission` VALUES (1,1,'Sales Representative','[1, 2, 69, 5, 7, 9, 10, 13, 14, 15, 64, 66, 67, 68, 51, 53, 54, 55, 56, 57, 58, 59, 60, 70, 31, 32, 33, 34, 35, 36, 37, 38, 39]'),(1,2,'Sales Manager','[1, 2, 69, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 26, 29, 70, 31, 32, 33, 34, 35, 36, 37, 38, 39, 51, 53, 54, 55, 56, 57, 58, 59, 60, 64, 66, 67, 68]'),(1,99,'Sales Admin','[1, 15]'),(2,1,'Inventory Worker','[1, 2, 3, 11, 12, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 64, 66, 67, 68, 70, 30, 32, 33, 34, 36, 37, 38, 39, 42, 44, 45, 46, 47, 48, 49, 50]'),(2,2,'Inventory Manager','[1, 2, 3, 11, 12, 15, 70, 30, 32, 33, 34, 36, 37, 38, 39, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 42, 44, 45, 46, 47, 48, 49, 50, 64, 66, 67, 68]'),(2,99,'Inventory Admin','[1, 15]'),(3,1,'Technical Worker','[1, 2, 15, 51, 54, 57, 59, 60, 64, 66, 67, 68]'),(3,2,'Technical Manager','[1, 2, 5, 6, 70, 33, 36, 37, 38, 39, 26, 29, 51, 52, 54, 55, 56, 57, 59, 60, 64, 66, 67, 68]'),(3,99,'Technical Admin','[1, 15]'),(4,1,'Accounting Clerk','[1, 2, 17, 20, 26, 29, 40, 41, 42, 48, 50, 61, 62, 63, 64, 66, 67, 68]'),(4,2,'Accounting Manager','[1, 2, 17, 20, 26, 29, 40, 41, 42, 48, 50, 61, 62, 63, 64, 66, 67, 68]'),(4,99,'Accounting Admin','[1, 15]'),(5,1,'Purchase Clerk','[1, 2, 17, 18, 19, 20, 26, 29, 40, 42, 43, 45, 47, 48, 49, 50, 64, 66, 67, 68]'),(5,2,'Purchase Manager','[1, 2, 15, 40, 42, 43, 45, 47, 48, 49, 50, 64, 66, 67, 68]'),(5,99,'Purchase Admin','[1, 15]'),(99,99,'Admin','[1, 2, 16, 69, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 64, 65, 66, 67, 68, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 61, 62, 63, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 70, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50]');
/*!40000 ALTER TABLE `group_permission` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-29  3:39:42
