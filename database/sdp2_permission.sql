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
-- Table structure for table `permission`
--

DROP TABLE IF EXISTS `permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission` (
  `permission_id` int NOT NULL AUTO_INCREMENT,
  `permission_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`permission_id`)
) ENGINE=InnoDB AUTO_INCREMENT=71 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permission`
--

LOCK TABLES `permission` WRITE;
/*!40000 ALTER TABLE `permission` DISABLE KEYS */;
INSERT INTO `permission` VALUES (1,'Notification Details'),(2,'Notification'),(3,'Confirm or Reject Reorder Request'),(4,'Cancel Reorder Request'),(5,'Statement'),(6,'Add Record Request'),(7,'Item Detail'),(8,'Edit Item'),(9,'Add Item'),(10,'Checkout'),(11,'Re-order Record'),(12,'Re-order Request'),(13,'HK02'),(14,'HK01'),(15,'Sale Management Button'),(16,'Permission'),(17,'Supplier Details'),(18,'Edit Supplier'),(19,'Add New Supplier'),(20,'View Supplier'),(21,'Print Good Return Note'),(22,'Reject or Confirm Defect Item'),(23,'Defect Item Details'),(24,'Add Defect Item'),(25,'View Defect Item'),(26,'Inventory Item Details'),(27,'Edit Inventory Item'),(28,'Add Inventory Item'),(29,'Inventory'),(30,'Reject or Confirm Delivery Order'),(31,'Cancel Delivery Order'),(32,'Print Delivery Note'),(33,'View Sale Order Detail'),(34,'Edit Delivery Order'),(35,'New Delivery Order'),(36,'View Delivery Order Record'),(37,'View Confirm Delivery Order'),(38,'View Delivery Table'),(39,'Delivery'),(40,'Genarate Purchase Order'),(41,'Reject or Confirm Purchase Order'),(42,'View Purchase Order Details'),(43,'Confirm or Reject Purchase Request Order'),(44,'Cancel Purchase Request Order'),(45,'Purchase Request Order Details'),(46,'Add Purchase Request Order'),(47,'View Good Received Note'),(48,'View Purchase Order'),(49,'View Purchase Request Order'),(50,'Purchase'),(51,'Installation Order Record'),(52,'Confirm or Reject Installation Order'),(53,'Cancel Installation Order'),(54,'View Confirm Installation Order Details'),(55,'Confirm Installation Order'),(56,'Edit Technical Support Order'),(57,'View Technical Support Order Details'),(58,'New Technical Support Order'),(59,'View Technical Support Table'),(60,'Technical Support'),(61,'Export Purchase Order'),(62,'Export Payment Receipt'),(63,'Account'),(64,'Change Name'),(65,'Create Account'),(66,'Language'),(67,'Change Password'),(68,'Setting'),(69,'Sale Record'),(70,'View Delivery Order');
/*!40000 ALTER TABLE `permission` ENABLE KEYS */;
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
