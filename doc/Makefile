# nmake needs a manually created list of all GNU dia files
DIA = components.dia


PNG = $(DIA:.dia=.png)

png: $(PNG)

.SUFFIXES: .dia .png

.dia.png:
	dia $< --export $@

clean:
	-del *.png
