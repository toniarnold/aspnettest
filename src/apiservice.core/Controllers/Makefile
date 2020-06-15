# Makefile to build the Accesscode context and the diagram fron 

all: Accesscode_sm.cs Accesscode_sm.png

Accesscode_sm.cs: Accesscode.sm
	java -jar ..\..\smc\Smc.jar -csharp -serial -reflection -generic Accesscode.sm

Accesscode_sm.dot: Accesscode.sm
	java -jar ..\..\smc\Smc.jar  -graph -glevel 1 Accesscode.sm

Accesscode_sm.png: Accesscode_sm.dot
	dot -T png -o Accesscode_sm.png Accesscode_sm.dot
	xcopy /D /F /Y Accesscode_sm.png ..\..\..\doc\img

clean:
	-del Accesscode_sm.cs
	-del Accesscode_sm.dot
	-del Accesscode_sm.png 
