-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: dbordemservicos
-- ------------------------------------------------------
-- Server version	8.0.44

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
-- Table structure for table `dbpermissoes`
--

DROP TABLE IF EXISTS `dbpermissoes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dbpermissoes` (
  `IDPermissao` int NOT NULL AUTO_INCREMENT,
  `IDMenu` int DEFAULT NULL,
  `IDItensMenu` int DEFAULT NULL,
  `IDUsuario` int DEFAULT NULL,
  `Executar` bit(1) DEFAULT b'0',
  `Criar` bit(1) DEFAULT b'0',
  `Alterar` bit(1) DEFAULT b'0',
  `Excluir` bit(1) DEFAULT b'0',
  PRIMARY KEY (`IDPermissao`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dbpermissoes`
--

LOCK TABLES `dbpermissoes` WRITE;
/*!40000 ALTER TABLE `dbpermissoes` DISABLE KEYS */;
INSERT INTO `dbpermissoes` VALUES (1,1,1,25,_binary '',_binary '',_binary '',_binary ''),(2,1,2,25,_binary '',_binary '',_binary '',_binary ''),(3,1,3,25,_binary '',_binary '',_binary '',_binary ''),(4,1,5,25,_binary '',_binary '',_binary '',_binary ''),(6,4,27,25,_binary '',_binary '',_binary '',_binary ''),(7,5,18,25,_binary '',_binary '\0',_binary '\0',_binary '\0'),(8,5,19,25,_binary '',_binary '\0',_binary '\0',_binary '\0'),(9,1,7,25,_binary '',_binary '',_binary '',_binary ''),(10,1,9,25,_binary '',_binary '',_binary '',_binary ''),(11,1,8,25,_binary '',_binary '',_binary '',_binary ''),(12,1,1,40,_binary '',_binary '',_binary '',_binary ''),(14,1,2,40,_binary '',_binary '',_binary '',_binary '\0'),(15,1,6,25,_binary '',_binary '',_binary '',_binary ''),(16,1,4,25,_binary '',_binary '',_binary '',_binary ''),(17,5,16,25,_binary '',_binary '\0',_binary '\0',_binary '\0'),(18,5,17,25,_binary '',_binary '\0',_binary '\0',_binary '\0');
/*!40000 ALTER TABLE `dbpermissoes` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-25 23:38:51
