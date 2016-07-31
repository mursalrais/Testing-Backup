using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class LocationMasterVM : PlaceMasterVM
    {
        public IEnumerable<PlaceMasterVM> PlaceMasters { get; set; } = new List<PlaceMasterVM>();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Province { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetLocationMaster",
            ControllerName = "ASSLocationMaster",
            ValueField = "ID",
            TextField = "Title",
        };

        public string OfficeName { get; set; }

        public int FloorName { get; set; }

        public string RoomName { get; set; }

        public string Remarks { get; set; }
    }
}
