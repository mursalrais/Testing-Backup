using MCAWebAndAPI.Model.ViewModel.Form.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Travel
{
    public interface IAuthAdvRequestService
    {
        AuthAdvRequestVM Get(int? ID);

        IEnumerable<AuthAdvReq_FlightInCountryVM> GetFlightInCountryItems(int authAdvRequestID);
    }
}
