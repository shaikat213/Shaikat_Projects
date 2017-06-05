using Finix.IPDC.Infrastructure;
using System;

namespace Finix.IPDC.DTO
{
    public class DateRangeDto
    {
        public DateRangeDto(TimeLine timeLine)
        {
            DateTime now = DateTime.Now.Date;
            DateTime thisDayEnd = now.AddDays(1).AddMilliseconds(-3);
            DateTime MTDFromDate = new DateTime(now.Year, now.Month, 1);
            DateTime MTDToDate = MTDFromDate.AddDays(now.Day - 1);
            DateTime LMTDFromDate = MTDFromDate.AddMonths(-1);
            DateTime LMTDToDate = LMTDFromDate.AddDays(now.Day - 1);



            //DateTime QTDFromDate =  new DateTime();
            MTDToDate = MTDToDate.AddDays(1).AddMilliseconds(-3);
            //MTDFromDate = MTDFromDate.AddDays(1).AddMilliseconds(-3);
            LMTDToDate = LMTDToDate.AddDays(1).AddMilliseconds(-3);
            //LMTDFromDate = LMTDFromDate.AddDays(1).AddMilliseconds(-3);

            switch (timeLine)
            {
                case TimeLine.Yesterday:
                    {
                        FromDate = now.AddDays(-1);
                        ToDate = thisDayEnd.AddDays(-1);
                        break;
                    }
                case TimeLine.MTD:
                    {
                        FromDate = MTDFromDate;
                        ToDate = MTDToDate;
                        break;
                    }
                case TimeLine.LMTD:
                    {
                        FromDate = LMTDFromDate;
                        ToDate = LMTDToDate;
                        break;
                    }
                case TimeLine.QTD:
                    {
                        int quarter = (int)Math.Ceiling((decimal)now.Month / 3);
                        int month = (3 * (quarter - 1)) + 1;
                        FromDate = new DateTime(now.Year, month, 1);
                        ToDate = FromDate.AddDays(now.Day - 1);
                        break;
                    }
                case TimeLine.LYQTD:
                    {
                        int quarter = (int)Math.Ceiling((decimal)now.Month / 3);
                        int month = (3 * (quarter - 1)) + 1;
                        FromDate = new DateTime(now.Year - 1, month, 1);
                        ToDate = FromDate.AddDays(now.Day - 1);
                        break;
                    }
                case TimeLine.YTD:
                    {
                        FromDate = new DateTime(now.Year, 1, 1);
                        ToDate = FromDate.AddDays(now.Day - 1);
                        break;
                    }
                case TimeLine.LYTD:
                    {
                        FromDate = new DateTime(now.Year - 1, 1, 1);
                        ToDate = FromDate.AddDays(now.Day - 1);
                        break;
                    }
                default:
                    {
                        FromDate = now;
                        ToDate = thisDayEnd;
                        break;
                    }
            }
        }
        public DateRangeDto()
        {
            FromDate = DateTime.Now.Date;
            ToDate = FromDate.AddDays(1).AddMilliseconds(-3);
        }

        public DateTime FromDate;
        public DateTime ToDate;
    }
}
