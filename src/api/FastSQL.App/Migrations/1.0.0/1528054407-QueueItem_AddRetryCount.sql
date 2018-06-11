
-- Name: example-9b6feb45-2cf0-4505-862b-6adc86111bc1
-- Date: 6/4/2018 2:33:26 AM
-- Author: BaoChau
----------------------------
-- Migration up goes here.
----------------------------
ALTER TABLE core_queue_items ADD [RetryCount] INT NOT NULL;
--Down--
----------------------------
-- Migration down goes here.
----------------------------
ALTER TABLE core_queue_items DROP COLUMN [RetryCount]
