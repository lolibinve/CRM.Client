using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CRM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class CountryCodeMode
    {
        public string Code { get; set; }

        public string Country { get; set; }

        public string Country_Code 
        { 
            get { return this.Country + " " + this.Code; }
        }

    }
}
