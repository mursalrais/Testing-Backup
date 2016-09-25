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

        public IEnumerable<TRAAuthAdvReq_PerDiemInCountryVM> GetPerDiemsInCountryItems(int authAdvRequestID)
        {
            List<TRAAuthAdvReq_PerDiemInCountryVM> list = new List<TRAAuthAdvReq_PerDiemInCountryVM>();

            list.Add(new TRAAuthAdvReq_PerDiemInCountryVM() { ID = 1, Date = DateTime.Now, Rate=10000, DeductBreakfast =true, BreakfastDeductionAmount = 1000, Remarks = "test 1" });
            list.Add(new TRAAuthAdvReq_PerDiemInCountryVM() { ID = 2, Date = DateTime.Now.AddDays(1), Rate = 10000, DeductLunch = true, LunchDeductionAmount = 1000, Remarks = "test 2" });
            list.Add(new TRAAuthAdvReq_PerDiemInCountryVM() { ID = 3, Date = DateTime.Now.AddDays(2), Rate = 10000, DeductDinner = true, DinnerDeductionAmount = 1000, Remarks = "test 3" });

            var ps = new Shared.PlaceService();
            var placesList = ps.GetAllListItems(string.Empty);

            var provserv = new Shared.ProvinceService();
            var provinceList = provserv.GetAllListItems(string.Empty);

            list[0].PlaceOfDeparture.Value = placesList[0].ID;
            list[0].PlaceOfDeparture.Text = placesList[0].Name;
            list[0].PlaceOfArrival.Value = placesList[1].ID;
            list[0].PlaceOfArrival.Text = placesList[1].Name;
            list[0].ProvinceOfDeparture.Value = provinceList[0].ID;
            list[0].ProvinceOfDeparture.Text = provinceList[0].Name;
            list[0].ProvinceOfArrival.Value = provinceList[1].ID;
            list[0].ProvinceOfArrival.Text = provinceList[1].Name;
            list[0].ProvinceOfRatePerDay.Value = list[0].ProvinceOfArrival.Value;
            list[0].ProvinceOfRatePerDay.Text = list[0].ProvinceOfArrival.Text;

            list[1].PlaceOfDeparture.Value = placesList[1].ID;
            list[1].PlaceOfDeparture.Text = placesList[1].Name;
            list[1].PlaceOfArrival.Value = placesList[1].ID;
            list[1].PlaceOfArrival.Text = placesList[1].Name;
            list[1].ProvinceOfDeparture.Value = provinceList[1].ID;
            list[1].ProvinceOfDeparture.Text = provinceList[1].Name;
            list[1].ProvinceOfArrival.Value = provinceList[1].ID;
            list[1].ProvinceOfArrival.Text = provinceList[1].Name;
            list[1].ProvinceOfRatePerDay.Value = list[1].ProvinceOfArrival.Value;
            list[1].ProvinceOfRatePerDay.Text = list[1].ProvinceOfArrival.Text;

            list[2].PlaceOfDeparture.Value = placesList[1].ID;
            list[2].PlaceOfDeparture.Text = placesList[1].Name;
            list[2].PlaceOfArrival.Value = placesList[0].ID;
            list[2].PlaceOfArrival.Text = placesList[0].Name;
            list[2].ProvinceOfDeparture.Value = provinceList[1].ID;
            list[2].ProvinceOfDeparture.Text = provinceList[1].Name;
            list[2].ProvinceOfArrival.Value = provinceList[0].ID;
            list[2].ProvinceOfArrival.Text = provinceList[0].Name;
            list[2].ProvinceOfRatePerDay.Value = list[2].ProvinceOfDeparture.Value;
            list[2].ProvinceOfRatePerDay.Text = list[2].ProvinceOfDeparture.Text;

            return list;
        }
    }
}
