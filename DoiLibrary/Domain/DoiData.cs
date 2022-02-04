using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DoiLibrary.Domain
{
    public class DoiData
    {
        public Data data { get; set; }
    }

    [JsonObject(Title = "data")]
    public class Data
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
        public string type { get; set; }
        [JsonProperty("attributes")]
        public Attributes attributes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Relationships relationships { get; set; } //is always set in response (Gitte don't think used in request).
    }

    [JsonObject(Title = "attributes")]
    public class Attributes
    {
        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public string _event { get; set; } //legal values: "draft" (default), "register", "publish".

        //Either 'doi' or 'prefix' must be set, but not both:
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string doi { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prefix { get; set; }
        public string url { get; set; } //MANDATORY for publish
        public Types types { get; set; } //MANDATORY for publish, sub-field "ResourceType" 
        public Creator[] creators { get; set; } //MANDATORY for publish
        public DoiTitle[] titles { get; set; } //MANDATORY for publish
        public string publisher { get; set; } //MANDATORY for publish
        public int publicationYear { get; set; } //MANDATORY for publish
        public string language { get; set; } //OPTIONAL
        public Container container { get; set; }
        public Subject[] subjects { get; set; } //RECOMMENDED
        public Contributor[] contributors { get; set; } //RECOMMENDED
        public DoiDate[] dates { get; set; } //RECOMMENDED, sub-property
        public object[] identifiers { get; set; }
        public string[] sizes { get; set; }
        public string[] formats { get; set; }
        public Rightslist[] rightsList { get; set; }
        public Description[] descriptions { get; set; } //RECOMMENDED
        public Geolocation[] geoLocations { get; set; } //RECOMMENDED
        public object[] fundingReferences { get; set; }
        public Relatedidentifier[] relatedIdentifiers { get; set; } //RECOMMENDED
        public object[] relatedItems { get; set; }
        public string schemaVersion { get; set; }
        public string providerId { get; set; }
        public string clientId { get; set; }
        public string agency { get; set; }
        public string source { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool isActive { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; } //only for response, not request!
        public object reason { get; set; }
        public object landingPage { get; set; }
    }

    public class Types
    {
        public string ris { get; set; }
        public string bibtex { get; set; }
        public string citeproc { get; set; }
        public string schemaOrg { get; set; }
        public string resourceType { get; set; } //MANADATORY for publish
        public string resourceTypeGeneral { get; set; }
    }

    public class Container
    {
    }

    public class Creator
    {
        public string name { get; set; } //Only MANDATORY
        public string givenName { get; set; }
        public string familyName { get; set; }
        public Nameidentifier[] nameIdentifiers { get; set; }
        public string nameType { get; set; }
        public Affiliation[] affiliation { get; set; }
    }

    public class Nameidentifier
    {
        public string schemeUri { get; set; }
        public string nameIdentifier { get; set; }
        public string nameIdentifierScheme { get; set; }
    }

    public class Affiliation
    {
        public string name { get; set; }
    }

    [JsonObject(Title = "Title")]
    public class DoiTitle
    {
        [JsonProperty("title")]
        public string TitleText { get; set; } //MANDATORY
        public string Lang { get; set; }
        public string TitleType { get; set; } //Allowed values (Enum): AlternativeTitle, Subtitle, TranslatedTitle, Other
    }

    public class Subject
    {
        public string subject { get; set; }
        public string subjectScheme { get; set; }
        public string lang { get; set; }
    }

    public class Contributor
    {
        public string name { get; set; }
        public string nameType { get; set; }
        public string givenName { get; set; }
        public string familyName { get; set; }
        public Affiliation1[] affiliation { get; set; }
        public string contributorType { get; set; }
    }

    public class Affiliation1
    {
        public string name { get; set; }
    }

    [JsonObject(Title = "Date")]
    public class DoiDate
    {
        public string date { get; set; }
        public string dateType { get; set; }
    }

    public class Rightslist
    {
        public string rights { get; set; }
        public string rightsUri { get; set; }
        public string schemeUri { get; set; }
        public string rightsIdentifier { get; set; }
        public string rightsIdentifierScheme { get; set; }
    }

    public class Description
    {
        public string description { get; set; }
        public string descriptionType { get; set; }
    }

    public class Geolocation
    {
        public Geolocationpoint geoLocationPoint { get; set; }
        public string geoLocationPlace { get; set; }
        public Geolocationbox geoLocationBox { get; set; }
    }

    public class Geolocationpoint
    {
        public string pointLatitude { get; set; }
        public string pointLongitude { get; set; }
    }

    public class Geolocationbox
    {
        public string eastBoundLongitude { get; set; }
        public string northBoundLatitude { get; set; }
        public string southBoundLatitude { get; set; }
        public string westBoundLongitude { get; set; }
    }

    public class Relatedidentifier
    {
        public string relationType { get; set; }
        public string relatedIdentifier { get; set; }
        public string relatedIdentifierType { get; set; }
    }

    public class Relationships
    {
        public Client client { get; set; }
        public Provider provider { get; set; }
        public Media media { get; set; }
        public References references { get; set; }
        public Citations citations { get; set; }
        public Parts parts { get; set; }
        public Partof partOf { get; set; }
        public Versions versions { get; set; }
        public Versionof versionOf { get; set; }
    }

    public class Client
    {
        public Data1 data { get; set; }
    }

    public class Data1
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Provider
    {
        public Data2 data { get; set; }
    }

    public class Data2
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Media
    {
        public Data3 data { get; set; }
    }

    public class Data3
    {
        public object id { get; set; }
        public string type { get; set; }
    }

    public class References
    {
        public object[] data { get; set; }
    }

    public class Citations
    {
        public object[] data { get; set; }
    }

    public class Parts
    {
        public object[] data { get; set; }
    }

    public class Partof
    {
        public object[] data { get; set; }
    }

    public class Versions
    {
        public object[] data { get; set; }
    }

    public class Versionof
    {
        public object[] data { get; set; }
    }
}
