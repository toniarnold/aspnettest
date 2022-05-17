# Makefile to build the TestRunnerFsm context and the diagram from

all: TestRunnerFsm_sm.cs TestRunnerFsm_sm.png

TestRunnerFsm_sm.cs: TestRunnerFsm.sm
	java -jar ..\..\smc\Smc.jar -csharp -serial -reflection -generic TestRunnerFsm.sm

TestRunnerFsm_sm.dot: TestRunnerFsm.sm
	java -jar ..\..\smc\Smc.jar  -graph -glevel 1 TestRunnerFsm.sm

TestRunnerFsm_sm.png: TestRunnerFsm_sm.dot
	dot -T png -o TestRunnerFsm_sm.png TestRunnerFsm_sm.dot
	copy TestRunnerFsm_sm.png ..\wwwroot

clean:
	-del TestRunnerFsm_sm.cs
	-del TestRunnerFsm_sm.dot
	-del TestRunnerFsm_sm.png 
