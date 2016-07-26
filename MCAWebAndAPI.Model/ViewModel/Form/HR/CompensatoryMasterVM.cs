using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class CompensatoryMasterVM : Item
    {
        /// <summary>
        /// Title
        /// </summary> 
        public string CompensatoryTitle { get; set; }

        /// <summary>
        /// positionstatus
        /// </summary>
        public int? CompensatoryID { get; set; }

        /// <summary>
        /// crstatus
        /// </summary> 
        public string CompensatoryStatus { get; set; }

        /// <summary>
        /// CompensatoryDate
        /// </summary>
        /// 
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        public DateTime? CompensatoryDate { get; set; }
    }
}
