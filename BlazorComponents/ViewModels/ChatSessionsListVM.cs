using Abotti.Core.QueryResults;
using FluentDateTime;

namespace Abotti.BlazorComponents.ViewModels;

public class ChatSessionsListVm
{
    public ChatSessionsListVm()
    {
        Topics = Array.Empty<TopicQueryResult>();
    }

    public ChatSessionsListVm(TopicQueryResult[] topics, Guid? selectedTopicId)
    {
        Topics = topics;
        SelectedTopicId = selectedTopicId;
    }

    public Guid? SelectedTopicId { get; set; }

    public TopicQueryResult[] Topics { get; }

    public TopicQueryResult[] GetTopicsOnDate(DateTime topicDate)
    {
        var result = Topics.Where(t => t.Created >= topicDate.Date && t.Created <= topicDate.EndOfDay()).ToArray();
        return result;
    }

    public TopicQueryResult[] GetTopicsOnWeek(DateTime referenceDate, DateTime[] excludeDates)
    {
        var topicsThisWeek = Topics
            .Where(topic =>
                topic.Created.IsAfter(referenceDate.BeginningOfWeek().AddTicks(-1))
                && topic.Created.IsBefore(referenceDate.EndOfWeek().AddTicks(1))
                && !excludeDates.Any(excludeDate => excludeDate.Date == topic.Created.Date))
            .ToArray();

        return topicsThisWeek;
    }

    public TopicQueryResult[] GetTopicsOnMonth(DateTime referenceDate, bool excludeRefDateWeek)
    {
        var topicsThisMonth = Topics
            .Where(topic =>
                topic.Created.IsAfter(referenceDate.PreviousMonth().EndOfMonth())
                && topic.Created.IsBefore(referenceDate.EndOfMonth()));

        if (excludeRefDateWeek)
            topicsThisMonth = topicsThisMonth.Where(topic =>
                topic.Created.IsBefore(referenceDate.FirstDayOfWeek())
                || topic.Created.IsAfter(referenceDate.EndOfWeek()));

        return topicsThisMonth.ToArray();
    }


    public (int year, int month, TopicQueryResult[])[] GroupTopicsByYearMonth((int year, int month)[] excludeYearMonths)
    {
        var result = Topics
            .GroupBy(topic => new { topic.Created.Year, topic.Created.Month })
            .Where(g => !excludeYearMonths.Any(eym => eym.year == g.Key.Year && eym.month == g.Key.Month))
            .Select(g => (g.Key.Year, g.Key.Month, g.ToArray()))
            .ToArray();

        return result;
    }
}