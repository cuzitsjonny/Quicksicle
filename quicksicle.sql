-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 07, 2019 at 05:38 AM
-- Server version: 10.1.38-MariaDB
-- PHP Version: 7.3.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `quicksicle`
--
CREATE DATABASE IF NOT EXISTS `quicksicle` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `quicksicle`;

-- --------------------------------------------------------

--
-- Table structure for table `accounts`
--

CREATE TABLE `accounts` (
  `id` bigint(20) NOT NULL,
  `username` varchar(32) NOT NULL,
  `password_salt` char(64) NOT NULL,
  `password_iterations` int(11) NOT NULL,
  `password_hash` char(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `characters`
--

CREATE TABLE `characters` (
  `id` bigint(20) NOT NULL,
  `account_id` bigint(20) NOT NULL,
  `name` varchar(32) NOT NULL DEFAULT '',
  `pending_name` varchar(32) NOT NULL DEFAULT '',
  `pending_name_rejected` tinyint(1) NOT NULL DEFAULT '0',
  `head_color` int(11) UNSIGNED NOT NULL,
  `head` int(11) UNSIGNED NOT NULL,
  `chest_color` int(11) UNSIGNED NOT NULL,
  `chest` int(11) UNSIGNED NOT NULL,
  `legs` int(11) UNSIGNED NOT NULL,
  `hair_style` int(11) UNSIGNED NOT NULL,
  `hair_color` int(11) UNSIGNED NOT NULL,
  `left_hand` int(11) UNSIGNED NOT NULL,
  `right_hand` int(11) UNSIGNED NOT NULL,
  `eyebrow_style` int(11) UNSIGNED NOT NULL,
  `eyes_style` int(11) UNSIGNED NOT NULL,
  `mouth_style` int(11) UNSIGNED NOT NULL,
  `last_zone_id` smallint(5) UNSIGNED NOT NULL,
  `last_clone_id` int(11) UNSIGNED NOT NULL,
  `last_logout` datetime NOT NULL,
  `last_position_x` float NOT NULL,
  `last_position_y` float NOT NULL,
  `last_position_z` float NOT NULL,
  `last_rotation_x` float NOT NULL,
  `last_rotation_y` float NOT NULL,
  `last_rotation_z` float NOT NULL,
  `last_rotation_w` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `accounts`
--
ALTER TABLE `accounts`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- Indexes for table `characters`
--
ALTER TABLE `characters`
  ADD PRIMARY KEY (`id`),
  ADD KEY `FK_accounts_id_characters_account_id` (`account_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `accounts`
--
ALTER TABLE `accounts`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `characters`
--
ALTER TABLE `characters`
  ADD CONSTRAINT `FK_accounts_id_characters_account_id` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
