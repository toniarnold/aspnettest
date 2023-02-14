Feature: FetchData

Two pages of the template app as scenarios with the StaticComponentTest base

Scenario: Load the FetchData page
	Given the '/fetchdata' page is loaded
	Then the 'h1' element matches '<h1 diff:ignoreAttributes>Weather forecast</h1>'
	And the table has 5 rows
	And all table cells contain the appropriate data type
