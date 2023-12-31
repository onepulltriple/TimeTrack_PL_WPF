USE MASTER
GO

DROP DATABASE TIMETRACK_PL
CREATE DATABASE TIMETRACK_PL
GO

USE TIMETRACK_PL
GO


CREATE TABLE PROJECTS (
	id int PRIMARY KEY IDENTITY(1,1),
	Name varchar(50),
	Number varchar(50) NOT NULL UNIQUE,
	Description varchar(255),
	IsArchived bit NOT NULL DEFAULT 0
	)

SELECT * FROM PROJECTS


CREATE TABLE TASKS (
	id int PRIMARY KEY IDENTITY(1,1),
	Name varchar(50) NOT NULL,
	Description varchar(255),
	Project_id int NOT NULL,
	CONSTRAINT FK_ParentProjectChildTask
		FOREIGN KEY (Project_id) 
			REFERENCES PROJECTS(id)
			ON DELETE CASCADE
	)

SELECT * FROM TASKS


CREATE TABLE INTERVALS (
	id int PRIMARY KEY IDENTITY(1,1),
	StartTimeActual datetime NOT NULL,
	EndTimeActual datetime,
	StartTimeRounded datetime,
	EndTimeRounded datetime,
	Task_id int NOT NULL,
	CONSTRAINT FK_ParentTaskChildInterval
		FOREIGN KEY (Task_id) 
			REFERENCES TASKS(id)
			ON DELETE CASCADE
	)

SELECT * FROM INTERVALS


CREATE TABLE EMPLOYEEPUNCHCLOCKINTERVALS (
	id int PRIMARY KEY IDENTITY(1,1),
	StartTimeActual datetime NOT NULL,
	EndTimeActual datetime,
	StartTimePunchClockSoftware datetime,
	EndTimePunchClockSoftware datetime
	)

SELECT * FROM EMPLOYEEPUNCHCLOCKINTERVALS