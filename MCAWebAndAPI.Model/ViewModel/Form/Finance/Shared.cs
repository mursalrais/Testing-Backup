using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public static class Shared
    {
        public enum Operations { Create, Edit, View }

        public const string Fund = "3000";

        public const string ErrorDevInvalidState = "DevError: Editing or Viewing without providing a valid id";

        public static Operations GetOperation(string op)
        {
            return (Operations)Enum.Parse(typeof(Operations), op);
        }

    }
}
