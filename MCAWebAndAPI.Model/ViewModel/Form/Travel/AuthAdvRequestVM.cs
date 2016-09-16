using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Travel
{
    /// <summary>
    /// Wireframe TR03: Travel Authorization and Advance Request
    /// </summary>

    public class AuthAdvRequestVM : Item
    {
        public IEnumerable<AuthAdvReq_FlightInCountryVM> FlightsInCountry { get; set; } = new List<AuthAdvReq_FlightInCountryVM>();

        public IEnumerable<AuthAdvReq_FlightInternationalVM> FlightsInternational { get; set; } = new List<AuthAdvReq_FlightInternationalVM>();

        public IEnumerable<AuthAdvReq_LodgingInCountryVM> LodgingsInCountry { get; set; } = new List<AuthAdvReq_LodgingInCountryVM>();

        public IEnumerable<AuthAdvReq_LodgingInternationalVM> LodgingsInternational { get; set; } = new List<AuthAdvReq_LodgingInternationalVM>();

        public IEnumerable<TRAAuthAdvReq_PerDiemInCountryVM> PerDiemsInCountry { get; set; } = new List<TRAAuthAdvReq_PerDiemInCountryVM>();
        public IEnumerable<AuthAdvReq_PerDiemInternationalVM> PerDiemsInternational { get; set; } = new List<AuthAdvReq_PerDiemInternationalVM>();

        public IEnumerable<AuthAdvReq_OthersInCountryVM> OthersInCountry { get; set; } = new List<AuthAdvReq_OthersInCountryVM>();
        public IEnumerable<AuthAdvReq_OthersInternationalVM> OthersInternational { get; set; } = new List<AuthAdvReq_OthersInternationalVM>();

    }
}
