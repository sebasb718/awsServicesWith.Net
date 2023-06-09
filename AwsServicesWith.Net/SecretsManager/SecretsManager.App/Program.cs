
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretsManagerClient = new AmazonSecretsManagerClient();

var request = new GetSecretValueRequest
{
    SecretId = "ApiKey"
};

var response = await secretsManagerClient.GetSecretValueAsync(request);

Console.Write(response.SecretString);

var describeSecretRequest = new DescribeSecretRequest
{
    SecretId = "ApiKey"
};

var describeResponse = await secretsManagerClient.DescribeSecretAsync(describeSecretRequest);

Console.WriteLine();