using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using ChatGptBlazorCore.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;
using Serilog;
using Serilog.Events;

namespace DataAccessLayerTests;

public class TestLogger : ILogger
{
    public TestLogger()
    {
        LogEvents = new List<LogEvent>();
    }

    public List<LogEvent> LogEvents { get; set; }

    public void Write(LogEvent logEvent)
    {
        LogEvents.Add(logEvent);
    }
}

public class TestModel : IModel<string>
{
    public string Value { get; set; }
    public string Id { get; set; }
}

public class RepositoryFileDbTests
{
    [Fact]
    public async Task Should_be_able_to_initialize_an_empty_file_db()
    {
        var mockDbFilePath = $"./{Guid.NewGuid().ToString()}_db.json";
        var testLogger = new TestLogger();

        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var targetRepo = new RepositoryFileDb<Guid, User>(fileSystem, testLogger, mockDbFilePath);

        await targetRepo.InitializeAsync();

        targetRepo.IsInitialized.Should().BeTrue();
        fileSystem.File.Exists(mockDbFilePath).Should().BeTrue();
        var items = await targetRepo.GetAllAsync();
        items.Ok.Should().BeTrue();
        items.Result.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_be_able_to_initialize_a_file_db_with_data()
    {
        var mockDbFilePath = $"./{Guid.NewGuid().ToString()}_db.json";
        var testData = new Dictionary<string, TestModel>
        {
            { "1", new TestModel { Id = "1" } },
            { "2", new TestModel { Id = "2" } },
            { "3", new TestModel { Id = "3" } }
        };
        var expectedValues = testData.Values.ToArray();
        var testLogger = new TestLogger();

        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var targetRepo = new RepositoryFileDb<string, TestModel>(fileSystem, testLogger, mockDbFilePath);
        await targetRepo.InitializeAsync(testData);

        targetRepo.IsInitialized.Should().BeTrue();
        fileSystem.File.Exists(mockDbFilePath).Should().BeTrue();
        var items = await targetRepo.GetAllAsync();
        items.Ok.Should().BeTrue();
        items.Result.Should().BeEquivalentTo(expectedValues);
    }

    [Fact]
    public async Task Should_be_able_to_add_new_item_to_file_db()
    {
        var mockDbFilePath = $"./{Guid.NewGuid().ToString()}_db.json";
        var testData = new Dictionary<string, TestModel>
        {
            { "1", new TestModel { Id = "1" } },
            { "2", new TestModel { Id = "2" } },
            { "3", new TestModel { Id = "3" } }
        };
        var expectedValues = testData.Values.ToArray();
        var testLogger = new TestLogger();

        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var targetRepo = new RepositoryFileDb<string, TestModel>(fileSystem, testLogger, mockDbFilePath);
        await targetRepo.InitializeAsync(testData);

        targetRepo.IsInitialized.Should().BeTrue();
        fileSystem.File.Exists(mockDbFilePath).Should().BeTrue();
        var items = await targetRepo.GetAllAsync();
        items.Ok.Should().BeTrue();
        items.Result.Should().BeEquivalentTo(expectedValues);

        var newItem = new TestModel { Id = "4" };
        var addResult = await targetRepo.AddAsync(newItem);
        addResult.Ok.Should().BeTrue();
        addResult.Errors.Should().BeEmpty();

        var addedItemQueryResult = await targetRepo.GetAsync(newItem.Id);
        addedItemQueryResult.Ok.Should().BeTrue();
        addedItemQueryResult.Result.Should().BeEquivalentTo(newItem);
        var dataFileText = fileSystem.File.ReadAllText(mockDbFilePath);

        var serializedItems = JsonSerializer.Deserialize<Dictionary<string, TestModel>>(dataFileText);
        serializedItems.Should().ContainKey("4");
    }

    [Fact]
    public async Task Should_be_able_to_update_item_in_file_db()
    {
        var testItemKey = "3";
        var testItemValue = "test";
        var updatesValue = "updated";
        var mockDbFilePath = $"./{Guid.NewGuid().ToString()}_db.json";
        var testData = new Dictionary<string, TestModel>
        {
            { "1", new TestModel { Id = "1" } },
            { "2", new TestModel { Id = "2" } },
            { testItemKey, new TestModel { Id = testItemKey, Value = testItemValue } }
        };
        var expectedValues = testData.Values.ToArray();
        var testLogger = new TestLogger();

        IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var targetRepo = new RepositoryFileDb<string, TestModel>(fileSystem, testLogger, mockDbFilePath);
        await targetRepo.InitializeAsync(testData);

        targetRepo.IsInitialized.Should().BeTrue();
        fileSystem.File.Exists(mockDbFilePath).Should().BeTrue();
        var getQueryResult = await targetRepo.GetAsync(testItemKey);
        getQueryResult.Ok.Should().BeTrue();
        var item = getQueryResult.Result;
        item.Value = updatesValue;
        var updateResult = await targetRepo.UpdateAsync(item);
        updateResult.Ok.Should().BeTrue();
        updateResult.Errors.Should().BeEmpty();

        var dataFileText = fileSystem.File.ReadAllText(mockDbFilePath);

        var serializedItems = JsonSerializer.Deserialize<Dictionary<string, TestModel>>(dataFileText);
        serializedItems[testItemKey].Value.Should().Be(updatesValue);
    }
}