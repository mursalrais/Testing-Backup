using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Utils
{
    public class CamlQueryUtil
    {
        public static string Generate(string fieldRefName, string valueType, string value)
        {
            return string.Format("<View><Query><Where><Eq><FieldRef Name='{0}' /><Value Type='{1}'>{2}</Value></Eq></Where></Query></View>", fieldRefName, valueType, value);
        }
    }
}
