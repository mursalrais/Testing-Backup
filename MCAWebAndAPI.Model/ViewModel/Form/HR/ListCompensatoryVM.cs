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
    public class ListCompensatoryVM : Item
    {
        public IEnumerable<CompensatoryDetailVM> listCompensatoryDetails { get; set; } = new List<CompensatoryDetailVM>();

        /// <head>
        /// CompensatoryID
        /// </head>
        /// 
        [DisplayName("CompID")]
        public int? CmpID { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Name")]
        public string CmpName { get; set; }

        /// <head>
        /// CompensatoryInitial
        /// </head>
        /// 
        [DisplayName("Position")]
        public string CmpPosition { get; set; }

        /// <head>
        /// CompensatoryPosition
        /// </head>
        /// 
        [DisplayName("Project/Unit")]
        public string CmpProjUnit { get; set; }

        /// <head>
        /// CompensatoryYearDate
        /// <head>
        [DisplayName("YearDate")]
        public string CmpYearDate { get; set; }

    }
}
