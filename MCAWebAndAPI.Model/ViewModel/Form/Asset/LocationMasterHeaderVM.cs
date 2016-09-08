using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class LocationMasterHeaderVM
    {
        public string OfficeName { get; set; }

        public int FloorName { get; set; }

        public string RoomName { get; set; }

        public string Remarks { get; set; }
    }
}
