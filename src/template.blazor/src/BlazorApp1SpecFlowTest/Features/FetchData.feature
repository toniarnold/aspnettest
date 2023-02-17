Feature: FetchData

Two pages of the template app as scenarios with the StaticComponentTest base

Scenario: Load the FetchData page
	Given the '/fetchdata' page is loaded
	Then the title is 'Weather forecast'
	And the table has 5 rows
	And all table cells contain the appropriate data type
