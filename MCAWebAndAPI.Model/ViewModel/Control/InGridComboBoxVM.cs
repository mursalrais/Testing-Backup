﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class InGridComboBoxVM
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string ViewDataKey { get; set; }

        public IEnumerable<InGridComboBoxVM> BindTo { get; set; }
    }
}
