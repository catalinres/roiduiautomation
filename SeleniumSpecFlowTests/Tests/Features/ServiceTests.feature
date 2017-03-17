Feature: ServiceTests

Scenario: Add a service
	Given I navigate to Service Catalog
	And I click Add in the bottom bar
	And I fill in 'Test Service A' in the Name field
	When I click Save in the bottom bar
	And I wait until the page is redirected
	Then The address should contain 'Services/Detail'
	And The address should contain a valid Guid
	And I navigate to Home Page

Scenario: Delete a service
	Given I navigate to Service Catalog
	And I click on the service 'Selenium Test Service 2'
	Then The address should contain 'Services/Detail'
	And The address should contain a valid Guid
	Given I click Delete in the bottom bar
	And I wait until the page is redirected
	Then The address should not contain 'Services/Detail'
	And The address should not contain a valid Guid

	#Scenario: Add and edit a service
	#Given I navigate to Service Catalog
	#And I click Add in the bottom bar
	#And I fill in 'Selenium Test Service B' in the Name field
	#When I click Save in the bottom bar
	#And I wait until the page is redirected
	#Then The address should contain 'Services/Detail'
	#And The address should contain a valid Guid
	#Given I navigate to Service Catalog
	#And I click on the service 'Selenium Test Service B'
	#Then The address should contain 'Services/Detail'
	#And The address should contain a valid Guid
	#Given I fill in 'Selenium Test Service B2' in the Name field
	#When I click Save in the bottom bar
	#Then The address should contain 'Services/Detail'
	#And The address should contain a valid Guid
	#And The Save button in the bottom bar should be disabled


