

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models
{
    public class ActivityModel
    {
        public ActivityModel(string lineOne, string lineTwo, string url, string dayText)
        {
            LineOne = lineOne;
            LineTwo = lineTwo;
            Url = url;
            DayText = dayText;
        }

        public string LineOne { get; }

        public string LineTwo { get; }

        public string Url { get; }

        public string DayText { get; }
    }
}