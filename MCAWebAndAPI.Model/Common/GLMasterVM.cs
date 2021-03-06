﻿using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class GLMasterVM : Item
    {
        public string GLNo { get; set; }
        public string GLDescription { get; set; }

        public string GLNoDescription
        {
            get { return string.Format("{0} - {1}", GLNo, GLDescription); }

        }
    }
}