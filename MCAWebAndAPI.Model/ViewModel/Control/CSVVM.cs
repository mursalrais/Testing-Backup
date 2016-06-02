using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class CSVVM
    {
        public string ListName { get; set; }

        public DataTable DataTable { get; set; } = new DataTable();

        [UIHint("HttpPostedFileBase")]
        public HttpPostedFileBase File { get; set; }
        
    }
}
