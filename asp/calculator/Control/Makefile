# Makefile to build the Calculator context and the diagram fron 

all: Calculator_sm.cs Calculator_sm.dot Calculator_sm.png

Calculator_sm.cs: Calculator.sm
	java -jar ..\..\..\SMC\Smc.jar -csharp -serial -reflection -generic Calculator.sm

Calculator_sm.dot: Calculator.sm
	java -jar ..\..\..\SMC\Smc.jar  -graph -glevel 1 Calculator.sm

Calculator_sm.png: Calculator_sm.dot
	dot -T png -o Calculator_sm.png Calculator_sm.dot

clean:
	-del Calculator_sm.cs
	-del Calculator_sm.dot
	-del Calculator_sm.png 
