using MCAWebAndAPI.Model.ViewModel.Form.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Travel
{
    public class AuthAdvRequestService : IAuthAdvRequestService
    {
        public AuthAdvRequestVM Get(int? ID)
        {
            AuthAdvRequestVM obj = new AuthAdvRequestVM();
            obj.ID = ID;

            if (ID.HasValue)
            {
                obj.FlightsInCountry = GetFlightInCountryItems(ID.Value);
            }
            
            return obj;
        }

        public IEnumerable<AuthAdvReq_FlightInCountryVM> GetFlightInCountryItems(int authAdvRequestID)
        {
            List<AuthAdvReq_FlightInCountryVM> list = new List<AuthAdvReq_FlightInCountryVM>();

            DateTime datePlusOne = DateTime.Now.AddDays(1);
            DateTime datePlusSeven = DateTime.Now.AddDays(7);
            DateTime datePlusFourteen = DateTime.Now.AddDays(14);
            list.Add(new AuthAdvReq_FlightInCountryVM() { ID = 1, Amount = 100, DateOfArrival = datePlusSeven, DateOfDeparture = datePlusOne,
                Remarks = "Test 1"});

            list.Add(new AuthAdvReq_FlightInCountryVM() { ID = 1, Amount = 100, DateOfArrival = datePlusFourteen, DateOfDeparture = datePlusOne, Remarks = "Test 2" });

            var ps = new Shared.PlaceService();
            var placesList = ps.GetAllListItems(string.Empty);

            list[0].PlaceOfDeparture.Value = placesList[2].ID;
            list[0].PlaceOfDeparture.Text = placesList[2].Name;
            list[0].PlaceOfArrival.Value = placesList[1].ID;
            list[0].PlaceOfArrival.Text = placesList[1].Name;
        
            list[1].PlaceOfDeparture.Value = placesList[1].ID;
            list[1].PlaceOfDeparture.Text = placesList[1].Name;
            list[1].PlaceOfArrival.Value = placesList[0].ID;
            list[1].PlaceOfArrival.Text = placesList[0].Name;

            return list;
        }
    }
}
