using Abotti.Core.QueryResults;
using FluentDateTime;

namespace Abotti.BlazorComponentTests.TestData;

public class ChatSessionsListData
{
    //a date that is sunday
    public static DateTime ReferenceDate = new(2021, 9, 12);

    public static TopicQueryResult[] Topics =
    {
        //topics created on reference date
        new(Guid.NewGuid(), "Topic 1", ReferenceDate),

        //topics created reference day week
        new(Guid.NewGuid(), "Topic 3", ReferenceDate.FirstDayOfWeek()),
        new(Guid.NewGuid(), "Topic 4", ReferenceDate.AddDays(-1)),

        //topics created in the same month as reference date but on reference date week
        new(Guid.NewGuid(), "Topic 5", ReferenceDate.FirstDayOfMonth()),
        new(Guid.NewGuid(), "Topic 6", ReferenceDate.FirstDayOfMonth().AddDays(1)),

        //later topics
        new(Guid.NewGuid(), "Topic 7", ReferenceDate.FirstDayOfMonth().AddDays(-2)),
        new(Guid.NewGuid(), "Topic 8", ReferenceDate.FirstDayOfMonth().AddYears(-1))
    };
}