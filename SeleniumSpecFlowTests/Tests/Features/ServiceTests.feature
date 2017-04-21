Feature: ServiceTests

Scenario: Services_01 - Add a service - DEMO
	Given I navigate to Service Catalog
	And I click Add in the bottom bar
	And I fill in '--1Test Service A' in the Name field
	When I click Save in the bottom bar
	And I wait until the page is redirected
	Then The address should contain 'Services/Detail'
	And The address should contain a valid Guid
	And I navigate to Home Page

Scenario: Services_02 - Delete a service
	Given I navigate to Service Catalog
	And I click on the service '--1Test Service A'
	Then The address should contain 'Services/Detail'
	And The address should contain a valid Guid
	Given I click Delete in the bottom bar
	And I wait until the page is redirected
	Then The address should not contain 'Services/Detail'
	And The address should not contain a valid Guid

Scenario: Services_03 - DEMO
	Given I navigate to Service Catalog

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


