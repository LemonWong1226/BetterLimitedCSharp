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
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `user_id` varchar(50) NOT NULL,
  `user_password` varchar(50) NOT NULL,
  `first_name` varchar(40) NOT NULL,
  `last_name` varchar(15) NOT NULL,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES ('a','a','admin','admin','2022-06-21 14:21:04'),('ac','ac','q','q','2022-06-22 13:12:34'),('acm','acm','q','q','2022-06-22 13:12:38'),('admin','admin999','Lemon','Wong','2022-06-28 10:27:06'),('dghfghhdg','asdasd','fdssgfsdgdsfd','dfhdfhdf','2022-06-21 14:16:42'),('i','i','q','q','2022-06-22 13:11:59'),('im','im','q','q','2022-06-22 13:12:05'),('m000007','cheung000007','chak hoi','cheung','2022-05-23 15:38:13'),('m000008','kowk000008','tze ming','kowk','2022-05-23 15:38:13'),('m000009','choi000009','sing yan','choi','2022-05-23 15:38:13'),('m000010','cheng000010','tze ming','cheng','2022-05-23 15:38:13'),('m000011','cheng000011','yan kit','cheng','2022-05-23 15:38:13'),('p','p','q','q','2022-06-22 13:12:46'),('p000001','chan000001','siu ming','chan','2022-05-23 15:38:13'),('p000002','chan000002','da man','chan','2022-05-23 15:38:13'),('p000003','lam000003','da fai','lam','2022-05-23 15:38:13'),('p000004','cheng000004','tze ming','cheng','2022-05-23 15:38:13'),('p000005','wong000005','tze yin','wong','2022-05-23 15:38:13'),('p000006','wong000006','wai sing','wong','2022-05-23 15:38:13'),('p000012','chan000012','siu chi','chan','2022-05-23 15:38:13'),('p000013','chan000013','wan ting','chan','2022-05-23 15:38:13'),('p000014','mok000014','kai ming','mok','2022-05-23 15:38:13'),('peter','aaa123','Lee','Chi ho','2022-06-27 18:00:56'),('pm','pm','q','q','2022-06-22 13:12:50'),('root3','1234567','root','account','2022-06-05 06:57:15'),('s','s','q','q','2022-06-24 10:54:57'),('sm','sm','q','q','2022-06-22 13:11:50'),('t','t','q','q','2022-06-22 13:12:14'),('testgroup','testgroup','testgroup','testgroup','2022-06-15 11:39:08'),('tm','tm','q','q','2022-06-22 13:12:22');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
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
