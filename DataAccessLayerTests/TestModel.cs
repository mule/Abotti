using ChatGptBlazorCore.Models;

namespace DataAccessLayerTests;

public class TestModel : IModel<string>
{
    public string Value { get; set; }
    public string Id { get; set; }
}