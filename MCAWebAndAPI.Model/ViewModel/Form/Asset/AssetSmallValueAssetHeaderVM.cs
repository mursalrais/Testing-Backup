using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetSmallValueAssetHeaderVM
    {
        private DateTime _runDate;
             
        public DateTime RunDate
        {
            get
            {
                if (_runDate == null)
                    _runDate = new DateTime();
                return _runDate;
            }

            set
            {
                _runDate = value;
            }
        }


    }
}
