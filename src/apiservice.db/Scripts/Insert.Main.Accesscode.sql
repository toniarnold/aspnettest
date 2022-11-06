/*
Clean the Tables Main and Accesscode in order
and populate them with an empty example session main and test data
*/

DELETE FROM [Accesscode]
DBCC CHECKIDENT ('[Accesscode]', RESEED, 1);
DELETE FROM [Main]
DBCC CHECKIDENT ('[Main]', RESEED, 1);
GO

INSERT INTO [Main] ([session], [clsid], [main])
SELECT 'B962C764-A2A6-418E-8CF5-681A686FF1BE', '8F5C00A6-C0BE-4C67-ABE1-EA3BFB86B82A', 0x00

INSERT INTO [Accesscode] ([session], [phonenumber], [accesscode])
SELECT 'B962C764-A2A6-418E-8CF5-681A686FF1BE', '5555311577', '123456'

INSERT INTO [Accesscode] ([session], [phonenumber], [accesscode])
SELECT 'B962C764-A2A6-418E-8CF5-681A686FF1BE', '0445551751389', '654321'
