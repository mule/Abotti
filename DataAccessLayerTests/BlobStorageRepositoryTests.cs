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

        var repoDataQuery = repo.GetAll();
        repoDataQuery.Result.Should().BeEmpty();
        blobClient.Exists().Value.Should().BeTrue();

        containerClient.DeleteIfExists();
    }
}