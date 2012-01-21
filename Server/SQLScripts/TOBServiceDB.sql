
/****** Object:  Table [dbo].[TOBClientRegistration]    Script Date: 01/13/2010 12:40:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TOBClientRegistration](
	[RegistrationId] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TOBClientRegistration] PRIMARY KEY CLUSTERED 
(
	[RegistrationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TOBClientTimeTracker]    Script Date: 01/13/2010 12:40:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TOBClientTimeTracker](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RegisterationId] [uniqueidentifier] NOT NULL,
	[TOBStartTime] [datetime] NULL,
	[TOBEndTime] [datetime] NULL,
	[Duration] [bigint] NULL,
 CONSTRAINT [PK_TOBClientTimeTracker] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_TOBClientTimeTracker_TOBClientRegistration]    Script Date: 01/13/2010 12:40:33 ******/
ALTER TABLE [dbo].[TOBClientTimeTracker]  WITH CHECK ADD  CONSTRAINT [FK_TOBClientTimeTracker_TOBClientRegistration] FOREIGN KEY([RegisterationId])
REFERENCES [dbo].[TOBClientRegistration] ([RegistrationId])
GO
ALTER TABLE [dbo].[TOBClientTimeTracker] CHECK CONSTRAINT [FK_TOBClientTimeTracker_TOBClientRegistration]
GO
