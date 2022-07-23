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
-- Table structure for table `good_return_note`
--

DROP TABLE IF EXISTS `good_return_note`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `good_return_note` (
  `good_return_id` int unsigned NOT NULL AUTO_INCREMENT,
  `item_id` varchar(20) NOT NULL,
  `good_return_qty` int NOT NULL,
  `invoice_id` int unsigned NOT NULL,
  `good_return_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `good_return_remarks` tinytext,
  `staff_id` char(5) NOT NULL,
  `customer_name` varchar(45) DEFAULT NULL,
  `customer_phone` char(8) DEFAULT NULL,
  `good_return_status` int DEFAULT '1',
  PRIMARY KEY (`good_return_id`),
  KEY `good_return_note_fk1` (`item_id`),
  KEY `good_return_note_fk2` (`invoice_id`),
  KEY `good_return_note_fk3` (`staff_id`),
  CONSTRAINT `good_return_note_fk1` FOREIGN KEY (`item_id`) REFERENCES `inventory` (`item_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `good_return_note_fk2` FOREIGN KEY (`invoice_id`) REFERENCES `sales_receipt` (`invoice_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `good_return_note_fk3` FOREIGN KEY (`staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1300000010 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `good_return_note`
--

LOCK TABLES `good_return_note` WRITE;
/*!40000 ALTER TABLE `good_return_note` DISABLE KEYS */;
INSERT INTO `good_return_note` VALUES (1300000001,'SDP100001005',1,400000002,'2022-05-23 15:38:13','fgj','I0001','dfhdfh','12345678',1),(1300000002,'SDP100001004',1,400000005,'2022-05-23 15:38:13','fghk','I0001','hjfgj','12345678',1),(1300000003,'SDP100001009',1,400000004,'2022-05-23 15:38:13','jhl','I0001','tgbfg','12345678',1),(1300000004,'SDP100001009',1,400000006,'2022-05-23 15:38:13','ghjk','I0001','brtrnyyum','12345678',1),(1300000005,'SDP100001009',1,400000007,'2022-05-23 15:38:13','ghl','I0001','ghngbfv','12345678',1),(1300000006,'SDP100001004',1,400000008,'2022-05-23 15:38:13','hjl','I0001','fvrr','12345678',1),(1300000007,'SDP100001009',1,400000007,'2022-06-12 08:58:16','dhdfhdf','I0001','fdsgsdfgsdf','12345678',1),(1300000008,'SDP100001015',1,400000083,'2022-06-12 09:10:45','ghfhgfghhg','I0001','fdshdhfhfghdf','52253553',2),(1300000009,'SDP100001020',1,400000144,'2022-06-27 11:57:50','','Z0002','TOM CHAN','56123333',1);
/*!40000 ALTER TABLE `good_return_note` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-29  3:39:43
