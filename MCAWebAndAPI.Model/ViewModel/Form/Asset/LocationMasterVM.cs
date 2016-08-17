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
        public string CancelUrl { get; set; }

        public IEnumerable<PlaceMasterVM> PlaceMasters { get; set; } = new List<PlaceMasterVM>();

        [UIHint("ComboBox")]
        public ComboBoxVM Province { get; set; } = new ComboBoxVM();
  

        public string OfficeName { get; set; }

        public int FloorName { get; set; }

        public string RoomName { get; set; }

        public string Remarks { get; set; }

        public string Update { get; set; }
    }
}
