# nmake needs a manually created list of all GNU .dia files

DIA = apicaller-apiservice.dia \
	blazor-database.dia \
	components.dia \
	iie-component.dia \
	iie-page.dia \
	iie-test.dia \
	interaction-default.dia \
	interaction-withroot.dia \
	internet-explorer.dia \
	test-without-storage.dia \
	test-with-storage.dia \
	webforms-core.dia

.SUFFIXES: .dia .png
PNG = $(DIA:.dia=.png)
png: $(PNG)

.dia.png:
	dia $< --export $@

clean:
	-del *.png
