using ImageClassificationAPI_v1.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ImageClassificationAPI_v1.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("api/get")]
        public IHttpActionResult Get(string value)
        {
            return Ok(value);
        }

        [HttpPost]
        [Route("api/imageresponse")]
        public IHttpActionResult GetImageResponse(ImageResponseModel objectvalue)
        {
            //  return Ok(objectvalue.predictions[0].tagName.ToString());

          




            var json = new JavaScriptSerializer().Serialize(objectvalue);

            string connectionString =
                @"mongodb://salesapp-pwcindia:hS8AwanDu5PhM1eMi1Om3CB7Aj2DLTHKuyHOgy9OHEI9WGHMv0VsGB9msVzRXOUjJh89ZYg3767hwOlKf9Y8qw==@salesapp-pwcindia.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(connectionString)
            );
            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            settings.MaxConnectionIdleTime = TimeSpan.FromSeconds(40);
            var mongoClient = new MongoClient(settings);

            IMongoDatabase db = mongoClient.GetDatabase("SalesApp");

            var imageDetails = db.GetCollection<BsonDocument>("DealerAssessment");

            BsonElement imageElement = new BsonElement("Dealer ID", "A003");
            BsonElement imageResponseData = new BsonElement("Response Data", json.ToString());

            BsonDocument imageDoc = new BsonDocument
            {
                imageElement,
                imageResponseData
            };

            imageDetails.InsertOne(imageDoc);

            int Total = objectvalue.predictions.Select(x => x.tagId.Count()).Count();

            int TagCount = objectvalue.predictions.Where(x => x.tagName == "CocaCola")
                           .Select(x => x.tagId.Count()).Count();

            decimal result = Math.Round((decimal)TagCount / Total * 100, 2);




            return Ok(result.ToString());



        }
    }
}
