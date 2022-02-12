// Show the NUnit test result content in the same window tab
export function openXml(content) {
    var blob = new Blob([content], { type: 'application/xml; charset=UTF-8' });
    var dataUri = window.URL.createObjectURL(blob);
    window.open(dataUri, "_self");
    window.URL.revokeObjectURL(dataUri);
}