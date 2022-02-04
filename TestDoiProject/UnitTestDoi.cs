using NUnit.Framework;
using System.IO;
using DoiLibrary;
using DoiLibrary.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DoiLibrary.Interfaces;
using Doi_WebApi;
using System.Net.Http;
using System.Text;
using Moq;
using Moq.Protected;
using System.Threading;
using DoiLibrary.Services;
using Doi_WebApi.Controllers;
using Microsoft.Extensions.Logging;

namespace TestDoiProject
{
	public class UnitTestDoi
	{
		private readonly IConfiguration _configuration;
		public UnitTestDoi()
		{
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile(@"appsettings.json", false, false)
				.AddEnvironmentVariables()
				.Build();
		}

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void ResponseJsonToClassMostFieldTest()
		{
			//Assert.Pass(); 
			string fullFileName = Path.Combine("Files", "DoiData_MostFields.json");
			using (StreamReader file = File.OpenText(fullFileName))
			{
				JsonSerializer serializer = new JsonSerializer();
				DoiData responseDoi = (DoiData)serializer.Deserialize(file, typeof(DoiData));
				Assert.AreEqual(DoiDefinitions.DOI_DATA_TYPE, responseDoi.data.type);
				Assert.AreEqual("10.82081/Gitte-02-mostFields", responseDoi.data.id);
				Assert.AreEqual("PANGAEA - Data Publisher for Earth & Environmental Science", responseDoi.data.attributes.publisher);
			}
		}

		[Test]
		public void RequestClassToJsonSerializeTest()
		{
			var doiReqData = new DoiData();
			doiReqData.data = new Data();
			doiReqData.data.type = DoiDefinitions.DOI_DATA_TYPE;
			string myDoiId = "10.82081/Test-smallNumber-Fields";
			doiReqData.data.id = myDoiId;
			doiReqData.data.attributes = new Attributes();
			doiReqData.data.attributes.doi = myDoiId; //_configuration.GetSection("DoiSettings:DoiPrefix").Value;
			doiReqData.data.attributes._event = "publish";
			doiReqData.data.attributes.url = "https://doi.pangaea.de/10.1594/PANGAEA.726855";
			var jsonFromObject = JsonConvert.SerializeObject(doiReqData);
			//Remove all null-values (the end of the json string):
			string endOfUrl = "PANGAEA.726855";
			int indexOfEndUrl = jsonFromObject.IndexOf(endOfUrl) + endOfUrl.Length;
			string strippedJson = jsonFromObject.Substring(0, indexOfEndUrl);
			strippedJson += "\"}}}";

			string fullFileName = Path.Combine("Files", "ApiRequestSmall.json");
			using (StreamReader file = File.OpenText(fullFileName))
			{
				var apiReqExample = file.ReadToEnd();

				JToken expected = JToken.Parse(apiReqExample);
				JToken actual = JToken.Parse(strippedJson);
				actual.Should().BeEquivalentTo(expected);
			}
		}

        [Test]
        [Category("IntegrationTest")] //Do call remote Doi-Api
        public async Task CanCreateRandomDoiIntegrationTestAsync()
        {
            //Arrange
            // Create DoiHttpClient:
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(_configuration);

            Startup.AddDoiHttpClientToService(_configuration, services);

            var builderProvider = services.BuildServiceProvider();
            var sutHttpClient = builderProvider.GetRequiredService<IDoiHttpClient>();

            Attributes doiAttributes = new Attributes();
            doiAttributes.doi = null; //result in random DOI.
            doiAttributes.url = "https://www.dmi.dk/";

            //====================================
            //Avoid running test with event="publish" as a publication in state=publish/findable can never be deleted:
            //doiAttributes._event = "publish"; //defaults to "draft" when not set. 
            //doiAttributes._event = "draft";
            //====================================


            Creator myCreator = new Creator();
            myCreator.name = "MyCreatorName";
            //myCreator.nameType = "Personal";
            doiAttributes.creators = new Creator[] { myCreator };

            DoiTitle myTitle = new DoiTitle();
            var now = DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss");
            myTitle.TitleText = $"UnitTestTitle-{now}";
            myTitle.Lang = "dk";
            //myTitle.TitleType = null;
            doiAttributes.titles = new DoiTitle[] { myTitle };
            doiAttributes.publisher = "my test Publisher";
            doiAttributes.publicationYear = 2021;

            Types myType = new Types
            {
                ris = "GEN",
                bibtex = "misc",
                citeproc = "article",
                schemaOrg = "Collection",
                resourceType = "Supplementary Collection of Datasets", //MANDATORY
                resourceTypeGeneral = "Collection"
            };
            doiAttributes.types = myType;


            //======Recommended fields:
            Subject mySubject = new Subject();
            mySubject.subject = "my test subject";
            mySubject.subjectScheme = "my test subject schema";
            mySubject.lang = "da";
            doiAttributes.subjects = new Subject[] { mySubject };

            Contributor myContributor = new Contributor(); //RECOMMENDED
            myContributor.name = "MyContributorName";
            myContributor.contributorType = "ContactPerson";
            doiAttributes.contributors = new Contributor[] { myContributor };

            DoiDate mydate = new DoiDate
            {
                date = "2014-08-29",
                dateType = "Created"
            };
            doiAttributes.dates = new DoiDate[] { mydate };
            Relatedidentifier myRelatedIdentifier = new Relatedidentifier();
            myRelatedIdentifier.relationType = "IsSupplementTo";
            myRelatedIdentifier.relatedIdentifier = "10.82081/gitte - 20211210 - 23 - 59 - frompostman"; //"10.2343/geochemj.34.59";
            myRelatedIdentifier.relatedIdentifierType = "DOI";
            doiAttributes.relatedIdentifiers = new Relatedidentifier[] { myRelatedIdentifier };
            Description myDescription = new Description();
            myDescription.description = "my test description";
            myDescription.descriptionType = "Abstract";
            doiAttributes.descriptions = new Description[] { myDescription };
            Geolocationpoint myGeolocationpoint = new Geolocationpoint();
            myGeolocationpoint.pointLatitude = "38.61599999999999";
            myGeolocationpoint.pointLongitude = "134.53600000000003";
            Geolocation myGeolocation = new Geolocation();
            myGeolocation.geoLocationPoint = myGeolocationpoint;
            doiAttributes.geoLocations = new Geolocation[] { myGeolocation };



            //act and assert
            string expected = "10.82081/"; //"10.82081 /prez - ge65"; 
            var result = await sutHttpClient.CreateDoi(doiAttributes);
            Assert.AreEqual(expected.Substring(0, 9), result.Substring(0, 9));
        }


        [Test]
        public async Task GetSpecificDoi_Returns_Success_MockTest()
        {
            //This is an example of a test using MockUps. This should be expanded and maintained whenever signature of Controller change!

            //https://www.thecodebuzz.com/mock-typed-httpclient-httpclientfactory-moq-net-core/
            //https://makolyte.com/csharp-how-to-unit-test-code-that-uses-httpclient/
            //Arrange =================================
            string fullFileName = Path.Combine("Files", "ApiResponseSpecific.json");
            var jsonResponseString = File.ReadAllText(fullFileName);
            HttpResponseMessage httpResp = new HttpResponseMessage();
            httpResp.StatusCode = System.Net.HttpStatusCode.OK;
            httpResp.Content = new StringContent(jsonResponseString, Encoding.UTF8, "application/json"); //new StringContent(jsonResponseString);

            Mock<HttpMessageHandler> mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResp);

            HttpClient httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(_configuration["DoiSettings:DoiDataciteServerUrl"])
                //This avoids: //System.InvalidOperationException : An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.
            };
            //https://www.code4it.dev/blog/testing-httpclientfactory-moq

            IDoiHttpClient doiClient = new DoiHttpClient(httpClient, _configuration, Mock.Of<ILogger<DoiHttpClient>>());

            var logMock = new Mock<ILogger<DoiController>>();
            ILogger<DoiController> logger = logMock.Object; //shorter: Mock.Of<ILogger<DoiController>>();

            DoiController doiController = new DoiController(doiClient, logger);
            const string DOI_ID = "10.82081/dk-sa-dda-tst-009";

            var doiAttributes = new Attributes();
            doiAttributes.doi = DOI_ID;
            doiAttributes._event = "draft";
            doiAttributes.publicationYear = 2021;
            //Doi.Domain.Data.Relationships relationships = new Doi.Domain.Data.Relationships(); //only for response, not request.


            //act =================================
            var controllerDoiResult = await doiController.PostDoi(doiAttributes);

            //assert =================================
            //string actual = controllerDoiResult.Value; //and chg Controller from return Ok(result); to return result! https://stackoverflow.com/questions/50801094/how-to-get-the-values-from-a-taskiactionresult-returned-through-an-api-for-uni/50807112
            //Assert.AreEqual(DOI_ID, controllerDoiResult.Value as MyResponseDTO), "");
            Assert.Pass("Final assert not Implemented yet!"); //TODO

            //Assert.That(actual, Is.EqualTo(expected).NoClip); //eliminate clipping of long strings https://stackoverflow.com/questions/44833280/nunit-assert-areequal-strings-diff
        }

        /*
            // Possible see: https://stackoverflow.com/questions/57091410/unable-to-mock-httpclient-postasync-in-unit-tests
            //https://adamstorr.azurewebsites.net/blog/test-your-dotnet-httpclient-based-strongly-typed-clients-like-a-boss
            //Arrange
            //var builder = new HttpRequestInterceptionBuilder()
         */
    }
}