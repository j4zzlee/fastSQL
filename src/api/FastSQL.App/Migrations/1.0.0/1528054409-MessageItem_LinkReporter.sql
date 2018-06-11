
-- Name: example-9b6feb45-2cf0-4505-862b-6adc86111bc1
-- Date: 6/4/2018 2:33:26 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
CREATE TABLE core_rel_messages_reporters (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[MessageId] UNIQUEIDENTIFIER NOT NULL,
	[ReporterId] UNIQUEIDENTIFIER NOT NULL,
	[CreatedAt] INT NOT NULL
);
ALTER TABLE core_messages ADD [DeliverAt] INT;
ALTER TABLE core_messages ADD [Status] INT NOT NULL;
DROP TABLE core_messages_channels;
--Down--
----------------------------
-- Migration down goes here.
----------------------------
DROP TABLE core_rel_messages_reporters;
CREATE TABLE core_messages_channels (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	[MessageId] UNIQUEIDENTIFIER,
	[Channel] TEXT NOT NULL,
	[Status] INT NOT NULL,
	[CreatedAt] INT NOT NULL,
	[DeliverAt] INT NOT NULL
);
ALTER TABLE core_messages DROP COLUMN [DeliverAt];
ALTER TABLE core_messages DROP COLUMN [Status];