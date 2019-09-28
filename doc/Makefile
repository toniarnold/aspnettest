# nmake needs a manually created list of all GNU .dia files

DIA = components.dia \
	internet-explorer.dia \
	iie-component.dia \
	iie-test.dia \
	iie-page.dia \
	test-without-storage.dia \
	test-with-storage.dia \
	interaction-default.dia \
	interaction-withroot.dia \
	webforms-core.dia \
	apicaller-apiservice.dia


.SUFFIXES: .dia .png
PNG = $(DIA:.dia=.png)
png: $(PNG)

.dia.png:
	dia $< --export $@

clean:
	-del *.png
