Feature: Counter
The persistent page as a scenario with the StaticOwningComponentTest base

Scenario: Use the Counter page
	Given the '/counter' page is loaded
	Then the counter value is 0
	And the '#countP' element matches '<p diff:ignoreAttributes>Current count: 0</p>'
	When the increment button is clicked 100 times
	Then the counter value is 100
	And the '#countP' element matches '<p diff:ignoreAttributes>Current count: 100</p>'
	When the persistence storage is cleared
	Then the counter value is 0

Scenario: Persistence
	Given the '/counter' page is loaded
	Then the counter value is 0
	When the increment button is clicked 10 times
	Then the counter value is 10
	When the page is reloaded
	Then the counter value is 10
	And the '#countP' element matches '<p diff:ignoreAttributes>Current count: 10</p>'
	When the persistence storage is cleared
	Then the counter value is 0