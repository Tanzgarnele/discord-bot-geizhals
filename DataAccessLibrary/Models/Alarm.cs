using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class Alarm
    {
        public String Url { get; set; }
        public String Alias { get; set; }
        public Double Price { get; set; }
        public Int64 UserId { get; set; }
    }
}
