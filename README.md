# DOI-WebApi-TypedHttpClient
WebApi solution using a Typed HttpClient (.Net/#C) to access Digital Object Identifier (DOI) services and registration. 
- The International DOI Foundation (IDF) is the registration authority for the ISO standard (ISO 26324) for the DOI system.
- The DOI System is a way to uniquely identify e.g. scientific articles and documents. DOIs aid in citation tracking, ensuring a researcher has accurate metrics on how and where their research outputs are being used or referenced. 
About DOI: https://en.wikipedia.org/wiki/Digital_object_identifier
 
- This solution can also be used as an example on how to implement a Named Typed Http Client in .Net MVC. 
- This VisualStudio Solution can be plugged into your solution/project to create and publish a DOI.
DataCite REST API Guide (Developer docu): https://support.datacite.org/docs/api

Running the solution will start Swagger UI.
Before running the solution your organisation need to have a test-site created by DataCite (https://datacite.org/value.html) - url, user, password. Add you values in the appsettings.json file.

The Test-project of the VisualStudio solution holds TestCases to:
- test deserialize (ResponseJsonToClassMostFieldTest).
- test  serialize (RequestClassToJsonSerializeTest).
- integration test to create a random DOI, you manually switch between draft or publish/findable (the latter can never be deleted) (CanCreateRandomDoiIntegrationTestAsync).
- Mockup test POSTing via doiController - this can be used as an example to see how to Moq.

