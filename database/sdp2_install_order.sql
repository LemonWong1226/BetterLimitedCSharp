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
-- Table structure for table `install_order`
--

DROP TABLE IF EXISTS `install_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `install_order` (
  `install_order_id` int unsigned NOT NULL AUTO_INCREMENT,
  `delivery_order_id` int unsigned NOT NULL,
  `order_id` int unsigned NOT NULL,
  `invoice_id` int unsigned NOT NULL,
  `install_time` int NOT NULL,
  `install_date` date DEFAULT NULL,
  `install_status` int DEFAULT '1',
  `sales_staff_id` char(5) NOT NULL,
  `worker_staff_id` char(5) DEFAULT NULL,
  `worker_duty` varchar(45) DEFAULT NULL,
  `install_remark` longtext,
  PRIMARY KEY (`install_order_id`),
  KEY `install_order_fk1` (`delivery_order_id`),
  KEY `install_order_fk2` (`order_id`),
  KEY `install_order_fk3` (`invoice_id`),
  KEY `install_order_fk4` (`sales_staff_id`),
  KEY `install_order_fk5` (`worker_staff_id`),
  CONSTRAINT `install_order_fk1` FOREIGN KEY (`delivery_order_id`) REFERENCES `delivery_order` (`delivery_order_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `install_order_fk2` FOREIGN KEY (`order_id`) REFERENCES `sales_order` (`order_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `install_order_fk3` FOREIGN KEY (`invoice_id`) REFERENCES `sales_receipt` (`invoice_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `install_order_fk4` FOREIGN KEY (`sales_staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `install_order_fk5` FOREIGN KEY (`worker_staff_id`) REFERENCES `staff` (`staff_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1200000022 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `install_order`
--

LOCK TABLES `install_order` WRITE;
/*!40000 ALTER TABLE `install_order` DISABLE KEYS */;
INSERT INTO `install_order` VALUES (1200000001,1000000001,200000001,400000001,2000,'2022-05-23',2,'S0001','T0002','duty test','remarks test'),(1200000003,1000000003,200000003,400000003,1400,'2022-05-23',3,'S0001','T0004','duty test','remarks test'),(1200000004,1000000004,200000004,400000004,1600,'2022-06-04',2,'S0001','T0003','duty test','remarks test'),(1200000005,1000000005,200000005,400000005,1400,'2022-05-24',4,'S0001','T0003','duty test','remarks test'),(1200000019,1000000013,200000022,400000019,1400,'2022-06-10',2,'Z0002','T0002','gssfdfgssfd','remarks test'),(1200000020,1000000010,200000016,400000013,1420,'2022-06-04',2,'Z0002','T0003','test','dsfhdghf'),(1200000021,1000000073,200000114,400000123,1700,'2022-06-27',1,'Z0002','T0002','try','sds');
/*!40000 ALTER TABLE `install_order` ENABLE KEYS */;
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
