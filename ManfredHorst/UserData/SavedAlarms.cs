using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManfredHorst.UserData
{
    public class SavedAlarms
    {
        public SavedAlarms()
        {
            UserId = String.Empty;
            UrlList = new List<Urls>();
            Price = 0;
        }

        public String UserId { get; set; }
        public List<Urls> UrlList { get; set; }
        public Double Price { get; set; }
    }
}
