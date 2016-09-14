using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class WBSMapping : Item
    {
        string _WBSID;
        string _WBSDescription;
        string _SubActivity;
        string _Activity;
        string _Project;

        public string WBSID
        {
            get
            {
                return _WBSID;
            }

            set
            {
                _WBSID = value;
            }
        }

        public string WBSDescription
        {
            get
            {
                return _WBSDescription;
            }

            set
            {
                _WBSDescription = value;
            }
        }

        public string SubActivity
        {
            get
            {
                return _SubActivity;
            }

            set
            {
                _SubActivity = value;
            }
        }

        public string Activity
        {
            get
            {
                return _Activity;
            }

            set
            {
                _Activity = value;
            }
        }

        public string Project
        {
            get
            {
                return _Project;
            }

            set
            {
                _Project = value;
            }
        }

        public string WBSIDDescription
        {
            get
            {
                return string.Format("{0}-{1}", this.WBSID, this.WBSDescription);
            }
        }
    }
}
