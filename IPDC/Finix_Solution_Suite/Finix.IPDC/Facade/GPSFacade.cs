using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Facade
{
    public class GPSFacade : BaseFacade
    {
        public void SaveGPSInfo(GPSLogDto dto)
        {
            var entity = new GPSLog();
            entity = Mapper.Map<GPSLog>(dto);
            entity.CreateDate = DateTime.Now;
            GenService.Save(entity);
        }

        public GPSLogDto LoadGPSInfo(string IMEI)
        {
            var response = new GPSLogDto();
            if (!string.IsNullOrEmpty(IMEI))
            {
                var log = GenService.GetAll<GPSLog>().Where(g=>g.IMEI == IMEI).OrderByDescending(g=>g.Id).FirstOrDefault();
                if(log != null)
                {
                    response.Longitude = log.Longitude;
                    response.Latitude = log.Latitude;
                    
                    var timeDifference = DateTime.Now - ((DateTime)log.CreateDate);
                    var timeDifferenceInDays = timeDifference.TotalDays;
                    int diff = 0;
                    string timeline = "";
                    if(timeDifferenceInDays > 1)
                    {
                        if((timeDifferenceInDays/365) > 1)
                        {
                            diff = (int)(timeDifferenceInDays / 365);
                            timeline = "year";
                        }
                        else if((timeDifferenceInDays/30) > 1)
                        {
                            diff = (int)(timeDifferenceInDays / 30);
                            timeline = "month";
                        }
                        //else if((timeDifferenceInDays/7) > 1)
                        //{
                        //    diff = (int)(timeDifferenceInDays / 7);
                        //    timeline = "week";
                        //}
                        else
                        {
                            diff = (int)(timeDifferenceInDays);
                            timeline = "day";
                        }
                    }
                    else if(timeDifference.TotalHours > 1)
                    {
                        diff = (int)(timeDifference.TotalHours);
                        timeline = "hour";
                    }
                    else if(timeDifference.TotalMinutes > 1)
                    {
                        diff = (int)(timeDifference.TotalMinutes);
                        timeline = "minute";
                    }
                    

                    if(diff > 0)
                    {
                        response.Time = diff + " " + timeline;
                        if (diff > 1)
                            response.Time += "s";
                        response.Time += " ago";

                    }
                    else
                    {
                        response.Time = "Just a few moments ago";
                    }
                }
            }


            return response;
        }


    }
}
