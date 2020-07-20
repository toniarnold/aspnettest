# Makefile to build the Image context and the diagram

all: Image_sm.cs Image_sm.png

Image_sm.cs: Image.sm
	java -jar ..\..\smc\Smc.jar -csharp -serial -reflection -generic Image.sm

Image_sm.dot: Image.sm
	java -jar ..\..\smc\Smc.jar -graph -glevel 1 Image.sm

Image_sm.png: Image_sm.dot
	dot -T png -o Image_sm.png Image_sm.dot

clean:
	-del Image_sm.cs
	-del Image_sm.dot
	-del Image_sm.png 
