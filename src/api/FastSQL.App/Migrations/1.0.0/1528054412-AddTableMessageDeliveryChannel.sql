
-- Name: example-9b6feb45-2cf0-4505-862b-6adc86111bc1
-- Date: 6/4/2018 2:33:26 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_message_delivery_channels (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[ChannelId] NVARCHAR(255) NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(4000),
	[CreatedAt] INT NOT NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_message_delivery_channels;
