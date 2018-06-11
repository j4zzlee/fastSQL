
-- Name: example-9b6feb45-2cf0-4505-862b-6adc86111bc1
-- Date: 6/4/2018 2:33:26 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_messages (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[Message] TEXT NOT NULL,
	[CreatedAt] INT NOT NULL,
	[MessageType] INT NOT NULL
);

CREATE TABLE core_messages_channels (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[MessageId] UNIQUEIDENTIFIER,
	[Channel] TEXT NOT NULL,
	[Status] INT NOT NULL,
	[CreatedAt] INT NOT NULL,
	[DeliverAt] INT NOT NULL
);
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_message_items;
DROP TABLE core_messages_channels;
