using Abotti.BlazorComponents.ViewModels;
using Abotti.BlazorComponentTests.TestData;
using Abotti.Core.QueryResults;
using FluentAssertions;

namespace Abotti.BlazorAppTests.ViewModelTests;

public class ChatSessionListVmTests
{
    [Fact]
    public void ChatSessionListVm_Ctor_WithNullTopics_ShouldNotThrow()
    {
        // Arrange
        TopicQueryResult[] topics = null;
        Guid? selectedTopicId = null;

        // Act
        var act = () => new ChatSessionsListVm(topics, selectedTopicId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ChatListVm_get_referenceDates_topics_correctly()
    {
        // Arrange


        var vm = new ChatSessionsListVm(ChatSessionsListData.Topics, null);


        // Act
        var topicsOnReferenceDate = vm.GetTopicsOnDate(ChatSessionsListData.ReferenceDate);
        var laterTopicsOnReferenceDateWeek = vm.GetTopicsOnWeek(ChatSessionsListData.ReferenceDate,
            new[] { ChatSessionsListData.ReferenceDate });
        var laterTopicsOnReferenceDateMonth = vm.GetTopicsOnMonth(ChatSessionsListData.ReferenceDate, true);
        var laterTopics = vm.GroupTopicsByYearMonth(new[]
            { (ChatSessionsListData.ReferenceDate.Year, ChatSessionsListData.ReferenceDate.Month) });


        // Assert
        topicsOnReferenceDate.Should().HaveCount(1);
        laterTopicsOnReferenceDateWeek.Should().HaveCount(2);
        laterTopicsOnReferenceDateMonth.Should().HaveCount(2);
        laterTopics.Should().HaveCount(2);
    }
}