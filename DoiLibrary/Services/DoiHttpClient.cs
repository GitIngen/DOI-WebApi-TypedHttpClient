using DoiLibrary.Domain;
using DoiLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DoiLibrary.Services
{
    //'DoiClient' is a named & typed HttpClient (https://www.stevejgordon.co.uk/httpclientfactory-named-typed-clients-aspnetcore).
    public class DoiHttpClient : IDoiHttpClient
    {
        private readonly HttpClient _client;
        private readonly string DoiNumberPrefix;
        private readonly ILogger _logger;
        public DoiHttpClient(HttpClient client, IConfiguration _configuration, ILogger<DoiHttpClient> logger)
        {
            _client = client;
            DoiNumberPrefix = _configuration.GetSection("DoiSettings:DoiPrefix").Value;
            _logger = logger;
        }

        /// <summary>
        /// Create a DOI at DataCite.
        /// A specific DOI is created when doi-number is set in parameters, else a random number is created.
        /// Set doiAttributes._event to decide whether it is created as a 'draft' or findable/'publish'.
        /// </summary>
        /// <param name="doiAttributes"></param>
        /// <returns></returns>
        public async Task<string> CreateDoi(Attributes doiAttributes)
        {
            DoiData doiData = new DoiData();
            doiData.data = new Data();
            doiData.data.type = DoiDefinitions.DOI_DATA_TYPE;
            doiData.data.attributes = doiAttributes;

            if (string.IsNullOrEmpty(doiAttributes.doi))
            {
                doiData.data.attributes.prefix = DoiNumberPrefix;
                doiData.data.id = null;
            }
            else
            {
                int indexOfSlash = doiData.data.attributes.doi.IndexOf("/");
                if (indexOfSlash != DoiNumberPrefix.Length
                    || !(doiData.data.attributes.doi.Substring(0, DoiNumberPrefix.Length).Equals(DoiNumberPrefix))
                    )
                {
                    string errorMessage = $"Invalid DOI={doiAttributes.doi} requested. Wrong prefix. Expected={DoiNumberPrefix}.";
                    _logger.LogError(errorMessage);
                    throw new FormatException(errorMessage);
                }

                doiData.data.attributes.prefix = null;
                doiData.data.id = doiData.data.attributes.doi;
            }

            var json = JsonConvert.SerializeObject(doiData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("", data);

            response.EnsureSuccessStatusCode(); //throw exception if HttpResponseMessage.IsSuccessStatusCode is false - but then we don't  see it.

            var responseDoi = await response.Content.ReadFromJsonAsync<DoiData>();
            _logger.LogDebug($"ResponseDoi from external Api-Server: " + JsonConvert.SerializeObject(responseDoi));
            var createdDoi = responseDoi?.data?.id;
            if (createdDoi != null)
            {
                return createdDoi;
            }
            else
            {
                string errorMessage = $"Remote Doi-Api-Server returned unexpected format. {(responseDoi == null ? "ResponseDoi is null" : "ResponseDoi.data or it's id is null")}.";
                _logger.LogError(errorMessage);
                throw new FormatException(errorMessage);
            }
        }
    }
}
