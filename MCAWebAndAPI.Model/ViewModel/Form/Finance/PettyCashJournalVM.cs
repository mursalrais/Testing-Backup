using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class PettyCashJournalVM : Item
    {
        /// <summary>
        /// Wireframe FIN13: Petty Cash Journal
        /// </summary>
        public Operations Operation { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Date (from)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm}")]
        public DateTime DateFrom { get; set; } = DateTime.Today.AddDays(-14);

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("Date (to)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm}")]
        public DateTime DateTo { get; set; } = DateTime.Today;
        
        [DisplayName("Total amount to be replenished")]
        public decimal TotalAmount { get; set; } = 0;
        
        [DisplayName("Advances 1")]
        public decimal Advances1 { get; set; }
        
        [DisplayName("Advances 2")]
        public decimal Advances2 { get; set; }
        
        [DisplayName("Advances 3")]
        public decimal Advances3 { get; set; }
        
        [DisplayName("Cash on hand")]
        public decimal CashOnHand { get; set; } = 0;
        
        [DisplayName("Total petty cash fund")]
        public decimal TotalPettyCashFund { get; } = 10000000;

        public IEnumerable<PettyCashJournalItemVM> ItemDetails { get; set; } = new List<PettyCashJournalItemVM>();

        public bool ItemEdited { get; set; } = false;
    }
}
