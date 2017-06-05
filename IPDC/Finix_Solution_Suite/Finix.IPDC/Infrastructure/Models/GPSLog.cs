using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("GPSLog")]
    public class GPSLog : Entity
    {
        public string IMEI { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
