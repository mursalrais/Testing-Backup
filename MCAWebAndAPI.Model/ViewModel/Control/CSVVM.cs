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
    /// <summary>
    /// Used of combo box that retrives choices from ajax get function
    /// </summary>
    public class CSVVM
    {
        public string ListName { get; set; }
        DataTable _dataTable = new DataTable();

        [UIHint("HttpPostedFileBase")]
        public HttpPostedFileBase File { get; set; }
        

        public DataTable DataTable
        {
            get
            {
                return _dataTable;
            }

            set
            {
                _dataTable = value;
            }
        }

        
    }
}
