using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuccessWPF
{
    class ProjectDates
    {
        public DateTime StartDate { get; private set; }
        public DateTime StartDateBaseline { get; private set; }
        public DateTime FinishDate { get; private set; }
        public DateTime FinishDateBaseline { get; private set; }

        public ProjectDates(DateTime start, DateTime finish)
        {
            StartDate = start;
            FinishDate = finish;
        }

        public ProjectDates(DateTime? start, DateTime? finish)
        {
            if (start.HasValue)
                StartDate = start.Value;
            if (finish.HasValue)
                FinishDate = finish.Value;
        }

        public ProjectDates(java.util.Date start, java.util.Date finish)
        {
            if (start != null && finish != null)
            {
                //StartDate = new DateTime(year: start.getYear(), month: start.getMonth(), day: start.getDay());
                //FinishDate = new DateTime(year: finish.getYear(), month: finish.getMonth(), day: finish.getDay());
                StartDate = new DateTime(1970, 1, 1, 4, 0, 0, DateTimeKind.Utc);
                FinishDate = new DateTime(1970, 1, 1, 4, 0, 0, DateTimeKind.Utc);
                StartDate = StartDate.AddMilliseconds(start.getTime());
                FinishDate = FinishDate.AddMilliseconds(finish.getTime());
            }
        }

        public void SetBaseline(DateTime start, DateTime finish)
        {
            StartDateBaseline = start;
            FinishDateBaseline = finish;
        }

        public void SetBaseline(java.util.Date bStart, java.util.Date bFinish)
        {
            //StartDateBaseline = new DateTime(year: bStart.getYear(), month: bStart.getMonth(), day: bStart.getDay());
            //FinishDateBaseline = new DateTime(year: bFinish.getYear(), month: bFinish.getMonth(), day: bFinish.getDay());
            StartDateBaseline = new DateTime(1970, 1, 1, 4, 0, 0, DateTimeKind.Utc);
            FinishDateBaseline = new DateTime(1970, 1, 1, 4, 0, 0, DateTimeKind.Utc);
            StartDateBaseline = StartDateBaseline.AddMilliseconds(bStart.getTime());
            FinishDateBaseline = FinishDateBaseline.AddMilliseconds(bFinish.getTime());
        }
    }
}
