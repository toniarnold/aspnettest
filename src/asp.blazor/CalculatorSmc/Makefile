# Makefile to build the Calculator context and the diagram fron

all: Calculator_sm.cs Calculator_sm.png

Calculator_sm.cs: Calculator.sm
	java -jar ..\..\smc\Smc.jar -csharp -serial -reflection -generic Calculator.sm

Calculator_sm.dot: Calculator.sm
	java -jar ..\..\smc\Smc.jar  -graph -glevel 1 Calculator.sm
	REM Replace the first comment line to silence Code Access Security and nullable warnings:
	SET CS=Calculator_sm.cs
	powershell -Command "@('#pragma warning disable SYSLIB0003', '/* <auto-generated />') + ((Get-Content '%CS%') | Select-Object -Skip 1) | Set-Content -Path '%CS%'"

Calculator_sm.png: Calculator_sm.dot
	dot -T png -o Calculator_sm.png Calculator_sm.dot

clean:
	-del Calculator_sm.cs
	-del Calculator_sm.dot
	-del Calculator_sm.png
