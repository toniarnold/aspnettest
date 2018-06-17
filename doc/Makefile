# nmake needs a manually created list of all GNU .dia files

DIA = components.dia \
	internet-explorer.dia

.SUFFIXES: .dia .png
PNG = $(DIA:.dia=.png)
png: $(PNG)

.dia.png:
	dia $< --export $@

clean:
	-del *.png
