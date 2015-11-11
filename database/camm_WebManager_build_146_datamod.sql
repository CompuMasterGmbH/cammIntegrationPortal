---------------------------------------------
-- Add new column RequiredUserProfileFlags --
---------------------------------------------
if not exists (select * from dbo.syscolumns where id = object_id('dbo.System_Languages') and name = 'DirectionOfLetters') 
ALTER TABLE dbo.System_Languages ADD
	DirectionOfLetters varchar(3) NULL
