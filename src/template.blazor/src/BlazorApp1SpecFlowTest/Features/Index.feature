Feature: Index

Two pages of the template app as scenarios with the StaticComponentTest base

@index
Scenario: Load the index page
	Given the app is initially loaded
	Then the 'strong' element contains 'How is Blazor working for you?'

