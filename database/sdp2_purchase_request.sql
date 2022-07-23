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
-- Table structure for table `purchase_request`
--

DROP TABLE IF EXISTS `purchase_request`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `purchase_request` (
  `purchase_request_id` int unsigned NOT NULL AUTO_INCREMENT,
  `purchase_ttl_qty` int DEFAULT NULL,
  `purchase_remarks` tinytext,
  `purchase_request_status` int DEFAULT '1',
  `purchase_ttl_amount` double DEFAULT NULL,
  `purchase_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`purchase_request_id`)
) ENGINE=InnoDB AUTO_INCREMENT=700000015 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_request`
--

LOCK TABLES `purchase_request` WRITE;
/*!40000 ALTER TABLE `purchase_request` DISABLE KEYS */;
INSERT INTO `purchase_request` VALUES (700000001,1000,'remark test',4,23500,'2022-06-11 23:29:37'),(700000002,500,'remark test',2,11750,'2022-06-11 23:29:37'),(700000003,500,'remark test',3,11750,'2022-06-11 23:29:37'),(700000004,500,'remark test',1,349500,'2022-06-11 23:29:37'),(700000005,200,'remark test',1,156000,'2022-06-11 23:29:37'),(700000006,200,'remark test',4,25000,'2022-06-12 00:54:24'),(700000007,1000,'hjkggjj',2,23500,'2022-06-12 01:19:37'),(700000008,500,'',2,1299500,'2022-06-12 20:56:41'),(700000009,20,'testsetsetsetsetes',2,140000,'2022-06-16 02:08:02'),(700000010,99999999,'',3,199999998000,'2022-06-17 01:16:46'),(700000011,930,'',1,21855,'2022-06-26 01:41:20'),(700000012,10000,'',2,235000,'2022-06-26 18:44:24'),(700000013,4100,'',2,594000,'2022-06-27 21:15:30'),(700000014,650,'',2,264100,'2022-06-28 03:55:19');
/*!40000 ALTER TABLE `purchase_request` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-29  3:39:41
