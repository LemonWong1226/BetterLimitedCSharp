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
-- Table structure for table `store_stock`
--

DROP TABLE IF EXISTS `store_stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `store_stock` (
  `store_stock_id` char(6) NOT NULL,
  `store_id` char(4) NOT NULL,
  `item_id` varchar(20) NOT NULL,
  `store_stock_qty` int NOT NULL,
  `stock_alarm` int NOT NULL,
  `store_stock_remarks` tinytext,
  `is_low_level` tinyint(1) NOT NULL,
  PRIMARY KEY (`store_stock_id`),
  UNIQUE KEY `store_item` (`store_id`,`item_id`),
  KEY `store_stock_fk2` (`item_id`),
  CONSTRAINT `store_stock_fk2` FOREIGN KEY (`item_id`) REFERENCES `inventory` (`item_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `store_stock_cc1` CHECK ((`store_id` in (_utf8mb3'HK01',_utf8mb3'HK02',_utf8mb3'CN01',_utf8mb3'hk01',_utf8mb3'hk02',_utf8mb3'cn01')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `store_stock`
--

LOCK TABLES `store_stock` WRITE;
/*!40000 ALTER TABLE `store_stock` DISABLE KEYS */;
INSERT INTO `store_stock` VALUES ('A00001','HK01','SDP100001001',1387,1401,'remark test',0),('A00002','HK01','SDP100001002',1009,15,'dfh',0),('A00003','HK01','SDP100001015',5,10,'iphone 8',1),('A00004','HK01','SDP100001004',8,9,'dfj',0),('A00005','HK01','SDP100001005',7,9,'hj',0),('A00006','HK01','SDP100001011',12,11,'fhj',0),('A00007','HK01','SDP100001007',8,10,'hjgjkkgjgh',1),('A00008','HK01','SDP100001006',500,10,'hfjfgjhgjf',1),('A00009','HK01','SDP100001008',4,10,'fgfgkfj',1),('A00010','HK01','SDP100001012',9,10,'iphone X',1),('A00011','HK01','SDP100001013',10,11,'jhkk',1),('A00012','HK01','SDP100001009',10,10,'hjhjhhghj',1),('A00013','HK01','SDP100001018',2160,2168,'ryfdg',0),('A00014','HK01','SDP100001019',1010,990,'SDASDS',0),('A00015','HK01','sdp100001020',194,198,'',0),('A00016','HK01','SDP100001003',220,10,'sds',0),('A00017','HK01','SDP100001021',299,10,'',0),('B00001','HK02','SDP100001001',339,0,'remark test',0),('B00002','HK02','SDP100001002',239,0,'fgj',0),('B00003','HK02','SDP100001015',15,15,'hgjhfjgfjg',1),('B00004','HK02','SDP100001004',200,0,'fgj',0),('B00005','HK02','SDP100001005',0,0,'fgj',0),('B00006','HK02','SDP100001011',5,0,'fhj',0),('B00007','HK02','SDP100001009',10,0,'fhj',0),('B00008','HK02','SDP100001008',10,20,'dfjdjhjf',1),('B00009','HK02','SDP100001012',5,50,'sgddghghdghdhdhdhfg',1),('B00010','HK02','SDP100001006',30,10,'fgjgggghghghhg',0),('B00011','HK02','SDP100001007',20,10,'jkhjjhhhkjkkhj',0),('B00012','HK02','SDP100001010',100,20,'uyjhhgjkjjgkg',0),('B00013','HK02','SDP100001020',1000,50,'dsds',0),('B00014','HK02','SDP100001021',200,198,'test case 3',0);
/*!40000 ALTER TABLE `store_stock` ENABLE KEYS */;
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
