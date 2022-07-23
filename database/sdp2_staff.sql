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
-- Table structure for table `staff`
--

DROP TABLE IF EXISTS `staff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staff` (
  `staff_id` char(5) NOT NULL,
  `department` int NOT NULL,
  `position` int NOT NULL,
  `staff_active` char(1) NOT NULL,
  `user_id` varchar(10) NOT NULL,
  `store_id` char(4) DEFAULT NULL,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `email` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`staff_id`),
  KEY `staff_fk1` (`user_id`),
  KEY `staff_fk2` (`store_id`),
  KEY `department_fk1` (`department`,`position`),
  CONSTRAINT `group` FOREIGN KEY (`department`, `position`) REFERENCES `group_permission` (`department`, `position`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `staff_fk1` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `staff_fk2` FOREIGN KEY (`store_id`) REFERENCES `store` (`store_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `staff_cc3` CHECK ((`staff_active` in (_utf8mb3'Y',_utf8mb3'N',_utf8mb3'y',_utf8mb3'n')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
INSERT INTO `staff` VALUES ('A0001',4,1,'y','p000003','HK01','2022-06-03 07:03:01','test@gmail.com'),('A0002',4,2,'y','m000010','HK01','2022-06-01 16:18:31','test@gmail.com'),('A0003',4,1,'y','ac','HK01','2022-06-22 13:12:34','s@gmail.com'),('A0004',4,2,'y','acm','HK01','2022-06-22 13:12:38','s@gmail.com'),('I0001',2,1,'y','p000002','HK02','2022-06-03 07:03:01','test@gmail.com'),('I0002',2,1,'y','i','HK01','2022-06-22 13:11:59','s@gmail.com'),('I0003',2,2,'y','im','HK01','2022-06-22 13:12:05','s@gmail.com'),('P0001',5,1,'y','p000004','HK02','2022-06-03 07:03:01','test@gmail.com'),('P0002',5,2,'y','m000011','HK01','2022-06-01 16:18:31','test@gmail.com'),('P0003',5,1,'y','p','HK01','2022-06-22 13:12:46','s@gmail.com'),('P0004',5,2,'y','pm','HK01','2022-06-22 13:12:50','s@gmail.com'),('S0001',1,1,'y','p000001','HK01','2022-06-01 16:18:31','test@gmail.com'),('S0002',1,1,'y','p000005','HK02','2022-06-01 16:18:31','test@gmail.com'),('S0003',1,2,'y','m000007','HK01','2022-06-01 16:18:31','test@gmail.com'),('S0004',1,2,'y','m000008','HK02','2022-06-01 16:18:31','test@gmail.com'),('S0005',1,1,'y','s','HK01','2022-06-22 13:11:34','s@gmail.com'),('S0006',1,2,'y','sm','HK01','2022-06-22 13:11:50','s@gmail.com'),('T0001',3,2,'y','m000009','HK02','2022-06-03 07:03:01','test@gmail.com'),('T0002',3,1,'y','p000012','HK01','2022-06-03 07:03:01','test@gmail.com'),('T0003',3,1,'y','p000013','HK01','2022-06-03 07:03:01','test@gmail.com'),('T0004',3,1,'y','p000014','HK01','2022-06-03 07:03:01','test@gmail.com'),('T0005',3,1,'y','t','HK01','2022-06-22 13:12:14','s@gmail.com'),('T0006',3,2,'y','tm','HK01','2022-06-22 13:12:22','s@gmail.com'),('Z0001',99,99,'y','peter','HK01','2022-06-03 09:59:35','leechiho@gmail.com'),('Z0002',99,99,'y','a','HK01','2022-06-03 07:03:01','test@gmail.com'),('Z0003',99,99,'y','root3','HK02','2022-06-05 06:57:15','rootaccount@gmail.com'),('Z0004',99,99,'y','testgroup','HK01','2022-06-15 11:39:56','sdfgfdsg@gmail.com'),('Z0005',99,99,'y','dghfghhdg','HK01','2022-06-21 14:16:42','fdgsdfgsdf@gmail.com'),('Z0006',99,99,'y','admin','HK02','2022-06-27 18:11:44','lemon@gmail.com');
/*!40000 ALTER TABLE `staff` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-29  3:39:40
