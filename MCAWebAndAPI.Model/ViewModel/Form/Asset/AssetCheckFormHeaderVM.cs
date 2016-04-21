using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckFormHeaderVM
    {
        private DateTime _createDate;
             
        public DateTime CreateDate
        {
            get
            {
                if (_createDate == null)
                    _createDate = new DateTime();
                return _createDate;
            }

            set
            {
                _createDate = value;
            }
        }


    }
}
