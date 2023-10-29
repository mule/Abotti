using Abotti.Core.QueryResults;
using FluentAssertions;
using FluentDateTime;

namespace Abotti.BlazorAppTests.ViewModelTests;

public class ChatSessionListVmTests
{
    //a date that is sunday
    public static DateTime ReferenceDate = new(2021, 9, 12);

    public static TopicQueryResult[] Topics =
    {
        //topics created on reference date
        new TopicQueryResult(Guid.NewGuid(), "Topic 1", ReferenceDate),

        //topics created reference day week
        new TopicQueryResult(Guid.NewGuid(), "Topic 3", ReferenceDate.FirstDayOfWeek()),
        new TopicQueryResult(Guid.NewGuid(), "Topic 4", ReferenceDate.AddDays(-1)),

        //topics created in the same month as reference date but on reference date week
        new TopicQueryResult(Guid.NewGuid(), "Topic 5", ReferenceDate.FirstDayOfMonth()),
        new TopicQueryResult(Guid.NewGuid(), "Topic 6", ReferenceDate.FirstDayOfMonth().AddDays(1)),

        //later topics
        new TopicQueryResult(Guid.NewGuid(), "Topic 7", ReferenceDate.FirstDayOfMonth().AddDays(-2)),
        new TopicQueryResult(Guid.NewGuid(), "Topic 8", ReferenceDate.FirstDayOfMonth().AddYears(-1))
    };


    [Fact]
    public void ChatSessionListVm_Ctor_WithNullTopics_ShouldNotThrow()
    {
        // Arrange
        TopicQueryResult[] topics = null;

        // Act
        var act = () => new ChatSessionsListVm(topics);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ChatListVm_get_referenceDates_topics_correctly()
    {
        // Arrange


        var vm = new ChatSessionsListVm(Topics);


        // Act
        var topicsOnReferenceDate = vm.GetTopicsOnDate(ReferenceDate);
        var laterTopicsOnReferenceDateWeek = vm.GetTopicsOnWeek(ReferenceDate, new[] { ReferenceDate });
        var laterTopicsOnReferenceDateMonth = vm.GetTopicsOnMonth(ReferenceDate, true);
        var laterTopics = vm.GroupTopicsByYearMonth(Topics, new[] { (ReferenceDate.Year, ReferenceDate.Month) });


        // Assert
        topicsOnReferenceDate.Should().HaveCount(1);
        laterTopicsOnReferenceDateWeek.Should().HaveCount(2);
        laterTopicsOnReferenceDateMonth.Should().HaveCount(2);
        laterTopics.Should().HaveCount(2);
    }
}