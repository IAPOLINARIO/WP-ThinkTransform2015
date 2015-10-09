using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WP_TT.Models
{
    public class PersonalInfo
    {
        public List<object> birthday { get; set; }
        public object building { get; set; }
        public string email { get; set; }
        public string location { get; set; }
        public string login { get; set; }
        public string manager_login { get; set; }
        public string mentor { get; set; }
        public string mentor_login { get; set; }
        public string name { get; set; }
        public object contact1 { get; set; }
        public object contact2 { get; set; }
        public object contact3 { get; set; }
        public string role { get; set; }
        public object own_words { get; set; }

        public string photo { get; set; }
    }
}
