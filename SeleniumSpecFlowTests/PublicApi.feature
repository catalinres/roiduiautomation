Feature: PublicAPI

Scenario: Login
	Given I login with username 'RESQA\CijsouwMTest1' and password 'reverofser'
	Then I should receive an authorization token

@AfterScenarioDeleteService
Scenario: Add Service
	Given I login with username 'RESQA\CijsouwMTest1' and password 'reverofser'
	And I add a new service with name 'SpecFlow Test Service'
	Then A service with the name 'SpecFlow Test Service' should exist