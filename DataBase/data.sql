USE [Sanguosha-data]
GO
ALTER TABLE [dbo].[generals] DROP CONSTRAINT [kingdom]
GO
ALTER TABLE [dbo].[general_skin] DROP CONSTRAINT [id]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [卡牌数字在A~K之间]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [卡牌类型]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [卡牌花色]
GO
ALTER TABLE [dbo].[general_skin] DROP CONSTRAINT [FK_general_skin_generals]
GO
ALTER TABLE [dbo].[general_skill] DROP CONSTRAINT [FK_general-skill_generals]
GO
ALTER TABLE [dbo].[general_hp_adjust] DROP CONSTRAINT [FK_general_hp_adjust_generals]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [FK_cards_card_package]
GO
ALTER TABLE [dbo].[achieve] DROP CONSTRAINT [FK_achieve_title]
GO
ALTER TABLE [dbo].[skills] DROP CONSTRAINT [DF_skills_attach_lord]
GO
ALTER TABLE [dbo].[generals] DROP CONSTRAINT [DF_generals_hidden]
GO
ALTER TABLE [dbo].[generals] DROP CONSTRAINT [DF_generals_classic_lord]
GO
ALTER TABLE [dbo].[generals] DROP CONSTRAINT [DF_generals_adjust_hp]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [DF_cards_can_recast]
GO
ALTER TABLE [dbo].[cards] DROP CONSTRAINT [DF_cards_transferable]
GO
/****** Object:  Index [IX_translation]    Script Date: 2018/11/21 0:44:10 ******/
DROP INDEX [IX_translation] ON [dbo].[translation]
GO
/****** Object:  Index [IX_title]    Script Date: 2018/11/21 0:44:10 ******/
DROP INDEX [IX_title] ON [dbo].[title]
GO
/****** Object:  Table [dbo].[translation]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[translation]
GO
/****** Object:  Table [dbo].[title]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[title]
GO
/****** Object:  Table [dbo].[skills]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[skills]
GO
/****** Object:  Table [dbo].[pair_value]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[pair_value]
GO
/****** Object:  Table [dbo].[generals]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[generals]
GO
/****** Object:  Table [dbo].[general_value]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[general_value]
GO
/****** Object:  Table [dbo].[general_skin]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[general_skin]
GO
/****** Object:  Table [dbo].[general_skill]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[general_skill]
GO
/****** Object:  Table [dbo].[general_package]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[general_package]
GO
/****** Object:  Table [dbo].[general_hp_adjust]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[general_hp_adjust]
GO
/****** Object:  Table [dbo].[game_mode]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[game_mode]
GO
/****** Object:  Table [dbo].[function_card]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[function_card]
GO
/****** Object:  Table [dbo].[cards]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[cards]
GO
/****** Object:  Table [dbo].[card_package]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[card_package]
GO
/****** Object:  Table [dbo].[achieve]    Script Date: 2018/11/21 0:44:10 ******/
DROP TABLE [dbo].[achieve]
GO
USE [master]
GO
/****** Object:  Database [Sanguosha-data]    Script Date: 2018/11/21 0:44:10 ******/
DROP DATABASE [Sanguosha-data]
GO
/****** Object:  Database [Sanguosha-data]    Script Date: 2018/11/21 0:44:10 ******/
CREATE DATABASE [Sanguosha-data]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Sanguosha-data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Sanguosha-data.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Sanguosha-data_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\Sanguosha-data_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Sanguosha-data] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Sanguosha-data].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Sanguosha-data] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Sanguosha-data] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Sanguosha-data] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Sanguosha-data] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Sanguosha-data] SET ARITHABORT OFF 
GO
ALTER DATABASE [Sanguosha-data] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Sanguosha-data] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Sanguosha-data] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Sanguosha-data] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Sanguosha-data] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Sanguosha-data] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Sanguosha-data] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Sanguosha-data] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Sanguosha-data] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Sanguosha-data] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Sanguosha-data] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Sanguosha-data] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Sanguosha-data] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Sanguosha-data] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Sanguosha-data] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Sanguosha-data] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Sanguosha-data] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Sanguosha-data] SET RECOVERY FULL 
GO
ALTER DATABASE [Sanguosha-data] SET  MULTI_USER 
GO
ALTER DATABASE [Sanguosha-data] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Sanguosha-data] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Sanguosha-data] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Sanguosha-data] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Sanguosha-data] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Sanguosha-data', N'ON'
GO
ALTER DATABASE [Sanguosha-data] SET QUERY_STORE = OFF
GO
USE [Sanguosha-data]
GO
/****** Object:  Table [dbo].[achieve]    Script Date: 2018/11/21 0:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[achieve](
	[achieve_id] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[descript] [varchar](50) NOT NULL,
	[reward_title_id] [int] NULL,
	[reward_image1_id] [int] NULL,
	[reward_image2_id] [int] NULL,
	[reward_image3_id] [int] NULL,
 CONSTRAINT [PK_achieve] PRIMARY KEY CLUSTERED 
(
	[achieve_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[card_package]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[card_package](
	[package_name] [varchar](50) NOT NULL,
	[mode] [varchar](50) NOT NULL,
	[translation] [varchar](50) NOT NULL,
	[index] [int] NOT NULL,
 CONSTRAINT [PK_card_package] PRIMARY KEY CLUSTERED 
(
	[package_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cards]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cards](
	[id] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[suit] [varchar](50) NOT NULL,
	[number] [int] NOT NULL,
	[type] [varchar](50) NOT NULL,
	[mode] [varchar](50) NOT NULL,
	[package] [varchar](50) NOT NULL,
	[transferable] [bit] NOT NULL,
	[can_recast] [bit] NOT NULL,
 CONSTRAINT [PK_cards] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[function_card]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[function_card](
	[index] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[name_translation] [varchar](50) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[description] [varchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[game_mode]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[game_mode](
	[mode_name] [varchar](50) NOT NULL,
	[players_count] [int] NOT NULL,
	[is_scenario] [bit] NOT NULL,
	[index] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[general_hp_adjust]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[general_hp_adjust](
	[general_name] [varchar](50) NOT NULL,
	[hp_adjust] [int] NOT NULL,
	[is_head] [bit] NOT NULL,
 CONSTRAINT [PK_general_hp_adjust] PRIMARY KEY CLUSTERED 
(
	[general_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[general_package]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[general_package](
	[package_name] [varchar](50) NOT NULL,
	[mode] [varchar](50) NOT NULL,
	[translation] [varchar](50) NOT NULL,
	[index] [int] NOT NULL,
 CONSTRAINT [PK_general_package] PRIMARY KEY CLUSTERED 
(
	[package_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[general_skill]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[general_skill](
	[general] [varchar](50) NOT NULL,
	[skill_names] [varchar](50) NOT NULL,
	[related_skills] [varchar](50) NULL,
	[mode] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[general_skin]    Script Date: 2018/11/21 0:44:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[general_skin](
	[general_name] [varchar](50) NOT NULL,
	[skin_id] [int] NOT NULL,
	[title] [varchar](50) NOT NULL,
	[is_animation] [bit] NOT NULL,
	[quality] [int] NULL,
	[CV] [varchar](50) NULL,
	[illustrator] [varchar](50) NULL,
	[mode] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[general_value]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[general_value](
	[general] [varchar](50) NOT NULL,
	[value] [int] NOT NULL,
	[mode] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[generals]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[generals](
	[general_name] [varchar](50) NOT NULL,
	[sex] [bit] NOT NULL,
	[kingdom] [varchar](50) NOT NULL,
	[HP] [int] NOT NULL,
	[adjust_hp] [int] NOT NULL,
	[package] [varchar](max) NULL,
	[hegemony_lord] [bit] NOT NULL,
	[classic_lord] [bit] NOT NULL,
	[hidden] [bit] NOT NULL,
	[companion] [varchar](50) NULL,
	[translation] [varchar](50) NULL,
 CONSTRAINT [PK_generals] PRIMARY KEY CLUSTERED 
(
	[general_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pair_value]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pair_value](
	[general1] [varchar](50) NOT NULL,
	[general2] [varchar](50) NOT NULL,
	[value1] [int] NOT NULL,
	[value2] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skills]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[skills](
	[index] [int] NOT NULL,
	[skill_name] [varchar](50) NOT NULL,
	[preshow] [bit] NOT NULL,
	[frequency] [int] NOT NULL,
	[attach_lord] [bit] NOT NULL,
	[translation] [varchar](50) NOT NULL,
	[description] [varchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[title]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[title](
	[title_id] [int] NOT NULL,
	[title_name] [varchar](50) NOT NULL,
	[describe] [varchar](50) NULL,
	[translation] [varchar](50) NULL,
 CONSTRAINT [PK_title] PRIMARY KEY CLUSTERED 
(
	[title_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[translation]    Script Date: 2018/11/21 0:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[translation](
	[key] [varchar](50) NOT NULL,
	[translation] [varchar](max) NOT NULL,
	[path] [varchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[card_package] ([package_name], [mode], [translation], [index]) VALUES (N'GuanduCards', N'GuanduWarfare', N'官渡标准', 3)
INSERT [dbo].[card_package] ([package_name], [mode], [translation], [index]) VALUES (N'LordCards', N'Hegemony', N'君主卡牌', 2)
INSERT [dbo].[card_package] ([package_name], [mode], [translation], [index]) VALUES (N'StanderCards', N'Hegemony', N'国战标准', 0)
INSERT [dbo].[card_package] ([package_name], [mode], [translation], [index]) VALUES (N'StrategicAdvantageCards', N'Hegemony', N'势备', 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (0, N'Slash', N'spade', 5, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (1, N'Slash', N'spade', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (2, N'Slash', N'spade', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (3, N'Slash', N'spade', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (4, N'Slash', N'spade', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (5, N'Slash', N'spade', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (6, N'Slash', N'spade', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (7, N'Slash', N'club', 2, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (8, N'Slash', N'club', 3, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (9, N'Slash', N'club', 4, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (10, N'Slash', N'club', 5, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (11, N'Slash', N'club', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (12, N'Slash', N'club', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (13, N'Slash', N'club', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (14, N'Slash', N'club', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (15, N'Slash', N'club', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (16, N'Slash', N'heart', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (17, N'Slash', N'heart', 12, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (18, N'Slash', N'diamond', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (19, N'Slash', N'diamond', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (20, N'Slash', N'diamond', 12, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (21, N'FireSlash', N'heart', 4, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (22, N'FireSlash', N'diamond', 4, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (23, N'FireSlash', N'diamond', 5, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (24, N'ThunderSlash', N'spade', 6, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (25, N'ThunderSlash', N'spade', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (26, N'ThunderSlash', N'club', 6, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (27, N'ThunderSlash', N'club', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (28, N'ThunderSlash', N'club', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (29, N'Jink', N'heart', 2, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (30, N'Jink', N'heart', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (31, N'Jink', N'heart', 13, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (32, N'Jink', N'diamond', 2, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (33, N'Jink', N'diamond', 3, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (34, N'Jink', N'diamond', 6, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (35, N'Jink', N'diamond', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (36, N'Jink', N'diamond', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (37, N'Jink', N'diamond', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (38, N'Jink', N'diamond', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (39, N'Jink', N'diamond', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (40, N'Jink', N'diamond', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (41, N'Jink', N'diamond', 11, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (42, N'Jink', N'diamond', 13, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (43, N'Peach', N'heart', 4, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (44, N'Peach', N'heart', 6, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (45, N'Peach', N'heart', 7, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (46, N'Peach', N'heart', 8, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (47, N'Peach', N'heart', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (48, N'Peach', N'heart', 10, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (49, N'Peach', N'heart', 12, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (50, N'Peach', N'diamond', 2, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (51, N'Analeptic', N'spade', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (52, N'Analeptic', N'club', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (53, N'Analeptic', N'diamond', 9, N'basic', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (54, N'CrossBow', N'diamond', 1, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (55, N'DoubleSword', N'spade', 2, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (56, N'QinggangSword', N'spade', 6, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (57, N'IceSword', N'spade', 2, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (58, N'Spear', N'spade', 12, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (59, N'Fan', N'diamond', 1, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (60, N'Axe', N'diamond', 5, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (61, N'KylinBow', N'heart', 5, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (62, N'SixSwords', N'diamond', 6, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (63, N'Triblade', N'diamond', 12, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (64, N'EightDiagram', N'spade', 2, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (65, N'RenwangShield', N'club', 2, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (66, N'Vine', N'club', 2, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (67, N'SilverLion', N'club', 1, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (68, N'Jueying', N'spade', 5, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (69, N'Dilu', N'club', 5, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (70, N'Zhuahuangfeidian', N'heart', 13, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (71, N'Chitu', N'heart', 5, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (72, N'Dayuan', N'spade', 13, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (73, N'Zixing', N'diamond', 13, N'equip', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (74, N'AmazingGrace', N'heart', 3, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (75, N'GodSalvation', N'heart', 1, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (76, N'SavageAssault', N'spade', 13, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (77, N'SavageAssault', N'club', 7, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (78, N'ArcheryAttack', N'heart', 1, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (79, N'Duel', N'spade', 1, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (80, N'Duel', N'club', 1, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (81, N'ExNihilo', N'heart', 7, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (82, N'ExNihilo', N'heart', 8, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (83, N'Snatch', N'spade', 3, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (84, N'Snatch', N'spade', 4, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (85, N'Snatch', N'diamond', 3, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (86, N'Dismantlement', N'spade', 3, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (87, N'Dismantlement', N'spade', 4, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (88, N'Dismantlement', N'heart', 12, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (89, N'IronChain', N'spade', 12, N'trick', N'Hegemony', N'StanderCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (90, N'IronChain', N'club', 12, N'trick', N'Hegemony', N'StanderCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (91, N'IronChain', N'club', 13, N'trick', N'Hegemony', N'StanderCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (92, N'FireAttack', N'heart', 2, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (93, N'FireAttack', N'heart', 3, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (94, N'Collateral', N'club', 12, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (95, N'Nullification', N'spade', 11, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (96, N'HegNullification', N'club', 13, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (97, N'HegNullification', N'diamond', 12, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (98, N'AwaitExhausted', N'heart', 11, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (99, N'AwaitExhausted', N'diamond', 4, N'trick', N'Hegemony', N'StanderCards', 0, 0)
GO
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (100, N'KnownBoth', N'club', 3, N'trick', N'Hegemony', N'StanderCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (101, N'KnownBoth', N'club', 4, N'trick', N'Hegemony', N'StanderCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (102, N'BefriendAttacking', N'heart', 9, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (103, N'Indulgence', N'club', 6, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (104, N'Indulgence', N'heart', 6, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (105, N'SupplyShortage', N'spade', 10, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (106, N'SupplyShortage', N'club', 10, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (107, N'Lightning', N'spade', 1, N'trick', N'Hegemony', N'StanderCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (108, N'DragonPhoenix', N'spade', 2, N'equip', N'Hegemony', N'LordCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (109, N'PeaceSpell', N'heart', 3, N'equip', N'Hegemony', N'LordCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (110, N'Slash', N'spade', 4, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (111, N'Analeptic', N'spade', 6, N'basic', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (112, N'Slash', N'spade', 7, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (113, N'Slash', N'spade', 8, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (114, N'ThunderSlash', N'spade', 9, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (115, N'ThunderSlash', N'spade', 10, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (116, N'ThunderSlash', N'spade', 11, N'basic', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (117, N'Jink', N'heart', 4, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (118, N'Jink', N'heart', 5, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (119, N'Jink', N'heart', 6, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (120, N'Jink', N'heart', 7, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (121, N'Peach', N'heart', 8, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (122, N'Peach', N'heart', 9, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (123, N'Slash', N'heart', 10, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (124, N'Slash', N'heart', 11, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (125, N'Slash', N'club', 4, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (126, N'ThunderSlash', N'club', 5, N'basic', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (127, N'Slash', N'club', 6, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (128, N'Slash', N'club', 7, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (129, N'Slash', N'club', 8, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (130, N'Analeptic', N'club', 9, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (131, N'Peach', N'diamond', 2, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (132, N'Peach', N'diamond', 3, N'basic', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (133, N'Jink', N'diamond', 6, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (134, N'Jink', N'diamond', 7, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (135, N'FireSlash', N'diamond', 8, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (136, N'FireSlash', N'diamond', 9, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (137, N'Jink', N'diamond', 13, N'basic', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (138, N'ThreatenEmperor', N'spade', 1, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (139, N'BurningCamps', N'spade', 3, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (140, N'FightTogether', N'spade', 12, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (141, N'Nullification', N'spade', 13, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (142, N'AllianceFeast', N'heart', 1, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (143, N'LureTiger', N'heart', 2, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (144, N'BurningCamps', N'heart', 12, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (145, N'Drowning', N'heart', 13, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (146, N'ImperialOrder', N'club', 3, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (147, N'FightTogether', N'club', 10, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (148, N'BurningCamps', N'club', 11, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (149, N'Drowning', N'club', 12, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (150, N'HegNullification', N'club', 13, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (151, N'ThreatenEmperor', N'diamond', 1, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (152, N'ThreatenEmperor', N'diamond', 4, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (153, N'LureTiger', N'diamond', 10, N'trick', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (154, N'HegNullification', N'diamond', 11, N'trick', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (155, N'IronArmor', N'spade', 2, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (156, N'Blade', N'spade', 5, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (157, N'Jingfan', N'heart', 3, N'equip', N'Hegemony', N'StrategicAdvantageCards', 1, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (158, N'JadeSeal', N'club', 1, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (159, N'BreastPlate', N'club', 2, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 1)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (160, N'WoodenOx', N'diamond', 5, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (161, N'Halberd', N'diamond', 12, N'equip', N'Hegemony', N'StrategicAdvantageCards', 0, 0)
INSERT [dbo].[cards] ([id], [name], [suit], [number], [type], [mode], [package], [transferable], [can_recast]) VALUES (162, N'LuminouSpearl', N'diamond', 6, N'equip', N'Hegemony', N'LordCards', 0, 0)
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (1, N'Slash', N'杀', N'basic', N'基本牌\n\n使用时机：出牌阶段限一次。\n使用目标：你攻击范围内的一名角色。\n作用效果：你对目标角色造成1点伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (2, N'FireSlash', N'火杀', N'basic', N'基本牌\n\n使用时机：出牌阶段限一次。\n使用目标：你攻击范围内的一名角色。\n作用效果：你对目标角色造成1点火焰伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (3, N'ThunderSlash', N'雷杀', N'basic', N'基本牌\n\n使用时机：出牌阶段限一次。\n使用目标：你攻击范围内的一名角色。\n作用效果：你对目标角色造成1点雷电伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (4, N'Jink', N'闪', N'basic', N'基本牌\n\n使用时机：以你为目标的【杀】生效前。\n使用目标：以你为目标的【杀】。\n作用效果：抵消此【杀】。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (5, N'Peach', N'桃', N'basic', N'基本牌\n\n使用方法Ⅰ：\n使用时机：出牌阶段。\n使用目标：包括你在内的一名已受伤的角色。\n作用效果：目标角色回复1点体力。\n\n使用方法Ⅱ：\n使用时机：当一名角色处于濒死状态时。\n使用目标：一名处于濒死状态的角色。\n作用效果：目标角色回复1点体力。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (6, N'Analeptic', N'酒', N'basic', N'基本牌\n\n使用方法Ⅰ：\n使用时机：出牌阶段。每回合限一次。\n使用目标：包括你在内的一名角色。\n作用效果：目标角色于此回合内使用的下一张【杀】的伤害值基数+1。\n\n使用方法Ⅱ：\n使用时机：当你处于濒死状态时。\n使用目标：你。\n作用效果：你回复1点体力。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (7, N'CrossBow', N'诸葛连弩', N'equip', N'装备牌·武器\n\n攻击范围：1\n技能：你使用【杀】无次数限制。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (8, N'DoubleSword', N'雌雄双股剑', N'equip', N'装备牌·武器\n\n攻击范围：2\n技能：每当你使用【杀】指定与你性别不同的一个目标后，你可以令其选择一项：1.弃置一张手牌；2.令你摸一张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (9, N'QinggangSword', N'青缸剑', N'equip', N'装备牌·武器\n\n攻击范围：2\n技能：锁定技，每当你使用【杀】指定一个目标后，你无视其防具。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (10, N'IceSword', N'寒冰剑', N'equip', N'装备牌·武器\n\n攻击范围：2\n技能：每当你使用【杀】对目标角色造成伤害时，若其有牌，你可以防止此伤害，依次弃置其两张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (11, N'Spear', N'丈八蛇矛', N'equip', N'装备牌·武器\n\n攻击范围：3\n技能：你可以将两张手牌当【杀】使用或打出。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (12, N'Fan', N'朱雀羽扇', N'equip', N'装备牌·武器\n\n攻击范围：4\n技能：当你使用普通【杀】时，你可以将此【杀】改为火【杀】。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (13, N'Axe', N'贯石斧', N'equip', N'装备牌·武器\n\n攻击范围：3\n技能：每当你使用的【杀】被目标角色使用的【闪】抵消时，\n你可以弃置两张牌，令此【杀】依然对其生效。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (14, N'KylinBow', N'麒麟弓', N'equip', N'装备牌·武器\n\n攻击范围：5\n技能：每当你使用【杀】对目标角色造成伤害时，你可以弃置其装备区里的一张坐骑牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (15, N'SixSwords', N'吴六剑', N'equip', N'装备牌·武器\n\n攻击范围：2<br/>技能：锁定技，与你势力相同的其他角色的攻击范围+1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (16, N'Triblade', N'三尖两刃刀', N'equip', N'装备牌·武器\n\n攻击范围：3<br/>技能：每当你使用【杀】对目标角色造成伤害后，你可以弃置一张手牌并选择目标角色距离为1的一名其他角色，对其造成1点伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (17, N'EightDiagram', N'八卦阵', N'equip', N'装备牌·防具\n\n技能：每当你需要使用/打出【闪】时，你可以判定，若结果为红色，你视为使用/打出一张【闪】。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (18, N'RenwangShield', N'仁王盾', N'equip', N'装备牌·防具\n\n技能：锁定技，黑色【杀】对你无效。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (19, N'Vine', N'藤甲', N'equip', N'装备牌·防具\n\n技能：锁定技，【南蛮入侵】、【万箭齐发】和普通【杀】对你无效；\n锁定技，每当你受到火焰伤害时，你令此伤害+1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (20, N'SilverLion', N'白银狮子', N'equip', N'装备牌·防具\n\n技能：锁定技，每当你受到大于1点的伤害时，你令此伤害减至1点；锁定技，每当你失去装备区里的【白银狮子】后，你回复1点体力。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (21, N'Jueying', N'绝影', N'equip', N'装备牌·坐骑\n\n技能：其他角色与你的距离+1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (22, N'Dilu', N'的卢', N'equip', N'装备牌·坐骑\n\n技能：其他角色与你的距离+1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (23, N'Zhuahuangfeidian', N'爪黄飞电', N'equip', N'装备牌·坐骑\n\n技能：其他角色与你的距离+1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (24, N'Chitu', N'赤兔', N'equip', N'装备牌·坐骑\n\n技能：你与其他角色的距离-1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (25, N'Dayuan', N'大苑', N'equip', N'装备牌·坐骑\n\n技能：你与其他角色的距离-1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (26, N'Zixing', N'紫骍', N'equip', N'装备牌·坐骑\n\n技能：你与其他角色的距离-1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (27, N'AmazingGrace', N'五谷丰登', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有角色。\n执行动作：当你使用此牌时，你从亮出牌堆顶的X张牌（X为全场角色数）。\n作用效果：每名目标角色获得这些牌中（剩余）的一张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (28, N'GodSalvation', N'桃园结义', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有角色。\n作用效果：每名目标角色回复1点体力。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (29, N'SavageAssault', N'南蛮入侵', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有其他角色。\n作用效果：每名目标角色需打出【杀】，否则受到你造成的1点伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (30, N'ArcheryAttack', N'万箭齐发', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有其他角色。\n作用效果：每名目标角色需打出【闪】，否则受到你造成的1点伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (31, N'Duel', N'决斗', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名其他角色。\n作用效果：由目标角色开始，其与你轮流打出【杀】，直到其中的一名角色未打出【杀】。\n未打出【杀】的角色受到另一名角色造成的1点伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (32, N'ExNihilo', N'无中生有', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：包括你在内的一名角色。\n作用效果：目标角色摸两张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (33, N'Snatch', N'顺手牵羊', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：距离为1的一名区域里有牌的其他角色。\n作用效果：你获得目标角色的区域里的一张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (34, N'Dismantlement', N'过河拆桥', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名区域里有牌的其他角色。\n作用效果：你弃置目标角色的区域里的一张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (35, N'IronChain', N'铁索连环', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一至两名角色。\n作用效果：每名目标角色选择一项：1.横置；2. 重置。\n◆此牌能重铸。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (36, N'FireAttack', N'火攻', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名有手牌的角色。\n作用效果：目标角色展示一张手牌，然后你可以弃置与之花色相同的一张手牌，若如此做，其受到你造成的1点火焰伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (37, N'Collateral', N'借刀杀人', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名装备区里有武器牌且攻击范围内有其使用【杀】的合法目标的其他角色An。\n执行动作：你在选择An为目标的同时选择An攻击范围内的一个An使用【杀】的合法目标Bn；你在An也成为目标的同时选择An攻击范围内的一个An使用【杀】的合法目标Bn。\n作用效果：目标角色An需对Bn使用【杀】，否则将其装备区里的武器牌交给你。（n为目标角色的序号）。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (38, N'Nullification', N'无懈可击', N'trick', N'锦囊牌\n\n使用时机：一张锦囊牌对一个目标生效前。\n使用目标：一张对一个目标生效前的锦囊牌。\n作用效果：抵消此锦囊牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (39, N'HegNullification', N'无懈可击·国', N'trick', N'锦囊牌\n\n使用方法Ⅰ：\n使用时机：一张锦囊牌对一个目标生效前。\n使用目标：一张对一个目标生效前的锦囊牌。\n作用效果：抵消此锦囊牌。\n\n使用方法Ⅱ：\n使用时机：一张锦囊牌对一名目标角色生效前。\n使用目标：一张对一名目标角色生效前的锦囊牌。\n作用效果：抵消此牌，然后你选择所有除目标角色外与目标角色势力相同的角色，\n令所有角色不能使用【无懈可击】响应对这些角色结算的此牌，\n若如此做，每当此牌对你选择的这些角色中的一名角色生效前，抵消之。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (40, N'KnownBoth', N'知己知彼', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名其他角色。\n作用效果：你选择一项：\n1.观看目标角色的所有手牌；\n2.观看目标角色的一张暗置的武将牌。\n◆此牌能重铸。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (41, N'BefriendAttacking', N'远交近攻', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：与你势力不同的一名有明置的武将牌的角色。\n作用效果：目标角色摸一张牌，然后你摸三张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (42, N'AwaitExhausted', N'以逸待劳', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：你和与你势力相同的所有角色。\n作用效果：每名目标角色摸两张牌，然后每名目标角色弃置两张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (43, N'Indulgence', N'乐不思蜀', N'trick', N'延时锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名其他角色。\n作用效果：目标角色判定，若结果不为红桃，其跳过出牌阶段。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (44, N'SupplyShortage', N'兵粮寸断', N'trick', N'延时锦囊牌\n\n使用时机：出牌阶段。\n使用目标：距离为1的一名其他角色。\n作用效果：目标角色判定，若结果不为梅花，其跳过摸牌阶段。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (45, N'Lightning', N'闪电', N'trick', N'延时锦囊牌\n\n使用时机：出牌阶段。\n使用目标：你。\n作用效果：目标角色判定，若结果为黑桃2~9：其受到3点无来源的雷电伤害，将此【闪电】置入弃牌堆。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (46, N'DragonPhoenix', N'飞龙夺风', N'equip', N'装备牌·武器\n\n攻击范围：2\n技能：每当你使用【杀】指定一个目标后，你可以令其弃置一张牌；每当被你使用【杀】杀死的角色死亡后，若你的势力是角色最少的势力，你可以令扮演其的玩家选择是否从未使用的武将牌中选择与你势力相同的一张武将牌重新加入游戏。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (47, N'PeaceSpell', N'太平要术', N'equip', N'装备牌·防具\n\n技能：锁定技，每当你受到属性伤害时，你防止此伤害；\n锁定技，与你势力相同的角色的手牌上限+X（X为与你势力相同的角色数）；\n锁定技，每当你失去装备区里的【太平要术】后，你失去1点体力，然后摸两张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (48, N'ThreatenEmperor', N'挟天子以令诸侯', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：为大势力角色的你。\n作用效果：你结束出牌阶段，若如此做，此回合结束时，你可以弃置一张牌，获得一个额外的回合。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (49, N'BurningCamps', N'火烧连营', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：你的下家和除其外与其处于同一队列的所有角色。\n作用效果：目标角色受到你造成的1点火焰伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (50, N'FightTogether', N'戮力同心', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有大势力角色或所有小势力角色。\n作用效果：若目标角色：不处于连环状态，其横置；处于连环状态，其摸一张牌。\n◆此牌能重铸。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (51, N'AllianceFeast', N'联军盛宴', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：你和你选择的除你的势力外的一个势力的所有角色。\n作用效果：若目标角色：为你，你摸X张牌（X为该势力的角色数）；不为你，其选择一项：1. 回复1点体力；2. 摸一张牌，然后重置。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (52, N'LureTiger', N'调虎离山', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：至多两名其他角色。\n作用效果：目标角色于此回合结束之前不计入距离和座次计算且不能使用牌且不是牌的合法目标。\n执行动作：此牌结算结束时，你摸一张牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (53, N'Drowning', N'水淹七军', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：一名装备区里有牌的其他角色。\n作用效果：目标角色选择一项：1.弃置装备区里的所有牌；2.受到你造成的1点雷电伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (54, N'ImperialOrder', N'敕令', N'trick', N'锦囊牌\n\n使用时机：出牌阶段。\n使用目标：所有没有势力的角色。\n作用效果：目标角色选择一项：1. 明置一张武将牌，然后摸一张牌；2. 弃置一张装备牌；3. 失去1点体力。\n\n※若此牌未因使用此效果而进入弃牌堆时，则改为将此牌移出游戏，然后于此回合结束时视为对所有未确定势力的角色使用此牌。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (55, N'IronArmor', N'明光铠', N'equip', N'装备牌·防具\n\n技能：每当你成为【火烧连营】、【火攻】或火【杀】的目标时，你取消自己；若你是小势力角色，你的武将牌不能被横置。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (56, N'Blade', N'青龙偃月刀', N'equip', N'装备牌·武器\n\n攻击范围：3\n技能：锁定技，你使用【杀】时，目标角色不能明置武将牌直到此【杀】结算完毕。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (57, N'Jingfan', N'惊帆', N'equip', N'装备牌·坐骑\n\n技能：你与其他角色的距离-1。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (58, N'JadeSeal', N'玉玺', N'equip', N'装备牌·宝物\n\n技能：锁定技，若你有明置的武将牌，则：你所属的势力成为唯一的大势力；摸牌阶段摸牌时，你额外摸一张牌；出牌阶段开始时，你视为使用一张【知己知彼】。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (59, N'BreastPlate', N'护心境', N'equip', N'装备牌·防具\n\n技能：每当你受到伤害时，若此伤害不小于你的体力值，你可以将此牌从装备区置入弃牌堆，防止此伤害。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (60, N'WoodenOx', N'木牛流马', N'equip', N'装备牌·宝物\n\n技能：\n1. 出牌阶段限一次，你可以将一张手牌置于【木牛流马】下，若如此做，你可以将【木牛流马】移动至一名其他角色的装备区。\n2. 你可以将【木牛流马】下的牌视为手牌使用或打出。\n◆每当你失去装备区的【木牛流马】后，若【木牛流马】未移动至装备区，其下的牌置入弃牌堆，否则这些牌仍置于【木牛流马】下。\n◆【木牛流马】下的牌为移出游戏。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (61, N'Halberd', N'方天画戟', N'equip', N'装备牌·武器\n\n攻击范围：4\n技能：你使用的【杀】可以指定任意名势力各不相同的角色及未确定势力的角色为目标。当此【杀】被一名目标角色使用【闪】抵消时，此【杀】对其他目标角色无效。')
INSERT [dbo].[function_card] ([index], [name], [name_translation], [type], [description]) VALUES (62, N'LuminouSpearl', N'定澜夜明珠', N'equip', N'装备牌·宝物\n\n技能：锁定技，你视为拥有技能“制衡”若你已拥有“制衡”，则改为取消弃置牌数的限制。')
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'Hegemony', 8, 0, 0)
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'GuanduWarfare', 4, 1, 0)
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'Hegemony', 6, 0, 1)
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'Hegemony', 7, 0, 2)
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'Hegemony', 9, 0, 3)
INSERT [dbo].[game_mode] ([mode_name], [players_count], [is_scenario], [index]) VALUES (N'Hegemony', 10, 0, 4)
INSERT [dbo].[general_hp_adjust] ([general_name], [hp_adjust], [is_head]) VALUES (N'dengai', -1, 1)
INSERT [dbo].[general_hp_adjust] ([general_name], [hp_adjust], [is_head]) VALUES (N'jiangwei', -1, 0)
INSERT [dbo].[general_hp_adjust] ([general_name], [hp_adjust], [is_head]) VALUES (N'sunce', -1, 0)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'Authority', N'Hegemony', N'权', 4)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'Formation', N'Hegemony', N'阵', 1)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'GuanduLimited', N'GuanduWarfare', N'标准', 0)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'Momentum', N'Hegemony', N'势', 2)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'Stander', N'Hegemony', N'标准', 0)
INSERT [dbo].[general_package] ([package_name], [mode], [translation], [index]) VALUES (N'Transformation', N'Hegemony', N'变', 3)
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'caocao', N'jianxiong', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'simayi', N'fankui,guicai', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xiahoudun', N'ganglie', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhangliao', N'tuxi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xuchu', N'luoyi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'guojia', N'tiandu,yiji', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhenji', N'qingguo,luoshen', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xiahouyuan', N'shensu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhanghe', N'qiaobian', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xuhuang', N'duanliang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'caoren', N'jushou', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'dianwei', N'qiangxi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xunyu', N'quhu,jieming', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'caopi', N'xingshang,fangzhu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'yuejin', N'xiaoguo', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'huatuo', N'chuli,jijiu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lvbu', N'wushuang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'diaochan', N'lijian,biyue', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'yuanshao', N'luanji', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'yanliangwenchou', N'shuangxiong', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'jiaxu', N'wansha,weimu,luanwu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'pangde', N'mashu_pangde,jianchu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhangjiao', N'leiji,guidao', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'caiwenji', N'beige,duanchang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'mateng', N'mashu_mateng,xiongyi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'kongrong', N'mingshi,lirang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'jiling', N'shuangren', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'tianfeng', N'sijian,suishi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'panfeng', N'kuangfu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zoushi', N'huoshui,qingcheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'liubei', N'rende', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'guanyu', N'wusheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhangfei', N'paoxiao', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhugeliang', N'guanxing,kongcheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhaoyun', N'longdan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'machao', N'mashu_machao,tieqi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'huangyueying', N'jizhi,qicai', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'huangzhong', N'liegong', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'weiyan', N'kuanggu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'pangtong', N'lianhuan,niepan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'wolong', N'bazhen,huoji,kanpo', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'liushan', N'xiangle,fangquan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'menghuo', N'huoshou,zaiqi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhurong', N'juxiang,lieren', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'ganfuren', N'shushen,shenzhi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'sunquan', N'zhiheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'ganning', N'qixi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lvmeng', N'keji,mouduan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'huanggai', N'kurou', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhouyu', N'yingzi_zhouyu,fanjian', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'daqiao', N'guose,liuli', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'luxun', N'qianxun,duoshi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'sunshangxiang', N'jieyin,xiaoji', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'sunjian', N'yinghun_sunjian', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xiaoqiao', N'hongyan,tianxiang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'taishici', N'tianyi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhoutai', N'buqu,fenji', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lusu', N'haoshi,dimeng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'erzhang', N'zhijian,guzheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'dingfeng', N'duanbing,fenxun', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lidian', N'xunxun,wangxi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zangba', N'hengjiang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'madai', N'mashu_madai,qianxi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'mifuren', N'guixiu,cunsi', N'yongjue
', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'sunce', N'jiang,yingyang,hunshang', N'yinghun_sunce,yingzi_sunce', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'chenwudongxi', N'duanxie,fenming', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'dongzhuo', N'baoling,hengzheng', N'benghuai', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zhangren', N'chuanxin,fengshi', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lord_zhangjiao', N'wuxin,wendao,hongfa', N'hongfaslash
', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xunyou', N'qice,zhiyu', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'bianfuren', N'wanwei,yuejian', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'liguo', N'xiongsuan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'zuoci', N'huashen,xinsheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'shamoke', N'jili', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'masu', N'sanyao,zhiman', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lingtong', N'xuanlue,yongjin', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lvfan', N'diaodu,diancai', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lord_sunquan', N'jiahe,lianzi,jubao', N'flamemap,yingzi,haoshi,shelie,duoshi', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'dengai', N'jixi,tuntian,ziliang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'caohong', N'huyuan,heyi', N'feiying', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'jiangwei', N'tianfu,yizhi,tiaoxin', N'kanpo,guanxing', N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'jiangwanfeiyi', N'shengxi,shoucheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'jiangqin', N'shangyi,niaoxiang', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'xusheng', N'yicheng', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'yuji', N'qianhuan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'hetaihou', N'zhengdu,qiluan', NULL, N'Hegemony')
INSERT [dbo].[general_skill] ([general], [skill_names], [related_skills], [mode]) VALUES (N'lord_liubei', N'zhangwu,shouyue,jizhao', N'rende', N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'bianfuren', 0, N'奕世之雍裔', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'caiwenji', 0, N'异乡的孤女', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'caocao', 0, N'魏武帝', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'caohong', 0, N'魏之福将', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'caopi', 0, N'霸业的继承者', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'caoren', 0, N'大将军', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'chenwudongxi', 0, N'壮怀激烈', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'daqiao', 0, N'矜持之花', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'dengai', 0, N'矫然的壮士', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'dianwei', 0, N'古之恶来', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'diaochan', 0, N'绝世的舞姬', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'dingfeng', 0, N'清侧重臣', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'dongzhuo', 0, N'魔王', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'erzhang', 0, N'经天纬地', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'ganfuren', 0, N'昭烈皇后', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'ganning', 0, N'锦帆游侠', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'guanyu', 0, N'美髯公', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'guojia', 0, N'早终的先知', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'hetaihou', 0, N'弄权之蛇蝎', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'huanggai', 0, N'轻身为国', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'huangyueying', 0, N'归隐的杰女', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'huangzhong', 0, N'老当益壮', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'huatuo', 0, N'神医', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'jiangqin', 0, N'祁奚之器', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'jiangwanfeiyi', 0, N'社稷股肱', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'jiangwei', 0, N'龙的衣钵', 0, NULL, NULL, N'木美人', N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'jiaxu', 0, N'冷酷的毒士', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'jiling', 0, N'仲家的主将', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'kongrong', 0, N'凛然重义', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lidian', 0, N'深明大义', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'liguo', 0, N'飞熊狂豹', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lingtong', 0, N'豪情烈胆', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'liubei', 0, N'乱世的枭雄', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'liushan', 0, N'无为的真命主', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lord_liubei', 0, N'龙横蜀汉', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lord_sunquan', 0, N'东吴大帝', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lord_zhangjiao', 0, N'时代的先驱', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lusu', 0, N'独断的外交家', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'luxun', 0, N'擎天之柱', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lvbu', 0, N'武的化身', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lvfan', 0, N'忠篆亮直', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'lvmeng', 0, N'白衣渡江', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'machao', 0, N'一骑当千', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'madai', 0, N'临危受命', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'masu', 0, N'兵法在胸', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'mateng', 0, N'驰骋西陲', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'menghuo', 0, N'南蛮王', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'mifuren', 0, N'乱世沉香', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'panfeng', 0, N'联军上将', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'pangde', 0, N'人马一体', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'pangtong', 0, N'凤雏', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'shamoke', 0, N'五溪蛮王', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'simayi', 0, N'狼顾之鬼', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'soldier_f', 0, N'无', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'soldier_m', 0, N'无', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'sunce', 0, N'江东的小霸王', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'sunjian', 0, N'武烈帝', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'sunquan', 0, N'年轻的贤君', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'sunshangxiang', 0, N'弓腰姬', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'taishici', 0, N'笃烈之士', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'tianfeng', 0, N'河北瑰杰', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'weiyan', 0, N'嗜血的独狼', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'wolong', 0, N'卧龙', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xiahoudun', 0, N'独眼的罗刹', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xiahouyuan', 0, N'疾行的猎豹', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xiaoqiao', 0, N'矫情之花', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xuchu', 0, N'虎痴', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xuhuang', 0, N'周亚夫之风', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xunyou', 0, N'曹魏的谋主', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xunyu', 0, N'王佐之才', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'xusheng', 0, N'江东的铁壁', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'yanliangwenchou', 0, N'虎狼兄弟', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'yuanshao', 0, N'高贵的名门', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'yuejin', 0, N'奋强突固', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'yuji', 0, N'魂绕左右', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zangba', 0, N'节度青徐', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhangfei', 0, N'万夫不当', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhanghe', 0, N'料敌机先', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhangjiao', 0, N'天公将军', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhangliao', 0, N'前将军', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhangren', 0, N'索命神射', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhaoyun', 0, N'少年将军', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhenji', 0, N'薄幸的美人', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhoutai', 0, N'历战之驱', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhouyu', 0, N'大都督', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhugeliang', 0, N'迟暮的丞相', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhurong', 0, N'野性的女王', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zoushi', 0, N'惑心之魅', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zuoci', 0, N'谜之仙人', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_skin] ([general_name], [skin_id], [title], [is_animation], [quality], [CV], [illustrator], [mode]) VALUES (N'zhouyu', 1, N'雄姿英发', 0, NULL, NULL, NULL, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhenji', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'guojia', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'simayi', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'caopi', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'caocao', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lidian', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'dengai', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhangliao', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhanghe', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xuhuang', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xunyu', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xiahouyuan', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'dianwei', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xuchu', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'yuejin', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xiahoudun', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zangba', 2, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'conghong', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'caoren', 2, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'huangyueying', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhugeliang', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lord_liubei', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'guanyu', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhangfei', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'pangtong', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'jiangwei', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'wolong', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'huangzhong', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'jiangwanfeiyi', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhurong', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'madai', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'machao', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'liushan', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'mifuren', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhaoyun', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'weiyan', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'liubei', 3, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'menghuo', 2, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'ganfuren', 2, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'sunshangxiang', 10, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lusu', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'sunquan', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhoutai', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'luxun', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'erzhang', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'huanggai', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'taishici', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'ganning', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'daqiao', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhouyu', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'jiangqin', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'chenwudongxi', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xusheng', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'sunce', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'dingfeng', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'sunjian', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'xiaoqiao', 4, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lvmeng', 3, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'jiaxu', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'diaochan', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'yuji', 9, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'kongrong', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'mateng', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lord_zhangjiao', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'lvbu', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'yuanshao', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'hetaihou', 8, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'yanliangwenchou', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'caiwenji', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'huatuo', 7, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhangjiao', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'dongzhuo', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zhangren', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'tianfeng', 6, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'jiling', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'pangde', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'panfeng', 5, N'Hegemony')
INSERT [dbo].[general_value] ([general], [value], [mode]) VALUES (N'zoushi', 3, N'Hegemony')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'bianfuren', 0, N'wei', 3, 0, N'Transformation', 0, 0, 0, N'caocao', N'卞夫人')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'caiwenji', 0, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'蔡文姬')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'caocao', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, N'dianwei,xuchu', N'曹操')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'caohong', 1, N'wei', 4, 0, N'Formation', 0, 0, 0, N'caoren', N'曹洪')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'caopi', 1, N'wei', 3, 0, N'Stander', 0, 0, 0, N'zhenji', N'曹丕')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'caoren', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'曹仁')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'chenwudongxi', 1, N'wu', 4, 0, N'Momentum', 0, 0, 0, NULL, N'董袭&陈武')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'daqiao', 0, N'wu', 3, 0, N'Stander', 0, 0, 0, N'xiaoqiao', N'大乔')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'dengai', 1, N'wei', 4, 1, N'Formation', 0, 0, 0, NULL, N'邓艾')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'dianwei', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'典韦')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'diaochan', 0, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'貂蝉')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'dingfeng', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'丁奉')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'dongzhuo', 1, N'qun', 4, 0, N'Momentum', 0, 0, 0, NULL, N'董卓')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'erzhang', 1, N'wu', 3, 0, N'Stander', 0, 0, 0, NULL, N'张昭&张纮')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'ganfuren', 0, N'shu', 3, 0, N'Stander', 0, 0, 0, NULL, N'甘夫人')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'ganning', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'甘宁')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'guanyu', 1, N'shu', 5, 0, N'Stander', 0, 0, 0, NULL, N'关羽')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'guojia', 1, N'wei', 3, 0, N'Stander', 0, 0, 0, NULL, N'郭嘉')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'hetaihou', 0, N'qun', 3, 0, N'Formation', 0, 0, 0, NULL, N'何太后')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'huanggai', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'黄盖')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'huangyueying', 0, N'shu', 3, 0, N'Stander', 0, 0, 0, NULL, N'黄月英')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'huangzhong', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, N'weiyan', N'黄忠')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'huatuo', 1, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'华佗')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'jiangqin', 1, N'wu', 4, 0, N'Formation', 0, 0, 0, N'zhoutai', N'蒋钦')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'jiangwanfeiyi', 1, N'shu', 3, 0, N'Formation', 0, 0, 0, NULL, N'蒋琬&费祎')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'jiangwei', 1, N'shu', 4, 0, N'Formation', 0, 0, 0, NULL, N'姜维')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'jiaxu', 1, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'贾诩')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'jiling', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, NULL, N'纪灵')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'kongrong', 1, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'孔融')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lidian', 1, N'wei', 3, 0, N'Momentum', 0, 0, 0, N'yuejin', N'李典')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'liguo', 1, N'qun', 4, 0, N'Transformation', 0, 0, 0, N'jiaxu', N'李傕＆郭汜')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lingtong', 1, N'wu', 4, 0, N'Transformation', 0, 0, 0, N'ganning', N'凌统')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'liubei', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, N'guanyu,zhangfei,ganfuren', N'刘备')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'liushan', 1, N'shu', 3, 0, N'Stander', 0, 0, 0, NULL, N'刘禅')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lord_liubei', 1, N'shu', 4, 0, N'Formation', 1, 0, 1, NULL, N'君·刘备')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lord_sunquan', 1, N'wu', 4, 0, N'Transformation', 1, 0, 1, NULL, N'君·孙权')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lord_zhangjiao', 1, N'qun', 4, 0, N'Momentum', 1, 0, 1, NULL, N'君·张角')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lusu', 1, N'wu', 3, 0, N'Stander', 0, 0, 0, NULL, N'鲁肃')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'luxun', 1, N'wu', 3, 0, N'Stander', 0, 0, 0, NULL, N'陆逊')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lvbu', 1, N'qun', 5, 0, N'Stander', 0, 0, 0, N'diaochan', N'吕布')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lvfan', 1, N'wu', 3, 0, N'Transformation', 0, 0, 0, NULL, N'吕范')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'lvmeng', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'吕蒙')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'machao', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, NULL, N'马超')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'madai', 1, N'shu', 4, 0, N'Momentum', 0, 0, 0, N'machao', N'马岱')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'masu', 1, N'shu', 3, 0, N'Transformation', 0, 0, 0, NULL, N'马谡')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'mateng', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, NULL, N'马腾')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'menghuo', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, N'zhurong', N'孟获')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'mifuren', 0, N'shu', 3, 0, N'Momentum', 0, 0, 0, NULL, N'糜夫人')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'panfeng', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, NULL, N'潘凤')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'pangde', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, NULL, N'庞德')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'pangtong', 1, N'shu', 3, 0, N'Stander', 0, 0, 0, NULL, N'庞统')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'shamoke', 1, N'shu', 4, 0, N'Transformation', 0, 0, 0, NULL, N'沙摩柯')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'simayi', 1, N'wei', 3, 0, N'Stander', 0, 0, 0, NULL, N'司马懿')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'soldier_f', 0, N'god', 5, 0, N'Momentum', 0, 0, 1, NULL, N'女士兵')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'soldier_m', 1, N'god', 5, 0, N'Momentum', 0, 0, 1, NULL, N'男士兵')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'sunce', 1, N'wu', 4, -1, N'Momentum', 0, 0, 0, N'zhouyu,taishici,daqiao', N'孙策')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'sunjian', 1, N'wu', 5, 0, N'Stander', 0, 0, 0, NULL, N'孙坚')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'sunquan', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, N'zhoutai', N'孙权')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'sunshangxiang', 0, N'wu', 3, 0, N'Stander', 0, 0, 0, NULL, N'孙尚香')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'taishici', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'太史慈')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'tianfeng', 1, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'田丰')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'weiyan', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, NULL, N'魏延')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'wolong', 1, N'shu', 3, 0, N'Stander', 0, 0, 0, N'huangyueying,pangtong', N'卧龙诸葛亮')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xiahoudun', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, N'xiahouyuan', N'夏侯敦')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xiahouyuan', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'夏侯渊')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xiaoqiao', 0, N'wu', 3, 0, N'Stander', 0, 0, 0, NULL, N'小乔')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xuchu', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'许褚')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xuhuang', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'徐晃')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xunyou', 1, N'wei', 3, 0, N'Transformation', 0, 0, 0, N'xunyu', N'荀攸')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xunyu', 1, N'wei', 3, 0, N'Stander', 0, 0, 0, NULL, N'荀彧')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'xusheng', 1, N'wu', 4, 0, N'Formation', 0, 0, 0, N'dingfeng', N'徐盛')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'yanliangwenchou', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, NULL, N'颜良&文丑')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'yuanshao', 1, N'qun', 4, 0, N'Stander', 0, 0, 0, N'yanliangwenchou', N'袁绍')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'yuejin', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'乐进')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'yuji', 1, N'qun', 3, 0, N'Formation', 0, 0, 0, NULL, N'于吉')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zangba', 1, N'wei', 4, 0, N'Momentum', 0, 0, 0, N'zhangliao', N'臧霸')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhangfei', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, NULL, N'张飞')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhanghe', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'张郃')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhangjiao', 1, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'张角')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhangliao', 1, N'wei', 4, 0, N'Stander', 0, 0, 0, NULL, N'张辽')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhangren', 1, N'qun', 4, 0, N'Momentum', 0, 0, 0, NULL, N'张任')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhaoyun', 1, N'shu', 4, 0, N'Stander', 0, 0, 0, N'liushan', N'赵云')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhenji', 0, N'wei', 3, 0, N'Stander', 0, 0, 0, NULL, N'甄姬')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhoutai', 1, N'wu', 4, 0, N'Stander', 0, 0, 0, NULL, N'周泰')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhouyu', 1, N'wu', 3, 0, N'Stander', 0, 0, 0, N'huanggai,xiaoqiao', N'周瑜')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhugeliang', 1, N'shu', 3, 0, N'Stander', 0, 0, 0, N'huangyueying', N'诸葛亮')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zhurong', 0, N'shu', 4, 0, N'Stander', 0, 0, 0, NULL, N'祝融')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zoushi', 0, N'qun', 3, 0, N'Stander', 0, 0, 0, NULL, N'邹氏')
INSERT [dbo].[generals] ([general_name], [sex], [kingdom], [HP], [adjust_hp], [package], [hegemony_lord], [classic_lord], [hidden], [companion], [translation]) VALUES (N'zuoci', 1, N'qun', 3, 0, N'Transformation', 0, 0, 0, N'yuji', N'左慈')
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'zhangfei', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'zhanghe', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'luxun', 26, 25)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'guojia', 25, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'xuhuang', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'simayi', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guojia', N'zhanghe', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'caopi', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'xunyu', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'xuchu', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'xiahouyuan', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guojia', N'xiahouyuan', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'xunyu', N'caopi', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'simayi', N'guojia', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caopi', N'zhanghe', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'zhangfei', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'zhangliao', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'dianwei', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'zhenghe', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'xiahouyuan', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'xunyu', N'dianwei', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guojia', N'zhangliao', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'simayi', N'xunyu', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guojia', N'xunyu', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caopi', N'dianwei', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'xiahoudun', N'xiahouyuan', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'yuejin', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'xuhuang', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'lidian', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'dianwei', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caocao', N'xuchu', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caoren', N'simayi', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caoren', N'yuejin', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangliao', N'yuejin', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'caopi', N'xiahouyuan', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'zhugeliang', 25, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'pangtong', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'wolong', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'guanyu', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangfei', N'guanyu', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guanyu', N'zhugeliang', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'pangtong', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'wolong', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'wolong', N'pangtong', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhurong', N'huangyueying', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'weiyan', N'huangzhong', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'weiyan', N'machao', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhurong', N'huangzhong', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhurong', N'machao', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guanyu', N'wolong', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huangyueying', N'liubei', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'zhangfei', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangfei', N'zhaoyun', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangfei', N'huangzhong', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'liubei', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhaoyun', N'huangzhong', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhaoyun', N'machao', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guanyu', N'huangzhong', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guanyu', N'machao', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'machao', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'huangzhong', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhugeliang', N'liushan', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhurong', N'liubei', 21, 20)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhaoyun', N'weiyan', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'ganfuren', N'zhugeliang', 21, 20)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'weiyan', N'menghuo', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lusu', N'zhoutai', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'huanggai', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'ganning', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'sunquan', 24, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvmeng', N'xiaoqiao', 24, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'daqiao', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunquan', N'huanggai', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhouyu', N'zhoutai', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lusu', N'daqiao', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lusu', N'xiaoqiao', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunquan', N'lvmeng', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huanggai', N'taishici', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'zhoutai', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunquan', N'zhoutai', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'lusu', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'zhouyu', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunshangxiang', N'dingfeng', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunjian', N'zhoutai', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lusu', N'luxun', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'luxun', N'erzhang', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'erzhang', N'daqiao', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhouyu', N'daqiao', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhouyu', N'xiaoqiao', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'huanggai', N'ganning', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'erzhang', N'lvmeng', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'sunjian', N'lvmeng', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'taishici', N'dingfeng', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'luxun', N'zhouyu', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'luxun', N'huanggai', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvmeng', N'zhoutai', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'yuanshao', N'tianfeng', 25, 25)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'diaochan', N'jiaxu', 23, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'yuanshao', N'mateng', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'yuanshao', N'jiaxu', 24, 24)
GO
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvbu', N'yanliangwenchou', 24, 24)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'jiaxu', N'huatuo', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'diaochan', N'caiwenji', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'jiaxu', N'caiwenji', 23, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'diaochan', N'kongrong', 22, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'diaochan', N'huatuo', 22, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvbu', N'jiaxu', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'jiaxu', N'zhangjiao', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'yuanshao', N'yanliangwenchou', 23, 23)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'diaochan', N'zhangjiao', 21, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'mateng', N'yanliangwenchou', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'tianfeng', N'huatuo', 22, 22)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'kongrong', N'caiwenji', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'yuanshao', N'zoushi', 22, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'jiaxu', N'mateng', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvbu', N'zhangjiao', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'lvbu', N'mateng', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'jiaxu', N'yanliangwenchou', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'kongrong', N'huatuo', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'tianfeng', N'caiwenji', 21, 20)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'panfeng', N'pangde', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'panfeng', N'jiling', 21, 21)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangliao', N'zhanghe', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangliao', N'xuchu', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangliao', N'xiahouyuan', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhangliao', N'lidian                          ', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'zhenji', N'caoren', 5, 4)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'xunyu', N'caoren', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'guanyu', N'zhaoyun', 5, 5)
INSERT [dbo].[pair_value] ([general1], [general2], [value1], [value2]) VALUES (N'machao', N'huangzhong', 5, 5)
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (1, N'jianxiong', 1, 0, 0, N'奸雄', N'每当你受到伤害后，你可以获得造成此伤害的牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (2, N'fankui', 1, 0, 0, N'反馈', N'每当你受到伤害后，你可以获得来源的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (3, N'guicai', 1, 0, 0, N'鬼才', N'每当一名角色的判定牌生效前，你可以打出手牌代替之。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (4, N'ganglie', 1, 0, 0, N'刚烈', N'每当你受到伤害后，你可以判定，若结果不为红桃，来源选择一项：\n1.弃置两张手牌；\n2.受到你造成的1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (5, N'tuxi', 1, 0, 0, N'突袭', N'摸牌阶段开始时，你可以放弃摸牌并选择一至两名有手牌的其他角色，\n获得这些角色的各一张手牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (6, N'luoyi', 1, 0, 0, N'裸衣', N'摸牌阶段，你可以少摸一张牌，若如此做，\n每当你于此回合内使用【杀】或【决斗】对目标角色造成伤害时，此伤害+1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (7, N'tiandu', 1, 0, 0, N'天妒', N'每当你的判定牌生效后，你可以获得之。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (8, N'yiji', 1, 0, 0, N'遗计', N'每当你受到1点伤害后，你可以观看牌堆顶的两张牌，\n然后将其中的一张牌交给一名角色，将另一张牌交给一名角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (9, N'luoshen', 0, 0, 0, N'洛神', N'准备阶段开始时，你可以判定，若结果为黑色，你可以重复此流程。\n最后你获得所有的黑色判定牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (10, N'qingguo', 0, 0, 0, N'倾国', N'你可以将一张黑色手牌当【闪】使用或打出。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (11, N'shensu', 1, 0, 0, N'神速', N'你可以跳过判定阶段和摸牌阶段，视为使用【杀】；\n你可以跳过出牌阶段并弃置一张装备牌，视为使用【杀】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (12, N'qiaobian', 1, 0, 0, N'巧变', N'你可以弃置一张手牌，跳过一个阶段（准备阶段和结束阶段除外），然后若你以此法：\n跳过摸牌阶段，你可以选择有手牌的一至两名其他角色，然后获得这些角色的各一张手牌；\n跳过出牌阶段，你可以将一名角色判定区/装备区里的一张牌置入另一名角色的判定区/装备区。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (13, N'duanliang', 0, 0, 0, N'断粮', N'你可以将一张不为锦囊牌的黑色牌当【兵粮寸断】使用；\n你能对距离为2的角色使用【兵粮寸断】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (14, N'jushou', 1, 0, 0, N'据守', N'结束阶段开始时，你可以摸X张牌，然后选择一项：\n1.弃置一张不为装备牌的手牌；\n2.使用一张装备牌。\n若X大于2，则你将武将牌翻面。\n（X为此时亮明势力数）')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (15, N'qiangxi', 0, 0, 0, N'强袭', N'出牌阶段限一次，你可以失去1点体力或弃置一张武器牌，并选择你攻击范围内的一名角色，对其造成1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (16, N'quhu', 0, 0, 0, N'驱虎', N'出牌阶段限一次，你可以与一名体力值大于你的角色拼点：\n当你赢后，其对其攻击范围内你选择的一名角色造成1点伤害；\n当你没赢后，其对你造成1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (17, N'jieming', 1, 0, 0, N'节命', N'每当你受到1点伤害后，你可以令一名角色将手牌补至X张（X为其体力上限且至多为5）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (18, N'xingshang', 1, 0, 0, N'行殇', N'每当其他角色死亡时，你可以获得其所有牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (19, N'fangzhu', 1, 0, 0, N'放逐', N'每当你受到伤害后，你可以令一名其他角色摸X张牌（X为你已损失的体力值），然后其叠置。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (20, N'xiaoguo', 1, 0, 0, N'骁果', N'其他角色的结束阶段开始时，你可以弃置一张基本牌，令其选择一项：\n1.弃置一张装备牌；2.受到你造成的1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (21, N'qingnang', 0, 0, 0, N'青囊', N'出牌阶段限一次，你可以弃置一张手牌并选择一名已受伤的角色，令其回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (22, N'jijiu', 0, 0, 0, N'急救', N'你于回合外可以将一张红色牌当【桃】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (23, N'wushuang', 1, 0, 0, N'无双', N'锁定技，每当你使用【杀】指定一个目标后，你将其抵消此【杀】的方式改为依次使用两张【闪】；\n锁定技，每当你使用【决斗】指定一个目标后，或成为一名角色使用【决斗】的目标后，你将其执行此【决斗】中打出【杀】的效果改为依次打出两张【杀】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (24, N'lijian', 0, 0, 0, N'离间', N'出牌阶段限一次，你可以弃置一张牌并选择两名其他男性角色，令其中的一名男性角色视为对另一名男性角色使用【决斗】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (25, N'biyue', 1, 0, 0, N'闭月', N'结束阶段开始时，你可以摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (26, N'luanji', 0, 0, 0, N'乱击', N'你可以将两张手牌当【万箭齐发】使用（不能使用本回合此前发动此技能时已用过的花色） 。\n若如此做，当响应此【万箭齐发】而打出的【闪】结算结束时，\n若打出闪的角色与你势力相同，其可以摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (27, N'shuangxiong', 1, 0, 0, N'双雄', N'摸牌阶段开始时，你可以放弃摸牌，判定，当判定牌生效后，你获得之，若如此做，你于此回合内可以将一张与之颜色不同的手牌当【决斗】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (28, N'wansha', 1, 1, 0, N'完杀', N'锁定技，不处于濒死状态的其他角色于你的回合内不能使用【桃】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (29, N'weimu', 1, 1, 0, N'帷幕', N'锁定技，每当你成为黑色锦囊牌的目标时，你取消自己。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (30, N'luanwu', 0, 2, 0, N'乱武', N'限定技，出牌阶段，你可以选择所有其他角色，\n这些角色各需对距离最小的另一名角色使用【杀】，否则失去1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (31, N'mashu_pangde', 0, 1, 0, N'马术', N'锁定技，你与其他角色的距离-1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (32, N'mengjin', 1, 0, 0, N'猛进', N'每当你使用的【杀】被目标角色使用的【闪】抵消时，你可以弃置其一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (33, N'leiji', 1, 0, 0, N'雷击', N'每当你使用或打出【闪】时，你可以令一名角色判定，、\n若结果为黑桃，你对其造成2点雷电伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (34, N'guidao', 1, 0, 0, N'鬼道', N'每当一名角色的判定牌生效前，你可以打出黑色牌替换之。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (35, N'beige', 1, 0, 0, N'悲歌', N'每当一名角色受到【杀】造成的伤害后，若其存活，\n你可以弃置一张牌，令其判定，若结果为：\n红桃，其回复1点体力；\n方块，其摸两张牌；\n梅花，来源弃置两张牌；\n黑桃，来源叠置。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (36, N'duanchang', 0, 1, 0, N'断肠', N'锁定技，当你死亡时，你令杀死你的角色失去你选择的其一张武将牌的技能。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (37, N'mashu_mateng', 0, 1, 0, N'马术', N'锁定技，你与其他角色的距离-1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (38, N'xiongyi', 0, 2, 0, N'雄异', N'限定技，出牌阶段，你可以令与你势力相同的所有角色各摸三张牌，\n然后若你的势力是角色最少的势力，你回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (39, N'mingshi', 1, 1, 0, N'名士', N'锁定技，每当你受到伤害时，若来源有暗置的武将牌，你令此伤害-1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (40, N'lirang', 1, 0, 0, N'礼让', N'每当你的一张被弃置的牌置入弃牌堆后，你可以将之交给一名其他角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (41, N'shuangren', 1, 0, 0, N'双刃', N'出牌阶段开始时，你可以与一名角色拼点：当你赢后，\n你视为对其或一名与其势力相同的其他角色使用【杀】；\n当你没赢后，你结束出牌阶段。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (42, N'sijian', 1, 0, 0, N'死谏', N'每当你失去所有手牌后，你可以弃置一名其他角色的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (43, N'suishi', 1, 1, 0, N'随势', N'锁定技，每当其他角色因受到伤害而进入濒死状态时，\n若来源与你势力相同，你摸一张牌；\n锁定技，每当其他与你势力相同的角色死亡时，你失去1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (44, N'kuangfu', 1, 0, 0, N'狂斧', N'每当你使用【杀】对目标角色造成伤害后，你可以选择一项：\n1.将其装备区里的一张牌置入你的装备区；\n2.弃置其装备区里的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (45, N'huoshui', 0, 0, 0, N'祸水', N'出牌阶段，你可以明置此武将牌；其他角色于你的回合内不能明置其武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (46, N'qingcheng', 0, 0, 0, N'倾城', N'出牌阶段，你可以弃置一张黑色牌并选择一名武将牌均明置的其他角色，\n然后你暗置其一张武将牌。\n若你以此法弃置的牌是黑色装备牌，则你可以再选择另一名武将牌均明置的其他角色，暗置其一张武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (47, N'rende', 0, 0, 0, N'仁德', N'出牌阶段，你可以将任意张手牌交给一名本阶段未获得过“仁德”牌的其他角色。\n当你于本阶段给出第二张“仁德”牌时，你可以视为使用一张基本牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (48, N'wusheng', 0, 0, 0, N'武圣', N'你可以将一张红色牌当【杀】使用或打出。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (49, N'paoxiao', 1, 1, 0, N'咆哮', N'锁定技，你使用【杀】无次数限制。\n锁定技，当你于当前回合使用第二张【杀】时，你摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (50, N'guanxing', 0, 0, 0, N'观星', N'准备阶段开始时，你可以观看牌堆顶的X张牌（X为全场角色数且至多为5）并改变其中任意数量的牌的顺序并将其余的牌置于牌堆底。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (51, N'kongcheng', 1, 1, 0, N'空城', N'锁定技，每当你成为【杀】或【决斗】的目标时，若你没有手牌，你取消自己。\n锁定技，当你于回合外因为其他角色交给你牌而得到牌时，\n你将这些牌置于你的武将牌上。\n锁定技，摸牌阶段开始时，你获得武将牌上的所有牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (52, N'longdan', 0, 0, 0, N'龙胆', N'你可以将【杀】当【闪】、【闪】当【杀】使用或打出。\n当你通过发动【龙胆】使用的【杀】被一名角色使用的【闪】抵消时，\n你可以对另一名角色造成1点普通伤害。\n当一名角色使用的【杀】被你通过发动【龙胆】使用的【闪】抵消时，\n你可以令另一名其他角色回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (53, N'mashu_machao', 0, 1, 0, N'马术', N'锁定技，你与其他角色的距离-1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (54, N'tieqi', 1, 0, 0, N'铁骑', N'当你使用【杀】指定一个目标后，你可以进行判定。\n然后你选择其一张明置的武将牌，令此武将牌上的所有非锁定技于此回合内失效。\n除非该角色弃置与结果花色相同的一张牌，否则不能使用【闪】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (55, N'jizhi', 1, 0, 0, N'集智', N'每当你使用非转化的非延时类锦囊牌时，你可以摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (56, N'qicai', 0, 1, 0, N'奇才', N'锁定技，你使用锦囊牌无距离限制。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (57, N'liegong', 1, 0, 0, N'烈弓', N'每当你于出牌阶段内使用【杀】指定一个目标后，\n若其手牌数不小于你的体力值或不大于你的攻击范围，\n你可以令其不能使用【闪】响应此【杀】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (58, N'kuanggu', 1, 1, 0, N'狂骨', N'锁定技，每当你对一名角色造成1点伤害后，\n若你与其的距离于其因受到此伤害而扣减体力前不大于1，你回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (59, N'lianhuan', 0, 0, 0, N'连环', N'你可以将一张梅花手牌当【铁索连环】使用；你能重铸梅花手牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (60, N'niepan', 1, 2, 0, N'涅槃', N'限定技，当你处于濒死状态时，你可以弃置你区域里的所有牌，\n然后恢复至平置状态并重置，摸三张牌，将体力值回复至3点。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (61, N'bazhen', 1, 1, 0, N'八阵', N'锁定技，若你的装备区里没有防具牌，你视为装备着【八卦阵】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (62, N'huoji', 0, 0, 0, N'火计', N'你可以将一张红色手牌当【火攻】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (63, N'kanpo', 0, 0, 0, N'看破', N'你可以将一张黑色手牌当【无懈可击】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (64, N'xiangle', 1, 1, 0, N'享乐', N'锁定技，每当你成为其他角色使用【杀】的目标时，\n你令其选择是否弃置一张基本牌，\n若其选择否或其已死亡，此次对你结算的此【杀】对你无效。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (65, N'fangquan', 1, 0, 0, N'放权', N'你可以跳过出牌阶段，若如此做，此回合结束时，\n你可以弃置一张手牌并选择一名其他角色，若如此做，其获得一个额外的回合。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (66, N'huoshou', 1, 1, 0, N'祸首', N'锁定技，【南蛮入侵】对你无效；\n锁定技，每当其他角色使用【南蛮入侵】指定目标后，\n你代替其成为此【南蛮入侵】造成的伤害的来源。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (67, N'zaiqi', 1, 0, 0, N'再起', N'摸牌阶段开始时，若你已受伤，你可以放弃摸牌，\n亮出牌堆顶的X张牌（X为你已损失的体力值），然后回复等同于其中红桃牌数的体力，\n将这些红桃牌置入弃牌堆，获得其余的牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (68, N'juxiang', 1, 1, 0, N'巨象', N'锁定技，【南蛮入侵】对你无效；\n锁定技，每当其他角色使用的【南蛮入侵】因结算完毕而置入弃牌堆后，你获得之。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (69, N'lieren', 1, 0, 0, N'烈刃', N'每当你使用【杀】对目标角色造成伤害后，你可以与其拼点，当你赢后，你获得其一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (70, N'shushen', 1, 0, 0, N'淑慎', N'每当你回复1点体力后，你可以令一名其他角色摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (71, N'shenzhi', 0, 0, 0, N'神智', N'准备阶段开始时，你可以弃置所有手牌，\n然后若你以此法弃置的手牌数不小于X（X为你的体力值），你回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (72, N'zhiheng', 0, 0, 0, N'制衡', N'出牌阶段限一次，你可以弃置一至X张牌（X为你的体力上限），摸等量的牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (73, N'qixi', 0, 0, 0, N'奇袭', N'你可以将一张黑色牌当【过河拆桥】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (74, N'keji', 1, 1, 0, N'克己', N'锁定技，弃牌阶段开始时，\n若你未于出牌阶段内使用过颜色不同的牌或出牌阶段被跳过，你的手牌上限于此回合内+4。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (75, N'kurou', 0, 0, 0, N'苦肉', N'出牌阶段限一次，你可以弃一张牌并失去1点体力，\n然后摸三张牌，此阶段你使用【杀】的次数上限+1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (76, N'yingzi_zhouyu', 1, 1, 0, N'英姿', N'锁定技，摸牌阶段，你多摸一张牌；\n你的手牌上限等于X（X为你的体力上限）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (77, N'fanjian', 0, 0, 0, N'反间', N'出牌阶段限一次，你可以展示一张手牌并将之交给一名其他角色，\n该角色选择一项：\n1.展示所有手牌，然后弃置与此牌花色相同的所有牌；\n2.失去1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (78, N'guose', 0, 0, 0, N'国色', N'你可以将一张方块牌当【乐不思蜀】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (79, N'liuli', 1, 0, 0, N'流离', N'每当你成为【杀】的目标时，你可以弃置一张牌并选择你攻击范围内的一名角色，将此【杀】转移给该角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (80, N'qianxun', 1, 1, 0, N'谦逊', N'锁定技，每当你成为【顺手牵羊】或【乐不思蜀】的目标时，你取消自己。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (81, N'duoshi', 0, 0, 0, N'度势', N'你可以将一张红色手牌当【以逸待劳】使用。每阶段限四次。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (82, N'jieyin', 0, 0, 0, N'结姻', N'出牌阶段限一次，你可以弃置两张手牌并选择一名已受伤的其他男性角色，\n令你与其各回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (83, N'xiaoji', 1, 0, 0, N'枭姬', N'每当你失去装备区里的装备牌后，你可以摸两张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (84, N'yinghun_sunjian', 1, 0, 0, N'英魂', N'准备阶段开始时，若你已受伤，你可以选择一项：\n1.令一名其他角色摸X张牌，然后其弃置一张牌；\n2.令一名其他角色摸一张牌，然后其弃置X张牌。\n（X为你已损失的体力值）')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (85, N'hongyan', 1, 1, 0, N'红颜', N'锁定技，你的黑桃牌视为红桃牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (86, N'tianxiang', 1, 0, 0, N'天香', N'每当你受到伤害时，你可以弃置一张红桃手牌并选择一名其他角色，\n将此伤害转移给该角色，若如此做，当其因此而受到伤害进行的伤害结算结束时，\n其摸X张牌（X为其已损失的体力值）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (87, N'tianyi', 0, 0, 0, N'天义', N'出牌阶段限一次，你可以与一名角色拼点：\n当你赢后，你于此回合内使用【杀】的额外次数上限+1且使用【杀】无距离限制且使用【杀】的额外目标数上限+1；\n当你没赢后，你于此回合内不能使用【杀】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (88, N'buqu', 1, 1, 0, N'不屈', N'锁定技，当你处于濒死状态时，\n你将牌堆顶的一张牌置于你的武将牌上，称为"创"：\n若此牌点数与已有的"创"点数均不同，你将体力回复至1点；\n若点数相同，将此牌置入弃牌堆。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (89, N'haoshi', 1, 0, 0, N'好施', N'摸牌阶段，你可以多摸两张牌，然后若你的手牌数大于5，\n你将一半的手牌交给一名手牌最少的其他角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (90, N'dimeng', 0, 0, 0, N'缔盟', N'出牌阶段限一次，你可以选择两名其他角色并弃置X张牌（X为这两名角色手牌数的差），\n令这两名角色交换手牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (91, N'zhijian', 0, 0, 0, N'直谏', N'出牌阶段，你可以将手牌区里的一张装备牌置入一名其他角色的装备区，摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (92, N'guzheng', 1, 0, 0, N'固政', N'其他角色的弃牌阶段结束时，你可以将弃牌堆里的一张其于此阶段内因其弃置而失去过的手牌交给该角色，\n若如此做，你可以获得弃牌堆里的其余于此阶段内弃置的牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (93, N'duanbing', 1, 0, 0, N'短兵', N'你使用【杀】能额外选择一名距离为1的角色为目标。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (94, N'fenxun', 0, 0, 0, N'奋迅', N'出牌阶段限一次，你可以弃置一张牌并选择一名其他角色，\n令你与其的距离于此回合内视为1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (122, N'tuntian', 1, 0, 0, N'屯田', N'每当你于回合外失去牌后，你可以判定，\n当非红桃的判定牌生效后，你可以将之置于武将牌上，称为“田”；\n你与其他角色的距离-X（X为\"田\"数）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (123, N'jixi', 0, 0, 0, N'急袭', N'主将技，此武将牌上单独的阴阳鱼个数-1；\n主将技，你可以将一张\"田\"当【顺手牵羊】使用。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (124, N'ziliang', 1, 0, 0, N'资粮', N'副将技，每当与你势力相同的一名角色受到伤害后，\n你可以将一张\"田\"交给该角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (125, N'huyuan', 1, 0, 0, N'护援', N'结束阶段开始时，你可以将一张装备牌置入一名角色的装备区，\n若如此做，你可以弃置其距离为1的一名角色的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (126, N'heyi', 0, 0, 0, N'鹤翼', N'阵法技，与你处于同一队列的其他角色视为拥有\"飞影\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (127, N'feiying', 0, 0, 0, N'飞影', N'锁定技，其他角色与你的距离+1。')
GO
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (128, N'tiaoxin', 0, 0, 0, N'挑衅', N'出牌阶段限一次，你可以令攻击范围内含有你的一名角色选择是否对你使用【杀】，\n若其选择否，你弃置其一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (129, N'yizhi', 0, 1, 0, N'遗志', N'副将技，此武将牌上单独的阴阳鱼个数-1；\n副将技，若你的主将有\"观星\"，此\"观星\"描述中的X视为5，\n否则你视为拥有\"观星\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (130, N'tianfu', 0, 1, 0, N'天覆', N'主将技，阵法技，若当前回合角色为你所在队列里的角色，你视为拥有\"看破\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (131, N'shengxi', 1, 0, 0, N'生息', N'出牌阶段结束时，若你未于此阶段内造成过伤害，你可以摸两张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (132, N'shoucheng', 1, 0, 0, N'守成', N'每当与你势力相同的一名角色于其回合外失去所有手牌后，你可以令其摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (133, N'shangyi', 0, 0, 0, N'尚义', N'出牌阶段限一次，你可以令一名其他角色观看你的所有手牌，你选择一项：\n1.观看其手牌并可以弃置其中的一张黑色牌；\n2.观看其所有暗置的武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (134, N'niaoxiang', 0, 0, 0, N'鸟翔', N'阵法技，在你为围攻角色的围攻关系中，\n每当围攻角色使用【杀】指定一个被围攻的目标后，\n该围攻角色将该被围攻角色抵消此【杀】的方式改为依次使用两张【闪】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (135, N'yicheng', 1, 0, 0, N'疑城', N'每当与你势力相同的一名角色成为【杀】的目标后，\n你可以令其摸一张牌，然后其弃置一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (136, N'qianhuan', 1, 0, 0, N'千幻', N'每当与你势力相同的一名角色受到伤害后，若其存活，\n你可以将一张与你武将牌上花色均不相同的牌置于武将牌上，称为\"幻\"；\n每当与你势力相同的一名角色成为基本牌或锦囊牌的目标时，若目标数为1，\n你可以将一张\"幻\"置入弃牌堆，取消该角色。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (137, N'zhendu', 1, 0, 0, N'鸩毒', N'其他角色的出牌阶段开始时，你可以弃置一张手牌，\n令其视为以方法Ⅰ使用【酒】，然后你对其造成1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (138, N'qiluan', 1, 0, 0, N'戚乱', N'一名角色的回合结束后，若你于此回合内杀死过角色，你可以摸三张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (139, N'zhangwu', 1, 1, 0, N'章武', N'锁定技，每当【飞龙夺凤】置入弃牌堆或其他角色的装备区后，你获得之；\n锁定技，每当你失去【飞龙夺凤】前，你展示之，\n然后将此牌移动的目标区域改为牌堆底，\n若如此做，当此牌置于牌堆底后，你摸两张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (140, N'shouyue', 0, 1, 0, N'授钺', N'君主技，锁定技，你拥有\"五虎将大旗\"。\n\n#\"五虎将大旗\"\n存活的蜀势力角色拥有的下列五个技能分别调整为：\n武圣——你可以将一张牌当【杀】使用或打出。\n咆哮——增加描述：每当你使用【杀】指定一个目标后，你无视其防具。\n龙胆——增加描述：每当你发动【龙胆①】使用或打出牌时，你摸一张牌。\n烈弓——增加描述：你的攻击范围+1。\n铁骑——修改描述：将“一张”改为“所有”。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (141, N'jizhao', 1, 2, 0, N'激诏', N'限定技，当你处于濒死状态时，你可以将手牌补至X张（X为你的体力上限），\n然后将体力值回复至2点，失去\"授钺\"并获得\"仁德\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (95, N'chuli', 0, 0, 0, N'除疠', N'出牌阶段限一次，你可以选择至多三名势力各不相同或未确定势力的其他角色，\n然后你弃置你和这些角色的各一张牌。被弃置黑桃牌的角色各摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (96, N'mouduan', 1, 0, 0, N'谋断', N'结束阶段开始时，若你于出牌阶段内使用过四种花色或三种类别的牌，则你可以移动场上的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (97, N'jianchu', 1, 0, 0, N'鞬出', N'当你使用【杀】指定一个目标后，你可以弃置其一张牌，\n若弃置的牌：\n是装备牌，该角色不能使用【闪】；\n不是装备牌，该角色获得此【杀】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (98, N'xunxun', 1, 0, 0, N'恂恂', N'摸牌阶段开始时，你可以观看牌堆顶的四张牌，\n然后将其中的两张牌置于牌堆顶，将其余的牌置于牌堆底。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (99, N'wangxi', 1, 0, 0, N'忘隙', N'每当你对其他角色造成1点伤害后，或受到其他角色造成的1点伤害后，\n若其存活，你可以令你与其各摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (100, N'hengjiang', 1, 0, 0, N'横江', N'每当你受到1点伤害后，你可以令当前回合角色的手牌上限于此回合内-1，\n若你此前于此回合内未发动过\"横江\"，此回合结束时，\n若其未于弃牌阶段内弃置过其牌，你摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (101, N'mashu_madai', 0, 1, 0, N'马术', N'锁定技，你与其他角色的距离-1。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (102, N'qianxi', 0, 0, 0, N'潜袭', N'准备阶段开始时，你可以判定，\n然后令距离为1的一名角色于此回合内不能使用或打出与结果颜色相同的手牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (103, N'guixiu', 0, 0, 0, N'闺秀', N'每当你明置此武将牌后，你可以摸两张牌；\n当你移除此武将牌后，你可以回复1点体力。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (104, N'cunsi', 0, 0, 0, N'存嗣', N'出牌阶段，你可以移除此武将牌并选择一名角色，令其获得\"勇决\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (105, N'yongjue', 0, 0, 0, N'勇决', N'每当与你势力相同的一名角色于其出牌阶段内使用的【杀】因结算完毕而置入弃牌堆后，\n若此【杀】为其于此阶段内使用的首张牌，其可以获得之')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (106, N'jiang', 1, 0, 0, N'激昂', N'每当你使用【决斗】/红色【杀】指定目标后，\n或成为一张【决斗】/红色【杀】的目标后，你可以摸一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (107, N'yingyang', 1, 0, 0, N'鹰扬', N'每当你拼点的牌亮出后，你可以令此牌的点数于此次拼点中+3或-3。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (108, N'hunshang', 0, 1, 0, N'魂殇', N'副将技，此武将牌上单独的阴阳鱼个数-1；副将技，准备阶段开始时，若你的体力值为1，你于此回合内拥有\"英姿\"和\"英魂\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (109, N'yingzi_sunce', 1, 1, 0, N'英姿', N'锁定技，摸牌阶段，你多摸一张牌；\n你的手牌上限等于X（X为你的体力上限）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (110, N'yinghun_sunce', 0, 0, 0, N'英魂', N'准备阶段开始时，若你已受伤，你可以选择一项：\n1.令一名其他角色摸X张牌，然后其弃置一张牌；\n2.令一名其他角色摸一张牌，然后其弃置X张牌。\n（X为你已损失的体力值）')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (111, N'duanxie', 0, 0, 0, N'断绁', N'出牌阶段限一次，你可以令一名其他角色横置，\n若如此做，你横置。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (112, N'fenming', 1, 0, 0, N'奋命', N'结束阶段开始时，若你处于连环状态，\n你可以弃置处于连环状态的每名角色的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (113, N'hengzheng', 1, 0, 0, N'横征', N'摸牌阶段开始时，若你的体力值为1或你没有手牌，\n你可以放弃摸牌，获得每名其他角色区域里的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (114, N'baoling', 0, 1, 0, N'暴凌', N'主将技，锁定技，出牌阶段结束时，\n若此武将牌已明置且你有副将，你移除副将的武将牌，\n然后加3点体力上限，回复3点体力，获得\"崩坏\"。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (115, N'benghuai', 0, 1, 0, N'崩坏', N'锁定技，结束阶段开始时，若你不是体力值最小的角色，\n你选择一项：1.失去1点体力；2.减1点体力上限。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (116, N'chuanxin', 1, 0, 0, N'穿心', N'每当你于出牌阶段内使用【杀】或【决斗】对与你势力不同的目标角色造成伤害时，\n若其有副将，你可以防止此伤害，令其选择一项：\n.弃置装备区里的所有牌，若如此做，其失去1点体力；\n2.移除副将的武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (117, N'fengshi', 0, 1, 0, N'锋矢', N'阵法技，在你为围攻角色的围攻关系中，\n每当围攻角色使用【杀】指定一个被围攻的目标后，\n该围攻角色令该被围攻角色弃置装备区里的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (118, N'wuxin', 1, 0, 0, N'悟心', N'摸牌阶段开始时，你可以观看牌堆顶的X张牌（X为群势力角色数），\n然后你可以改变这些牌的顺序。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (119, N'wendao', 0, 0, 0, N'问道', N'出牌阶段限一次，你可以弃置一张红色牌，\n获得弃牌堆里或场上的一张【太平要术】。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (120, N'hongfa', 0, 1, 0, N'弘法', N'君主技，当此武将牌明置时，你获得\"黄巾天兵符\"；\n君主技，准备阶段开始时，若没有\"天兵\"，\n你将牌堆顶的X张牌置于\"黄巾天兵符\"上，称为\"天兵\"\n（X为群势力角色数）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (121, N'hongfaslash', 0, 0, 1, N'黄巾天兵符', N'你执行的效果中的\"群势力角色数\"+X（X为不大于\"天兵\"数的自然数）；\n每当你失去体力时，你可以将一张\"天兵\"置入弃牌堆，防止此失去体力；\n与你势力相同的角色可以将一张\"天兵\"当【杀】使用或打出。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (142, N'qice', 0, 0, 0, N'奇策', N'出牌阶段限一次，你可以将所有手牌当一张目标数不大于X的非延时类锦囊牌使用（X为你的手牌数），\n当此牌结算后，你可以变更副将的武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (143, N'zhiyu', 1, 0, 0, N'智愚', N'每当你受到伤害后，你可以摸一张牌：\n若如此做，你展示所有手牌。\n若你的手牌均为同一颜色，伤害来源弃置一张手牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (144, N'wanwei', 1, 0, 0, N'挽危', N'当你因被其他角色获得为手牌或弃置而失去牌时，\n你可以改为自己选择失去的牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (145, N'yuejian', 1, 0, 0, N'约俭', N'与你势力相同角色的弃牌阶段开始时，\n若其本回合未使牌指定过除该角色外的其他角色，\n你可令其本回合手牌上限等于其体力上限。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (146, N'xiongsuan', 0, 2, 0, N'凶算', N'限定技，出牌阶段，你可以弃置一张手牌，\n对与你势力相同的一名角色造成1点伤害，然后你摸三张牌。\n若该角色有已发动的限定技，你选择其一项限定技，\n此回合结束后，视为该技能未发动。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (147, N'huashen', 1, 0, 0, N'化身', N'这是煞笔设计出来的东西')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (148, N'xinsheng', 1, 0, 0, N'新生', N'当你受到伤害后，你可以从剩余武将牌中连续亮出武将牌，\n直到亮出一张与“化身”相同势力的武将牌为止，\n然后你可以将之与一张“化身”替换。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (149, N'jili', 1, 0, 0, N'蒺藜', N'当你一回合内使用或打出第X张牌时，\n你可以摸X张牌（X为你的攻击范围）。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (150, N'sanyao', 0, 0, 0, N'散谣', N'出牌阶段限一次，你可以弃置一张牌，\n然后对体力最多的一名角色造成1点伤害。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (151, N'zhiman', 1, 0, 0, N'制蛮', N'当你对其他角色造成伤害时，你可以防止此伤害，\n然后你获得其装备区或判定区的一张牌。\n若该角色与你势力相同，其可以变更副将的武将牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (152, N'xuanlue', 1, 0, 0, N'旋略', N'每当你失去装备区里的牌时，可以弃置一名其他角色一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (153, N'yongjin', 0, 2, 0, N'勇进', N'限定技，出牌阶段，你可以依次移动场上至多三张装备牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (154, N'diaodu', 0, 0, 0, N'调度', N'出牌阶段限一次，你可令所有与你相同势力的角色依次选择一项：\n使用手牌中的一张装备牌，\n或将装备区里的一张牌移动至另一名与你相同势力的角色的装备区。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (155, N'diancai', 1, 0, 0, N'典财', N'其他角色的出牌阶段结束时，若你本阶段失去了X张或更多的牌（X为你的体力值），\n你可以将手牌补至体力上限，然后你可以变更副将。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (156, N'jiahe', 0, 0, 0, N'嘉禾', N'君主技，若此武将牌处于明置状态，你便拥有“缘江烽火图”。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (157, N'lianzi', 0, 0, 0, N'敛资', N'出牌阶段限一次，你可以弃置一张手牌，\n然后亮出牌堆顶的X张牌（X为吴势力角色装备区里的牌和“烽火”之和），\n你获得所有与该牌相同类别的牌，然后将其余的牌置入弃牌堆。\n若你以此法获得四张或更多的牌，你失去此技能，并获得技能“制衡”。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (158, N'jubao', 0, 1, 0, N'聚宝', N'锁定技，你装备区里的宝物牌不能被其他角色获得；\n结束阶段开始时，若场上或弃牌堆有【定澜夜明珠】，你摸一张牌，\n然后获得装备区里有【定澜夜明珠】的角色的一张牌。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (159, N'flamemap', 0, 0, 1, N'缘江烽火图', N'每名吴势力角色的出牌阶段限一次，\n该角色可以将一张装备牌置于此牌上，称为“烽火”。\n当你受到【杀】或锦囊牌造成的伤害后，你将一张“烽火”置入弃牌堆。\n吴势力角色的准备阶段开始时，该角色可以根据“烽火”的数量获得以下一项技能，直到回合结束：\n至少一张，“英姿”；\n至少两张，“好施”；\n至少三张，“涉猎”；\n至少四张，“度势”；\n至少五张，可额外获得一项。')
INSERT [dbo].[skills] ([index], [skill_name], [preshow], [frequency], [attach_lord], [translation], [description]) VALUES (160, N'fenji', 1, 0, 0, N'奋激', N'一名角色的结束阶段，若其没有手牌，\n你可令其摸两张牌，然后你失去一点体力。')
INSERT [dbo].[title] ([title_id], [title_name], [describe], [translation]) VALUES (0, N'newbee', N'沙雕萌新', N'初出茅庐')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ConncetionError', N'连接失败', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Connecting', N'连接中', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Connected', N'已连接', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Asking', N'请求中', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Disconnected', N'连接中断', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LoginDuplicated', N'重复登录', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'NoAccount', N'账号不存在', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'PasswordWrong', N'密码错误', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'RegisterSuccesful', N'注册成功', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'AccountDuplicated', N'账号已注册', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'PasswordChangeSuccesful', N'修改成功', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'WrongMessage', N'错误的协议', N'Asset/Prefabs/LoginDialogScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Hegemony', N'国战', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Classic', N'身份', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Stander', N'标准', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Formation', N'阵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Momentum', N'势', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Transformation', N'变', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Authority', N'权', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GuanduLimited', N'官渡标准', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LordCards', N'君主卡牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'StrategicAdvantage', N'势备', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'bianfuren', N'卞夫人', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'caiwenji', N'蔡文姬', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'caocao', N'曹操', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'caohong', N'曹洪', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'caopi', N'曹丕', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'caoren', N'曹仁', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'chenwudongxi', N'董袭&陈武', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'daqiao', N'大乔', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dengai', N'邓艾', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dianwei', N'典韦', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'diaochan', N'貂蝉', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dingfeng', N'丁奉', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dongzhuo', N'董卓', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'erzhang', N'张昭&张纮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ganfuren', N'甘夫人', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ganning', N'甘宁', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guanyu', N'关羽', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guojia', N'郭嘉', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hetaihou', N'何太后', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huanggai', N'黄盖', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huangyueying', N'黄月英', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huangzhong', N'黄忠', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huatuo', N'华佗', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiangqin', N'蒋钦', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiangwanfeiyi', N'蒋琬&费祎', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiangwei', N'姜维', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiaxu', N'贾诩', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiling', N'纪灵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kongrong', N'孔融', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lidian', N'李典', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'liguo', N'李傕＆郭汜', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lingtong', N'凌统', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'liubei', N'刘备', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'liushan', N'刘禅', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lord_liubei', N'君·刘备', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lord_sunquan', N'君·孙权', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lord_zhangjiao', N'君·张角', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lusu', N'鲁肃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luxun', N'陆逊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lvbu', N'吕布', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lvfan', N'吕范', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lvmeng', N'吕蒙', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'machao', N'马超', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'madai', N'马岱', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'masu', N'马谡', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mateng', N'马腾', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'menghuo', N'孟获', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mifuren', N'糜夫人', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'panfeng', N'潘凤', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'pangde', N'庞德', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'pangtong', N'庞统', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shamoke', N'沙摩柯', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'simayi', N'司马懿', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'soldier_f', N'女士兵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'soldier_m', N'男士兵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sunce', N'孙策', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sunjian', N'孙坚', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sunquan', N'孙权', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sunshangxiang', N'孙尚香', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'taishici', N'太史慈', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tianfeng', N'田丰', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'weiyan', N'魏延', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wolong', N'卧龙诸葛亮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiahoudun', N'夏侯敦', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiahouyuan', N'夏侯渊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiaoqiao', N'小乔', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xuchu', N'许褚', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xuhuang', N'徐晃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xunyou', N'荀攸', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xunyu', N'荀彧', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xusheng', N'徐盛', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yanliangwenchou', N'颜良&文丑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yuanshao', N'袁绍', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yuejin', N'乐进', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yuji', N'于吉', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zangba', N'臧霸', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhangfei', N'张飞', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhanghe', N'张郃', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhangjiao', N'张角', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhangliao', N'张辽', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhangren', N'张任', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhaoyun', N'赵云', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhenji', N'甄姬', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhoutai', N'周泰', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhouyu', N'周瑜', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhugeliang', N'诸葛亮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhurong', N'祝融', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zoushi', N'邹氏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zuoci', N'左慈', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GuanduWarfare', N'官渡之战', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'''s ROOM', N'的游戏间', N'Asset/Prefabs/SettingScript.cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Registration Successful', N'注册成功', N'Asset/NewWord/MessageHandlerScript.sc')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Protocol Wrong', N'错误的通讯协议', N'Asset/NewWord/MessageHandlerScript.sc')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Switch Failed', N'操作失败', N'Asset/NewWord/MessageHandlerScript.sc')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'leave now?', N'确定离开吗？', N'Asset/Prefabs/ExitNoticeScript.sc')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'game is started, will you escape?', N'游戏已开始，确定要逃跑吗？', N'Asset/Prefabs/ExitNoticeScript.sc')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Slash', N'杀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'FireSlash', N'火杀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ThunderSlash', N'雷杀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Jink', N'闪', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Peach', N'桃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Analeptic', N'酒', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CrossBow', N'诸葛连弩', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'DoubleSword', N'雌雄双股剑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'QinggangSword', N'青缸剑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'IceSword', N'寒冰剑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Spear', N'丈八蛇矛', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Fan', N'朱雀羽扇', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Axe', N'贯石斧', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'KylinBow', N'麒麟弓', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SixSword', N'吴六剑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Triblade', N'三尖两刃刀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'EightDiagram', N'八卦阵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'RenwangShield', N'仁王盾', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Vine', N'藤甲', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SilverLion', N'白银狮子', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Jueying', N'绝影', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Dilu', N'的卢', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Zhuahuangfeidian', N'爪黄飞电', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Chitu', N'赤兔', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Dayuan', N'大苑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Zixing', N'紫骍', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'AmazingGrace', N'五谷丰登', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GodSalvation', N'桃园结义', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SavageAssault', N'南蛮入侵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ArcheryAttack', N'万箭齐发', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Duel', N'决斗', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ExNihilo', N'无中生有', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Snatch', N'顺手牵羊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Dismantlement', N'过河拆桥', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'IronChain', N'铁索连环', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'FireAttack', N'火攻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Collateral', N'借刀杀人', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Nullification', N'无懈可击', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'HegNullification', N'无懈可击·国', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'KnownBoth', N'知己知彼', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'BefriendAttacking', N'远交近攻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'AwaitExhausted', N'以逸待劳', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Indulgence', N'乐不思蜀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SupplyShortage', N'兵粮寸断', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Lightning', N'闪电', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'DragonPhoenix', N'飞龙夺风', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'PeaceSpell', N'太平要术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ThreatenEmperor', N'挟天子以令诸侯', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'BurningCamps', N'火烧连营', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'FightTogether', N'戮力同心', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'AllianceFeast', N'联军盛宴', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LureTiger', N'调虎离山', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Drowning', N'水淹七军', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'imperial_order', N'敕令', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'IronArmor', N'明光铠', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Blade', N'青龙偃月刀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Jingfan', N'惊帆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'JadeSeal', N'玉玺', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'BreastPlate', N'护心境', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Woodenox', N'木牛流马', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Halberd', N'方天画戟', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LuminouSpearl', N'定澜夜明珠', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'anjiang', N'暗将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@define:changetolord', N'你可以选择更换为君主。', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@define:FirstShowReward', N'是否摸两张牌作为首亮奖励？', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#FirstShowReward', N'%from 全场第一个亮将，可以摸两张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule_AskForGeneralShowHead', N'明置主将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule_AskForGeneralShowDeputy', N'明置副将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'armorskill', N'选择要发动的技能', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#BasaraReveal', N'%from 明置了武将，主将为 %arg，副将为 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#BasaraConceal', N'%from 暗置了武将，主将为 %arg，副将为 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#BasaraRemove', N'%from 移除了 %arg %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule_AskForGeneralShow', N'明置武将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule:TurnStart', N'选择需要明置的武将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'show_head_general', N'明置武将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'show_deputy_general', N'明置副将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'show_both_generals', N'明置双将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Companions', N'珠联璧合', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'head_general', N'主将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'deputy_general', N'副将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CompanionEffect', N'珠联璧合', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CompanionEffect:recover', N'回复1点体力', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CompanionEffect:draw', N'摸两张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#HalfMaxHpLeft', N'%from 的武将牌上有单独的阴阳鱼，可以摸一张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule_AskForArraySummon', N'阵法召唤', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#SummonType', N'召唤阵列为 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'summon_type_siege', N'围攻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'summon_type_formation', N'队列', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#SummonResult', N'%from 选择了 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'summon_success', N'响应', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'summon_failed', N'不响应', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SiegeSummon', N'响应围攻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SiegeSummon!', N'响应围攻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'FormationSummon', N'响应队列', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'FormationSummon!', N'响应队列', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'GameRule:TriggerOrder', N'请选择先发动的技能', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'trigger_none', N'不发动', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'anjiang_head', N'暗将（主）', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'anjiang_deputy', N'暗将（副）', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#BasaraGeneralChosen', N'你选择的武将为 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#BasaraGeneralChosenDual', N'你选择的武将为 %arg 和 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'view as ', N'视为', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yourself', N'你', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'use upon {0}', N'对{0}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yourown', N'自己', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'use upon {0}({1})', N'对{0}({1})', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'use', N'使用', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'response', N'打出', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'discard', N'弃置', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'spade_char', N'?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'club_char', N'?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'heart_char', N'?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'diamond_char', N'?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'spade', N'黑桃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'club', N'梅花', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'heart', N'红桃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'diamond', N'方块', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit', N'无色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit_black', N'黑色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit_red', N'红色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit_char', N'无色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit_black_char', N'黑色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'no_suit_red_char', N'红色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'basic', N'基本牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'trick', N'锦囊牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'equip', N'装备牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'BasicCard', N'基本牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'TrickCard', N'锦囊牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'EquipCard', N'装备牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ndtrick', N'非延时锦囊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'nothing', N'不发动', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'handcards', N'手牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'start', N'开始', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'judge', N'判定', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'retrial', N'改判', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'judgedone', N'的判定结果', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'put', N'置于', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'throw', N'弃置', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'enter', N'置入', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'backinto', N'移回弃牌堆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'draw', N'摸牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'play', N'出牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'finish', N'结束', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dismantled', N'被弃置', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'get', N'被获得', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'recast', N'重铸', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'pindian', N'拼点', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'alter_pindian', N'点数视为', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'alter_pindian_card', N'拼点牌被替换', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'change equip', N'更换装备', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'turnover', N'从牌堆顶翻开', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'preview', N'观看牌堆顶', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'preeffect', N'即将生效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'show', N'展示', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'transfer_next', N'移动至下家', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'online', N'在线', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'offline', N'离线', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'bot', N'电脑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'trust', N'托管', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'discardPile', N'弃牌堆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'drawPileTop', N'牌堆顶', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'drawPileBottom', N'牌堆底', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'generalPile', N'武将牌堆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luck_card', N'手气卡', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'default', N'默认', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'recasting', N'重铸', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'%from {3} [{0}] {4}, {2} [{1}]', N'%from {3} “{0}” {4}， {2}了 [{1}]', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'%from {1} {0}', N'%from {1}了 {0}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'carry out', N'执行了', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'use skill', N'发动了', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'%from {1} [{0}] {2}', N'%from {1}“{0}”{2}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'%from {2} [{0}] {3}, and the cost is {1}', N'%from {2}“{0}”{3}弃置了 {1}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'%from {4} [{0}] {5} {3} {1} as {2}', N'%from {4}“{0}”{5}将 {1} 当成 {2} {3}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'add to {0} pile [{1}]', N'移入{0}的“{1}”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Due to the effect of [{0}], %from {3} {1} as {2}', N'由于“{0}”的效果，%from 的 {1} 被视为 {2} {3}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N', target is %name used by %arg2', N'，目标是 %arg2 使用的 %name', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N', target is %name', N'，目标是 %name', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N', will cancel it''s effect to %to', N'，将抵消其对 %to 的效果', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N', will cancel it''s effect', N'，将抵消其效果', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N', target is %to', N'，目标是 %to', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Do you want to use the luck card?', N'要使用手气卡更换手牌吗？', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Murder', N'%to【%arg】 阵亡，伤害来源为 %from', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Suicide', N'%to【%arg】 自杀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#InvokeSkill', N'%from 发动了“%arg”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#InvokeOthersSkill', N'%from 发动了 %to 的“%arg”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#ChoosePlayerWithSkill', N'%from 发动了“%arg”，目标是 %to', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#TriggerSkill', N'%from 的“%arg”被触发', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Pindian', N'%from 向 %to 发起了拼点', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#PindianSuccess', N'%from (对 %to) 拼点赢！', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#PindianFailure', N'%from (对 %to) 拼点没赢', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Damage', N'%from 对 %to 造成了 %arg 点伤害[%arg2]', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#DamageNoSource', N'%to 受到了 %arg 点伤害[%arg2]', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Recover', N'%from 回复了 %arg 点体力', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#ChooseKingdom', N'%from 选择了 %arg 势力', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$NullificationDetails2', N'%from 使用了 %card, 目标是对 %to 的锦囊 【%arg】', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hegnul_single', N'对单一角色生效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hegnul_all', N'对该势力的全体剩余角色生效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#CardNullified', N'【%arg】对 %from 无效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Transfigure', N'%from 变身为 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#TransfigureDual', N'%from 的 %arg2 变身为 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#AcquireSkill', N'%from 获得了技能“%arg”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#LoseSkill', N'%from 失去了技能“%arg”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$InitialJudge', N'%from 的判定牌为 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ChangedJudge', N'%from 发动“%arg”把 %to 的判定牌改为 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$MoveCard', N'%to 从 %from 处获得 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$PasteCard', N'%from 对 %to 使用延时锦囊 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$LightningMove', N'%card 从 %from 移动到 %to', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$TurnOver', N'%from 展示了牌堆顶的 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$DiscardCard', N'%from 弃置了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$DiscardCardWithSkill', N'%from 弃置了 %card 发动“%arg”', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$DiscardCardByOther', N'%from 弃置了 %to 的卡牌 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$EnterDiscardPile', N'%card 被置入弃牌堆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$MoveToDiscardPile', N'%from 将 %card 置入弃牌堆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$GotCardBack', N'%from 收回了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$RecycleCard', N'%from 从弃牌堆获得了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$Dismantlement', N'%from 的 %card 被弃置', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ShowCard', N'%from 展示了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ShowAllCards', N'%from 展示了所有手牌 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ViewAllCards', N'%from 观看了 %to 的所有手牌 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ViewDrawPile', N'%from 观看了牌堆顶的 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ViewDrawPile2', N'%from 观看了牌堆顶的 %arg 张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$JileiShowAllCards', N'%from 展示了不能弃置的手牌 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$PutCard', N'%from 的 %card 被置于牌堆顶', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#PutCard', N'%from 的 %arg 张牌被置于牌堆顶', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$PutCardToDrawPileBottom', N'%from 的 %card 被置于牌堆底', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#PutCardToDrawPileBottom', N'%from 的 %arg 张牌被置于牌堆底', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'normal_nature', N'无属性', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fire_nature', N'火焰属性', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'thunder_nature', N'雷电属性', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#Contingency', N'%to【%arg】 阵亡，无伤害来源', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#DelayedTrick', N'%from 的延时锦囊【%arg】开始判定', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#SkillNullify', N'%from 的“%arg”被触发，【%arg2】对其无效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#ArmorNullify', N'%from 的防具【%arg】效果被触发，【%arg2】对其无效', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#DrawNCards', N'%from 摸了 %arg 张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$DrawCards', N'%from 摸了 %arg 张牌 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#MoveNCards', N'%to 从 %from 处得到 %arg 张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$TakeAG', N'%from 获得了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$Install', N'%from 装备了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$Uninstall', N'%from 卸载了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$JudgeResult', N'%from 的判定结果为 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$PindianResult', N'%from 的拼点牌为 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#ChooseSuit', N'%from 选择了花色 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#TurnOver', N'%from 将武将牌叠置，现在是 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'face_up', N'平置状态', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'face_down', N'叠置状态', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#SkipPhase', N'%from 跳过了 %arg 阶段', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#SkipAllPhase', N'%from 当前回合结束', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#IronChainDamage', N'%from 处于横置状态，将受到传导的属性伤害', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#LoseHp', N'%from 失去了 %arg 点体力', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#LoseMaxHp', N'%from 失去了 %arg 点体力上限', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#GainMaxHp', N'%from 增加了 %arg 点体力上限', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#GetHp', N'%from 当前体力为 %arg ，体力上限为 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#ChangeKingdom', N'%from 把 %to 的势力由 %arg 改成了 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#GetMark', N'%from 得到了 %arg2 枚 %arg 标记', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#LoseMark', N'%from 失去了 %arg2 枚 %arg 标记', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$AddToPileFrom', N'%from 将 %card 作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#RemoveFromGameFrom', N'%from 将 %arg2 张牌作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$AddToPile', N'%from 的 %card 被作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#RemoveFromGame', N'%from 的 %arg2 张牌被作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$AddToPile1', N'%card 被作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#RemoveFromGame1', N'%arg2 张牌被作为 %to 的 %arg 牌移出游戏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$GotCardFromPile', N'%from 从 %arg 牌中获得 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#GotNCardFromPile', N'%from 从 %arg 牌中获得 %arg2 张牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@askforslash', N'你可以对你攻击范围内的一名其他角色使用一张【杀】', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@askforretrial', N'请使用“%dest”来修改 %src 的 %arg 判定', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$CheatCard', N'%from 作弊，获得了 %card', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#FilterJudge', N'%from 的“%arg”效果被触发，判定牌被改变', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$CancelTarget', N'%from 使用【%arg】的目标 %to 被取消', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$CancelTargetNoUser', N'【%arg】的目标 %to 被取消', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'$ViewRole', N'%from 观看了 %to 的身份 %arg', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#UseLuckCard', N'%from 使用了 <color=yellow><b>手气卡</b></color>', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@attribute', N'已为%src分配', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'cw', N'顺时针', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ccw', N'逆时针', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'warm', N'暖色方', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'cool', N'冷色方', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#VsTakeGeneral', N'%arg 选择了 %arg2', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#TrickDirectio', N'%from 选择了 %arg 作为结算顺序', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'@waked', N'觉醒', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(1)', N'一', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(2)', N'二', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(3)', N'三', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(4)', N'四', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(5)', N'五', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(6)', N'六', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(7)', N'七', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(8)', N'八', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(9)', N'九', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'CAPITAL(10)', N'十', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(1)', N'一号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(2)', N'二号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(3)', N'三号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(4)', N'四号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(5)', N'五号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(6)', N'六号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(7)', N'七号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(8)', N'八号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(9)', N'九号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SEAT(10)', N'十号位', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'SingleTargetTrick', N'单体锦囊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'MultiTarget', N'群体锦囊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'DelayedTrick', N'延时锦囊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'convert_general', N'武将转换', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} {1} card(s) are required at least', N'你至少需要弃置 {0} 张{1}牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'<b>Source</b>: {0}', N'<b>技能来源</b>: {0}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please choose {0} to {1} players', N'请选择 {0} 至 {1} 名目标', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Plsase choose {0} players at most', N'请选择最多 {0} 名目标', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please choose a player', N'请选择一名目标', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} card(s) are selected', N'已选择 {0} 张卡牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'<b>Notice</b>: {0}', N'<b>提示</b>: {0}', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'using', N'使用', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'*', N'×', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} (use upon {1})', N'{0} (对{1})', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'anonymous user', N'匿名用户', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'InGame', N'游戏中', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Waiting', N'等待中', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Do you want to use nullification to trick card {0}', N'是否对 {1} 的 {0} 使用【无懈可击】？', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} used trick card {1} to {2},\nDo you want to us', N'{0} 对 {2} 使用锦囊【{1}】，是否使用【无懈可击】？', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Do you want to invoke skill [{0}] ?', N'你想发动技能“{0}”吗?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Do you want to invoke skill [{0}] to {1} ?', N'你想对{1}发动技能“{0}”吗?', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please discard {0} card(s), include equip', N'请弃置 {0} 张牌，包括装备区的牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please discard {0} card(s), only hand cards is all', N'请弃置 {0} 张手牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qiaobian', N'巧变', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'duanliang', N'断粮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jushou', N'据守', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qiangxi', N'强袭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'quhu', N'驱虎', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jieming', N'节命', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xingshang', N'行殇', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fangzhu', N'放逐', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiaoguo', N'骁果', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qingnang', N'青囊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jijiu', N'急救', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wushuang', N'无双', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lijian', N'离间', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'biyue', N'闭月', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luanji', N'乱击', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shuangxiong', N'双雄', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wansha', N'完杀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'weimu', N'帷幕', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luanwu', N'乱武', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu_pangde', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mengjin', N'猛进', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'leiji', N'雷击', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guidao', N'鬼道', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'beige', N'悲歌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'duanchang', N'断肠', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu_mateng', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiongyi', N'雄异', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mingshi', N'名士', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lirang', N'礼让', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shuangren', N'双刃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sijian', N'死谏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'suishi', N'随势', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kuangfu', N'狂斧', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huoshui', N'祸水', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qingcheng', N'倾城', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'rende', N'仁德', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wusheng', N'武圣', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'paoxiao', N'咆哮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guanxing', N'观星', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kongcheng', N'空城', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'longdan', N'龙胆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tieqi', N'铁骑', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jizhi', N'集智', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qicai', N'奇才', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'liegong', N'烈弓', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kuanggu', N'狂骨', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lianhuan', N'连环', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'niepan', N'涅槃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'bazhen', N'八阵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huoji', N'火计', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kanpo', N'看破', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiangle', N'享乐', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fangquan', N'放权', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huoshou', N'祸首', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zaiqi', N'再起', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'juxiang', N'巨象', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lieren', N'烈刃', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shushen', N'淑慎', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shenzhi', N'神智', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhiheng', N'制衡', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qixi', N'奇袭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'keji', N'克己', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'kurou', N'苦肉', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yingzi_zhouyu', N'英姿', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fanjian', N'反间', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guose', N'国色', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'liuli', N'流离', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qianxun', N'谦逊', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'duoshi', N'度势', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jieyin', N'结姻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiaoji', N'枭姬', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yinghun_sunjian', N'英魂', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hongyan', N'红颜', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tianxiang', N'天香', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tianyi', N'天义', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'buqu', N'不屈', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'haoshi', N'好施', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'dimeng', N'缔盟', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhijian', N'直谏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guzheng', N'固政', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'duanbing', N'短兵', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fenxun', N'奋迅', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tuntian', N'屯田', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jixi', N'急袭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ziliang', N'资粮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huyuan', N'护援', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'heyi', N'鹤翼', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'feiying', N'飞影', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tiaoxin', N'挑衅', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yizhi', N'遗志', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tianfu', N'天覆', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shengxi', N'生息', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shoucheng', N'守成', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shangyi', N'尚义', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'niaoxiang', N'鸟翔', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yicheng', N'疑城', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qianhuan', N'千幻', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhendu', N'鸩毒', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qiluan', N'戚乱', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhangwu', N'章武', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shouyue', N'授钺', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jizhao', N'激诏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'chuli', N'除疠', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mouduan', N'谋断', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jianchu', N'鞬出', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xunxun', N'恂恂', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wangxi', N'忘隙', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hengjiang', N'横江', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu_madai', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qianxi', N'潜袭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guixiu', N'闺秀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'cunsi', N'存嗣', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yongjue', N'勇决', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiang', N'激昂', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yingyang', N'鹰扬', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hunshang', N'魂殇', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yingzi_sunce', N'英姿', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yinghun_sunce', N'英魂', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'duanxie', N'断绁', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fenming', N'奋命', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hengzheng', N'横征', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'baoling', N'暴凌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'benghuai', N'崩坏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'chuanxin', N'穿心', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fengshi', N'锋矢', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wuxin', N'悟心', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wendao', N'问道', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hongfa', N'弘法', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'hongfaslash', N'黄巾天兵符', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qice', N'奇策', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhiyu', N'智愚', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wanwei', N'挽危', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yuejian', N'约俭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xiongsuan', N'凶算', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'huashen', N'化身', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xinsheng', N'新生', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jili', N'蒺藜', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'sanyao', N'散谣', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'zhiman', N'制蛮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'xuanlue', N'旋略', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yongjin', N'勇进', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'diaodu', N'调度', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'diancai', N'典财', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jiahe', N'嘉禾', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'lianzi', N'敛资', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'jubao', N'聚宝', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'flamemap', N'缘江烽火图', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'yiji', N'遗计', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tiandu', N'天妒', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wei', N'魏', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'shu', N'蜀', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'wu', N'吴', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qun', N'群', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'qingguo', N'倾国', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luoshen', N'洛神', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'tuxi', N'突袭', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'No Limited', N'无限制', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LuckCard On', N'启用手气卡', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Lord Convert', N'君主转换', NULL)
GO
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'StanderCards', N'标准', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'StrategicAdvantageCards', N'势备', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Kick Off', N'踢出房间', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Add Bot', N'添加AI', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} Seconds', N'{0} 秒', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'LuckCard Off', N'禁用手气卡', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Speaking Forbidden', N'禁止聊天', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'undefine', N'未指定', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Head', N'主将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Deputy', N'副将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please select the same nationality generals', N'请选择国籍相同的武将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please select one general', N'请选择一位武将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guanxing:up', N'牌堆顶', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guanxing:down', N'牌堆底', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} is arrenging cards', N'{0} 正在排列卡牌', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please play a card for pindian', N'请出一张手牌进行拼点', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} ask for you to play a card to pindian', N'{0} 要求与你拼点，请出一张手牌', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} is choosing card', N'{0} 正在选牌', N'CardShowBoxScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please choose a card', N'请选择一张牌', N'CardShowBoxScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fenji', N'奋激', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'please choose {0}''s card', N'请选择 {0} 的一张卡牌', N'CardChooseBox2.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please discard {0} hand card(s)', N'请弃置 {0} 张手牌', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ImperialOrder:show', N'亮将', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ImperialOrder:losehp', N'失去体力', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'luoyi', N'裸衣', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu_machao', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'mashu_pande', N'马术', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'ImperialOrder', N'敕令', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'effect', N'的效果', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N':tianyi', N'发起者赢：本回合使用【杀】上限+1，物理无限制，目标上限+1；输：本回合不能使用【杀】', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N':quhu', N'发起者赢：选择响应者攻击范围内的一名其他角色，响应者对其造成一点伤害；输：响应者对发起者造成一点伤害', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'#paoxiao-tm', N'咆哮', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please distribute {0} cards {1} as you wish', N'请将 {0} 张牌交给任意一名{1}角色', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'to another player', N'给其他角色', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please give {0} cards to exchange at most', N'请至多选择 {0} 张牌', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please give {0} cards to exchange', N'请选择用于交换的 {0} 张手牌', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'Please give {0} to {1} cards to exchange', N'请选择 {0} 至 {1} 张牌', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'You are dying, provide {0} peach(es)(or analeptic)', N'你处于濒死状态，请提供 {0} 个【桃】（或【酒】）来自救', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'{0} is dying, provide {1} peach(es) to save him', N'{0} 处于濒死状态，请提供  {1} 个【桃】来挽救该角色', N'ClientInstanceScript.Cs')
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'guicai', N'鬼才', NULL)
INSERT [dbo].[translation] ([key], [translation], [path]) VALUES (N'fankui', N'反馈', NULL)
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_title]    Script Date: 2018/11/21 0:44:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_title] ON [dbo].[title]
(
	[title_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_translation]    Script Date: 2018/11/21 0:44:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_translation] ON [dbo].[translation]
(
	[key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[cards] ADD  CONSTRAINT [DF_cards_transferable]  DEFAULT ((0)) FOR [transferable]
GO
ALTER TABLE [dbo].[cards] ADD  CONSTRAINT [DF_cards_can_recast]  DEFAULT ((0)) FOR [can_recast]
GO
ALTER TABLE [dbo].[generals] ADD  CONSTRAINT [DF_generals_adjust_hp]  DEFAULT ((0)) FOR [adjust_hp]
GO
ALTER TABLE [dbo].[generals] ADD  CONSTRAINT [DF_generals_classic_lord]  DEFAULT ((0)) FOR [classic_lord]
GO
ALTER TABLE [dbo].[generals] ADD  CONSTRAINT [DF_generals_hidden]  DEFAULT ((0)) FOR [hidden]
GO
ALTER TABLE [dbo].[skills] ADD  CONSTRAINT [DF_skills_attach_lord]  DEFAULT ((0)) FOR [attach_lord]
GO
ALTER TABLE [dbo].[achieve]  WITH CHECK ADD  CONSTRAINT [FK_achieve_title] FOREIGN KEY([reward_title_id])
REFERENCES [dbo].[title] ([title_id])
GO
ALTER TABLE [dbo].[achieve] CHECK CONSTRAINT [FK_achieve_title]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_card_package] FOREIGN KEY([package])
REFERENCES [dbo].[card_package] ([package_name])
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_card_package]
GO
ALTER TABLE [dbo].[general_hp_adjust]  WITH CHECK ADD  CONSTRAINT [FK_general_hp_adjust_generals] FOREIGN KEY([general_name])
REFERENCES [dbo].[generals] ([general_name])
GO
ALTER TABLE [dbo].[general_hp_adjust] CHECK CONSTRAINT [FK_general_hp_adjust_generals]
GO
ALTER TABLE [dbo].[general_skill]  WITH CHECK ADD  CONSTRAINT [FK_general-skill_generals] FOREIGN KEY([general])
REFERENCES [dbo].[generals] ([general_name])
GO
ALTER TABLE [dbo].[general_skill] CHECK CONSTRAINT [FK_general-skill_generals]
GO
ALTER TABLE [dbo].[general_skin]  WITH CHECK ADD  CONSTRAINT [FK_general_skin_generals] FOREIGN KEY([general_name])
REFERENCES [dbo].[generals] ([general_name])
GO
ALTER TABLE [dbo].[general_skin] CHECK CONSTRAINT [FK_general_skin_generals]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [卡牌花色] CHECK  (([suit]='club' OR [suit]='diamond' OR [suit]='heart' OR [suit]='spade'))
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [卡牌花色]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [卡牌类型] CHECK  (([type]='basic' OR [type]='equip' OR [type]='trick'))
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [卡牌类型]
GO
ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [卡牌数字在A~K之间] CHECK  (([number]>=(1) AND [number]<=(13)))
GO
ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [卡牌数字在A~K之间]
GO
ALTER TABLE [dbo].[general_skin]  WITH CHECK ADD  CONSTRAINT [id] CHECK  (([skin_id]>=(0)))
GO
ALTER TABLE [dbo].[general_skin] CHECK CONSTRAINT [id]
GO
ALTER TABLE [dbo].[generals]  WITH CHECK ADD  CONSTRAINT [kingdom] CHECK  (([kingdom]='wu' OR [kingdom]='shu' OR [kingdom]='wei' OR [kingdom]='qun' OR [kingdom]='god'))
GO
ALTER TABLE [dbo].[generals] CHECK CONSTRAINT [kingdom]
GO
USE [master]
GO
ALTER DATABASE [Sanguosha-data] SET  READ_WRITE 
GO
