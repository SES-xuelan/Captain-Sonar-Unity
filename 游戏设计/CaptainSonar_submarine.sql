CREATE DATABASE  IF NOT EXISTS `CaptainSonar` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `CaptainSonar`;
-- MySQL dump 10.13  Distrib 5.7.12, for osx10.9 (x86_64)
--
-- Host: localhost    Database: CaptainSonar
-- ------------------------------------------------------
-- Server version	5.7.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `submarine`
--

DROP TABLE IF EXISTS `submarine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `submarine` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号，自动增长',
  `room` tinytext NOT NULL COMMENT '房间号',
  `team` tinytext NOT NULL COMMENT '队伍名，blue/red',
  `blood` int(10) unsigned zerofill DEFAULT NULL COMMENT '血量',
  `startpos` tinytext NOT NULL COMMENT '起始位置，json字符串，例如：{"x":"xx","y":"xxx"}',
  `currentpos` tinytext NOT NULL COMMENT '当前位置，json字符串，例如：{"x":"xx","y":"xxx"}',
  `way` longtext COMMENT '己方潜艇走的路线，潜艇上浮后清空，json字符串',
  `mine` int(11) DEFAULT '0' COMMENT '武器：水雷 记录当前充能',
  `torpedo` int(11) DEFAULT '0' COMMENT '武器：鱼雷 记录当前充能',
  `drone` int(11) DEFAULT '0' COMMENT '侦测：无人机 记录当前充能',
  `sonar` int(11) DEFAULT '0' COMMENT '侦测：声呐 记录当前充能',
  `silence` int(11) DEFAULT '0' COMMENT '特殊功能：潜行 记录当前充能',
  `scenario` int(11) DEFAULT '0' COMMENT '特殊功能：剧本 记录当前充能',
  `engineermap` longtext COMMENT '工程师负责的，潜艇哪部分损坏的map，json字符串',
  `radioway` longtext COMMENT '监听的敌方的路线，json字符串',
  `captain` tinytext NOT NULL COMMENT '负责这个职位的用户名',
  `firstmate` tinytext NOT NULL COMMENT '负责这个职位的用户名',
  `engineer` tinytext NOT NULL COMMENT '负责这个职位的用户名',
  `radiooperator` tinytext NOT NULL COMMENT '负责这个职位的用户名',
  `status` int(11) NOT NULL DEFAULT '0' COMMENT '游戏状态，0游戏已经结束或者压根就没开始；1正在等待玩家中；2游戏进行中 [用于断线重连]',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `submarine`
--

LOCK TABLES `submarine` WRITE;
/*!40000 ALTER TABLE `submarine` DISABLE KEYS */;
/*!40000 ALTER TABLE `submarine` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-11-17 10:12:35
