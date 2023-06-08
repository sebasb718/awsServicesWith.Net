﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Transform;
using Customers.Api.Contracts.Data;
using System.Text.Json;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "customers";

    public CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttribute,
            ConditionExpression = "attribute_not_exists(pk)"
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue{ S = id.ToString() } },
                { "sk", new AttributeValue{ S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(getItemRequest);
        if (response.Item.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        var queryRequest = new QueryRequest
        {
            TableName = _tableName,
            IndexName = "email-id-index",
            KeyConditionExpression = "Email = :v_Email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {
                    ":v_Email", new AttributeValue{ S = email }
                }
            }
        };

        var response = await _dynamoDb.QueryAsync(queryRequest);
        if (response.Items.Count == 0)
        {
            return null;
        }

        var itemAsDocument = Document.FromAttributeMap(response.Items[0]);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var scanRequest = new ScanRequest
        {
            TableName = _tableName,
        };
        var response = await _dynamoDb.ScanAsync(scanRequest);
        return response.Items.Select(x =>
        {
            var json = Document.FromAttributeMap(x).ToJson();
            return JsonSerializer.Deserialize<CustomerDto>(json);
        });
    }

    public async Task<bool> UpdateAsync(CustomerDto customer, DateTime requestStarted)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var updateItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = customerAsAttribute, 
            ConditionExpression = "UpdatedAt < :requestStarted",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":requestStarted", new AttributeValue{ S = requestStarted.ToString("O") } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deletedItemRequest = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "pk", new AttributeValue{ S = id.ToString() } },
                { "sk", new AttributeValue{ S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deletedItemRequest);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}