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
-- Table structure for table `reorder_request`
--

DROP TABLE IF EXISTS `reorder_request`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reorder_request` (
  `reorder_id` int unsigned NOT NULL AUTO_INCREMENT,
  `reorder_ttl_qty` int NOT NULL,
  `reorder_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `staff_id` char(5) NOT NULL,
  `store_id` char(4) NOT NULL,
  `reorder_status` int DEFAULT '1',
  `remarks` longtext,
  PRIMARY KEY (`reorder_id`),
  KEY `reorder_request_fk1` (`staff_id`),
  KEY `reorder_request_fk2` (`store_id`),
  CONSTRAINT `reorder_request_fk1` FOREIGN KEY (`staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `reorder_request_fk2` FOREIGN KEY (`store_id`) REFERENCES `store` (`store_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=500000029 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reorder_request`
--

LOCK TABLES `reorder_request` WRITE;
/*!40000 ALTER TABLE `reorder_request` DISABLE KEYS */;
INSERT INTO `reorder_request` VALUES (500000001,150,'2022-05-23 15:38:13','S0003','HK01',2,'remarls test'),(500000002,100,'2022-05-23 15:38:13','S0003','HK01',2,'remarls test'),(500000003,100,'2022-05-23 15:38:13','S0004','HK02',3,'remarls test'),(500000004,100,'2022-05-23 15:38:13','S0004','HK02',1,'remarls test'),(500000005,10,'2022-05-23 15:38:13','S0003','HK01',4,'remarls test'),(500000006,20,'2022-05-23 15:38:13','S0004','HK02',3,'remarls test'),(500000008,70,'2022-06-10 16:48:35','Z0002','HK01',2,'remarls test'),(500000009,100,'2022-06-11 08:47:52','Z0002','HK01',2,'hjfhffhfjf'),(500000010,9999,'2022-06-11 09:14:58','Z0002','HK01',2,'hhghhhjj'),(500000011,99999,'2022-06-11 09:16:56','Z0002','HK01',1,'jhjhhh'),(500000013,8888,'2022-06-11 09:17:42','Z0002','HK01',1,''),(500000014,17776,'2022-06-11 09:17:47','Z0002','HK01',3,''),(500000015,26664,'2022-06-11 09:17:48','Z0002','HK01',4,''),(500000016,50,'2022-06-11 09:18:17','Z0002','HK01',2,''),(500000017,0,'2022-06-11 09:21:01','Z0002','HK02',1,''),(500000018,0,'2022-06-11 09:21:04','Z0002','HK01',1,''),(500000019,0,'2022-06-11 09:22:37','Z0002','HK01',1,''),(500000020,99999,'2022-06-11 16:48:02','Z0002','HK01',4,''),(500000021,20,'2022-06-12 13:12:34','Z0002','HK01',2,''),(500000022,20,'2022-06-15 18:08:47','Z0002','HK01',2,''),(500000023,1430,'2022-06-25 17:42:16','Z0002','HK01',2,''),(500000024,1,'2022-06-27 12:40:12','Z0002','HK01',2,''),(500000025,20,'2022-06-27 12:50:44','Z0002','HK01',3,''),(500000026,200,'2022-06-27 17:03:19','Z0002','HK01',2,''),(500000027,500,'2022-06-27 19:32:22','Z0006','HK01',2,''),(500000028,99,'2022-06-28 10:47:35','Z0002','HK01',2,'');
/*!40000 ALTER TABLE `reorder_request` ENABLE KEYS */;
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