Feature: Calculator

SMC RPN calculator test assimilated to the SpecFlow template

Scenario: Add two numbers
	Given the first number is 50
	And the second number is 70
	When the add button is clicked
	Then the result should be 120

Scenario: Subtract two numbers
	Given the first number is 70
	And the second number is 50
	When the subtract button is clicked
	Then the result should be 20

Scenario: Multiply two numbers
	Given the first number is 70
	And the second number is 50
	When the multiply button is clicked
	Then the result should be 3500

Scenario: Divide two numbers
	Given the first number is 70
	And the second number is 5
	When the divide button is clicked
	Then the result should be 14

Scenario: Square a number
	Given the first number is 70
	When the square button is clicked
	Then the result should be 4900

Scenario: Extract the root from a number
	Given the first number is 49
	When the square root button is clicked
	Then the result should be 7