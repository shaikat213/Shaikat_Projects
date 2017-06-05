using System;

namespace Finix.IPDC.DTO
{
    public class GPSLogDto
    {
        public string ApiKey { get; set; }
        public string IMEI { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
    }
}
