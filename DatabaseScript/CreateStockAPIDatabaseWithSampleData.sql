USE master;
GO

-- Create the database
CREATE DATABASE StockAPI;
GO

USE StockAPI;
GO

-- Create Brokers table
CREATE TABLE [dbo].[Brokers](
    [BrokerId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BrokerName] [nvarchar](100) NOT NULL
);

-- Insert data into Brokers table
INSERT INTO [dbo].[Brokers] ([BrokerName])
VALUES ('Broker A'),
       ('Broker B'),
       ('Broker C'),
       ('Broker D'),
       ('Broker E');

-- Create Stocks table
CREATE TABLE [dbo].[Stocks](
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TickerSymbol] [nvarchar](10) NOT NULL,
    [CurrentValue] [decimal](18, 2) NOT NULL,
    [Name] [nvarchar](255) NOT NULL
);

-- Insert data into Stocks table
INSERT INTO [dbo].[Stocks] ([TickerSymbol], [CurrentValue], [Name])
VALUES ('AAPL', 150.25, 'Apple Inc.'),
       ('MSFT', 300.10, 'Microsoft Corporation'),
       ('GOOGL', 2800.75, 'Alphabet Inc.'),
       ('AMZN', 3500.50, 'Amazon.com Inc.'),
       ('TSLA', 750.60, 'Tesla Inc.');

-- Create Trades table
CREATE TABLE [dbo].[Trades](
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TradeId] [uniqueidentifier] NOT NULL,
    [TickerSymbol] [nvarchar](10) NOT NULL,
    [Price] [decimal](18, 2) NOT NULL,
    [Shares] [decimal](18, 2) NOT NULL,
    [BrokerId] [int] NOT NULL,
    [TradeTime] [datetime] NOT NULL
);

-- Insert data into Trades table
INSERT INTO [dbo].[Trades] ([TradeId], [TickerSymbol], [Price], [Shares], [BrokerId], [TradeTime])
VALUES
    (NEWID(), 'AAPL', 150.25, 100, 1, GETDATE()),
    (NEWID(), 'MSFT', 300.10, 50, 2, GETDATE()),
    (NEWID(), 'GOOGL', 2800.75, 10, 3, GETDATE()),
    (NEWID(), 'AMZN', 3500.50, 20, 1, GETDATE()),
    (NEWID(), 'TSLA', 750.60, 30, 4, GETDATE());
