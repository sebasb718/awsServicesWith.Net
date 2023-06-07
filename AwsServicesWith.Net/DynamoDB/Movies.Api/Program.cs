using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Movies.Api;
using System.Text.Json;

//await new DataSeeder().ImportDataAsync();

var movie1 = new MovieYearTitle
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2022,
    RottenTomatoesPercentage = 85
};

var movie2 = new MovieTitleRotten
{
    Id = Guid.NewGuid(),
    Title = "21 Jump Street",
    AgeRestriction = 18,
    ReleaseYear = 2022,
    RottenTomatoesPercentage = 85
};

var asJson1 = JsonSerializer.Serialize(movie1);
var attributeMap1 = Document.FromJson(asJson1).ToAttributeMap();

var asJson2 = JsonSerializer.Serialize(movie2);
var attributeMap2 = Document.FromJson(asJson2).ToAttributeMap();

var transactionRequest = new TransactWriteItemsRequest
{
    TransactItems = new List<TransactWriteItem>
    {
        new()
        {
            Put = new Put
            {
                TableName = "movies-year-title",
                Item = attributeMap1
            }
        },
        new()
        {
            Put = new Put
            {
                TableName = "movies-title-rotten",
                Item = attributeMap2
            }
        }
    }
};

var dynamoDb = new AmazonDynamoDBClient();

var response = await dynamoDb.TransactWriteItemsAsync(transactionRequest);


