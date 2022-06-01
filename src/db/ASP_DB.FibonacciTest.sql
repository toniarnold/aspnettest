-- Saved ShareButton export file with manually inserted GUID for reproducability
USE [ASP_DB]
GO

-- WebForms
INSERT INTO Main (session, clsid, main) SELECT 'DE2CAAF5-6602-456D-B1F9-874095359593', 'EEA41DED-A899-406F-A293-B06917DD48E6', 0x0001000000FFFFFFFF01000000000000000C02000000436173702E776562666F726D732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C0C030000004953797374656D2C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038390501000000216173702E63616C63756C61746F722E436F6E74726F6C2E43616C63756C61746F7202000000045F66736D065F737461636B04041143616C63756C61746F72436F6E7465787402000000800153797374656D2E436F6C6C656374696F6E732E47656E657269632E537461636B60315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D03000000020000000904000000090500000005040000001143616C63756C61746F72436F6E746578740200000009737461636B53697A65057374617465000008080200000000000000020000000505000000800153797374656D2E436F6C6C656374696F6E732E47656E657269632E537461636B60315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D03000000065F6172726179055F73697A65085F76657273696F6E06000008080300000009060000000700000007000000110600000008000000060700000001310608000000013106090000000132060A0000000133060B0000000135060C0000000138060D0000000231330A0B
SELECT session FROM Main WHERE mainid = @@IDENTITY

-- Core
INSERT INTO Main (session, clsid, main) SELECT '131C17EF-EA02-48B5-A199-D12605CB8778', 'A4A6AD10-6435-4E86-B047-9BC3DCCF5982', 0x0001000000FFFFFFFF01000000000000000401000000E20153797374656D2E436F6C6C656374696F6E732E47656E657269632E44696374696F6E61727960325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D040000000756657273696F6E08436F6D7061726572084861736853697A650D4B657956616C756550616972730003000308920153797374656D2E436F6C6C656374696F6E732E47656E657269632E47656E65726963457175616C697479436F6D706172657260315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D08E60153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D5B5D0300000009020000000300000009030000000402000000920153797374656D2E436F6C6C656374696F6E732E47656E657269632E47656E65726963457175616C697479436F6D706172657260315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D00000000070300000000010000000300000003E40153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D0C050000004953797374656D2C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6237376135633536313933346530383904FCFFFFFFE40153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D02000000036B65790576616C75650102060600000005737461636B09070000000C090000003F6173702E636F72652C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C01F8FFFFFFFCFFFFFF060A000000143C46736D3E6B5F5F4261636B696E674669656C64090B00000001F4FFFFFFFCFFFFFF060D0000001F3C53657373696F6E53746F726167653E6B5F5F4261636B696E674669656C640A0507000000800153797374656D2E436F6C6C656374696F6E732E47656E657269632E537461636B60315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D03000000065F6172726179055F73697A65085F76657273696F6E060000080805000000090E0000000700000007000000050B0000001143616C63756C61746F72436F6E746578740200000009737461636B53697A6505737461746500000808090000000000000002000000110E00000008000000060F0000000131061000000001310611000000013206120000000133061300000001350614000000013806150000000231330A0B
SELECT session FROM Main WHERE mainid = @@IDENTITY

-- WebSharper
INSERT INTO Main (session, clsid, main) SELECT '5EC80614-B765-4281-3481-08D80B9F50D6', 'C718C2DA-0C9E-4A52-951C-C0739DE28B7E', 0x0001000000FFFFFFFF01000000000000000C020000004F6173702E776562736861727065722E7370612E4D6F64656C2C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C0501000000236173702E776562736861727065722E7370612E4D6F64656C2E43616C63756C61746F72030000000D646973706F73656456616C756505737461636B143C46736D3E6B5F5F4261636B696E674669656C64000404012A6173702E776562736861727065722E7370612E4D6F64656C2E53657269616C697A61626C65537461636B020000001143616C63756C61746F72436F6E746578740200000002000000000903000000090400000005030000002A6173702E776562736861727065722E7370612E4D6F64656C2E53657269616C697A61626C65537461636B030000000E537461636B60312B5F61727261790D537461636B60312B5F73697A6510537461636B60312B5F76657273696F6E0600000808020000000905000000070000000700000005040000001143616C63756C61746F72436F6E746578740200000009737461636B53697A650573746174650000080802000000000000000200000011050000000800000006060000000131060700000001310608000000013206090000000133060A0000000135060B0000000138060C0000000231330A0B
SELECT session FROM Main WHERE mainid = @@IDENTITY

-- Blazor
INSERT INTO Main (session, clsid, main) SELECT '45aff7ba-2137-4cef-1997-08da438d667f', 'f39f40ba-4152-4be0-8907-70f4b135e29d', 0x0001000000FFFFFFFF01000000000000000C02000000416173702E626C617A6F722C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C0C030000004953797374656D2C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038390501000000236173702E626C617A6F722E43616C63756C61746F72536D632E43616C63756C61746F7202000000045F66736D065F737461636B04041143616C63756C61746F72436F6E7465787402000000800153797374656D2E436F6C6C656374696F6E732E47656E657269632E537461636B60315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D03000000020000000904000000090500000005040000001143616C63756C61746F72436F6E746578740200000009737461636B53697A65057374617465000008080200000000000000020000000505000000800153797374656D2E436F6C6C656374696F6E732E47656E657269632E537461636B60315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D03000000065F6172726179055F73697A65085F76657273696F6E06000008080300000009060000000700000007000000110600000008000000060700000001310608000000013106090000000132060A0000000133060B0000000135060C0000000138060D0000000231330A0B
SELECT session FROM Main WHERE mainid = @@IDENTITY