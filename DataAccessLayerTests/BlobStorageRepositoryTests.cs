using System.ComponentModel;
using System.Text.Json;
using Azure.Storage.Blobs;
using ChatGptBlazorCore.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;

namespace DataAccessLayerTests;

public class BlobStorageRepositoryTests
{
    private static readonly string AzuriteConStr =
        "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";

    [Fact]
    [Category("AzuriteTest")]
    public void Should_be_able_to_initialize_an_empty_blob_storage_repository()
    {
        var logger = new TestLogger();

        var testContainerName = $"{Guid.NewGuid().ToString()}-testcontainer";
        var testBlobName = $"{Guid.NewGuid().ToString()}-testdb.json";
        var containerClient = new BlobContainerClient(AzuriteConStr, testContainerName);
        containerClient.CreateIfNotExists();
        var blobClient = containerClient.GetBlobClient(testBlobName);
        var repo = new BlobStorageRepository<Guid, User>(blobClient, logger);

        var action = () => repo.Initialize();
        action.Should().NotThrow();
        repo.IsInitialized.Should().BeTrue();

        var repoDataQuery = repo.GetAll();
        repoDataQuery.Result.Should().BeEmpty();
        blobClient.Exists().Value.Should().BeTrue();

        containerClient.DeleteIfExists();
    }

    [Fact]
    [Category("AzuriteTest")]
    public async Task Should_be_able_to_initialize_an_empty_blob_storage_repository_async()
    {
        var logger = new TestLogger();

        var testContainerName = $"{Guid.NewGuid().ToString()}-testcontainer";
        var testBlobName = $"{Guid.NewGuid().ToString()}-testdb.json";
        var containerClient = new BlobContainerClient(AzuriteConStr, testContainerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(testBlobName);
        var repo = new BlobStorageRepository<Guid, User>(blobClient, logger);

        var action = async () => await repo.InitializeAsync();
        await action.Should().NotThrowAsync();
        repo.IsInitialized.Should().BeTrue();

        var repoDataQuery = await repo.GetAllAsync();
        repoDataQuery.Result.Should().BeEmpty();
        blobClient.Exists().Value.Should().BeTrue();

        await containerClient.DeleteIfExistsAsync();
    }

    [Fact]
    [Category("AzuriteTest")]
    public void Should_be_able_to_initialize_a_blob_storage_repository_with_data()
    {
        var logger = new TestLogger();

        var testContainerName = $"{Guid.NewGuid().ToString()}-testcontainer";
        var testBlobName = $"{Guid.NewGuid().ToString()}-testdb.json";
        var containerClient = new BlobContainerClient(AzuriteConStr, testContainerName);
        containerClient.CreateIfNotExists();
        var blobClient = containerClient.GetBlobClient(testBlobName);
        var testData = new Dictionary<string, TestModel>
        {
            { "1", new TestModel { Id = "1" } },
            { "2", new TestModel { Id = "2" } },
            { "3", new TestModel { Id = "3" } }
        };
        var expectedValues = testData.Values.ToArray();
        var repo = new BlobStorageRepository<string, TestModel>(blobClient, logger);

        var action = () => repo.Initialize(testData);
        action.Should().NotThrow();
        repo.IsInitialized.Should().BeTrue();

        var repoDataQuery = repo.GetAll();
        repoDataQuery.Result.Should().BeEquivalentTo(expectedValues);
        blobClient.Exists().Value.Should().BeTrue();

        containerClient.DeleteIfExists();
    }

    [Fact]
    [Category("AzuriteTest")]
    public void Should_be_able_to_add_item_to_repo()
    {
        var logger = new TestLogger();
        var testData = new TestModel
        {
            Id = "TestId",
            Value = "TestValue"
        };

        var testContainerName = $"{Guid.NewGuid().ToString()}-testcontainer";
        var testBlobName = $"{Guid.NewGuid().ToString()}-testdb.json";
        var containerClient = new BlobContainerClient(AzuriteConStr, testContainerName);
        containerClient.CreateIfNotExists();
        var blobClient = containerClient.GetBlobClient(testBlobName);

        var repo = new BlobStorageRepository<string, TestModel>(blobClient, logger);
        repo.Initialize();

        repo.Add(testData);

        var repoDataQuery = repo.Get(testData.Id);
        repoDataQuery.Result.Should().BeEquivalentTo(testData);
        blobClient.Exists().Value.Should().BeTrue();
        var blobData = blobClient.DownloadContent();

        var persistedData =
            JsonSerializer.Deserialize<Dictionary<string, TestModel>>(blobData.Value.Content.ToString());

        persistedData.Should().ContainKey(testData.Id);
        persistedData[testData.Id].Should().BeEquivalentTo(testData);
        containerClient.DeleteIfExists();
    }

    [Fact]
    [Category("AzuriteTest")]
    public async Task Should_be_able_to_add_item_to_repo_async()
    {
        var logger = new TestLogger();
        var testData = new TestModel
        {
            Id = "TestId",
            Value = "TestValue"
        };

        var testContainerName = $"{Guid.NewGuid().ToString()}-testcontainer";
        var testBlobName = $"{Guid.NewGuid().ToString()}-testdb.json";
        var containerClient = new BlobContainerClient(AzuriteConStr, testContainerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(testBlobName);

        var repo = new BlobStorageRepository<string, TestModel>(blobClient, logger);
        await repo.InitializeAsync();

        repo.AddAsync(testData).Wait();

        var repoDataQuery = await repo.GetAsync(testData.Id);
        repoDataQuery.Ok.Should().BeTrue();
        (await blobClient.ExistsAsync()).Value.Should().BeTrue();
        var blobData = blobClient.DownloadContentAsync().Result;

        var persistedData =
            JsonSerializer.Deserialize<Dictionary<string, TestModel>>(blobData.Value.Content.ToString());

        persistedData.Should().ContainKey(testData.Id);
        persistedData[testData.Id].Should().BeEquivalentTo(testData);
        containerClient.DeleteIfExists();
    }
}