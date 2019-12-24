using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KDays.Data
{
    [Table("Days")]
    public class Days
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string KDStart { get; set; }
    }
}
