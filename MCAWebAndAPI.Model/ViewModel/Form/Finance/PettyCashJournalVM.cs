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
        [DataType(DataType.Date)]
        [DisplayName("Date (from)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateFrom { get; set; } = DateTime.Today.AddDays(-14);

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Date (to)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateTo { get; set; } = DateTime.Today;

        [UIHint("Decimal")]
        [DisplayName("Total amount to be replenished")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal TotalAmount { get; set; } = 0;

        [UIHint("Decimal")]
        [DisplayName("Advances 1")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal Advances1 { get; set; }

        [UIHint("Decimal")]
        [DisplayName("Advances 2")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal Advances2 { get; set; }

        [UIHint("Decimal")]
        [DisplayName("Advances 3")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal Advances3 { get; set; }

        [UIHint("Decimal")]
        [DisplayName("Cash on hand")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal CashOnHand { get; set; } = 0;

        [UIHint("Decimal")]
        [DisplayName("Total petty cash fund")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal TotalPettyCashFund { get; } = 10000000;

        public IEnumerable<PettyCashJournalItemVM> ItemDetails { get; set; } = new List<PettyCashJournalItemVM>();

        public bool ItemEdited { get; set; } = false;
    }
}
