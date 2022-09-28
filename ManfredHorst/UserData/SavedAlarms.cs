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
        }

        public String UserId { get; set; }
        public List<Urls> UrlList { get; set; }
    }
}
