
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretsManagerClient = new AmazonSecretsManagerClient();

var listSecretVersionsRequest = new ListSecretVersionIdsRequest
{
    SecretId = "ApiKey",
    IncludeDeprecated = true
};

var versionResponse = await secretsManagerClient.ListSecretVersionIdsAsync(listSecretVersionsRequest);

var request = new GetSecretValueRequest
{
    SecretId = "ApiKey",
    VersionId = "269ab378-d54d-4dfa-9c94-b037fc3991c1"
};

var response = await secretsManagerClient.GetSecretValueAsync(request);

Console.Write(response.SecretString);

var describeSecretRequest = new DescribeSecretRequest
{
    SecretId = "ApiKey"
};

var describeResponse = await secretsManagerClient.DescribeSecretAsync(describeSecretRequest);

Console.WriteLine();