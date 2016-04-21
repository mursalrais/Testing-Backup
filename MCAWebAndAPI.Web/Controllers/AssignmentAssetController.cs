using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.AssignmentAsset;

namespace MCAWebAndAPI.Web.Controllers
{
    public class AssignmentAssetController : Controller
    {
        public List<AssignmentAssetItemVM> Items { get; private set; }

        // GET: AssignmentAsset
        public ActionResult Index()
        {
            //viewModel.Header.AssetHolder = new List<string>()
            //{
            //    "List1","List2"
            // };

            List<AssignmentAssetItemVM> Dummy = new List<AssignmentAssetItemVM>();
            Dummy.Add(new AssignmentAssetItemVM
            {
                AssetDescription = "Asset Description",
                AssetNo = "Asset No",
                AssetSubNo = "Assets Sub No",
                FloorName = "Floor Name",
                Item = "Item",
                New = "New",
                OfficeName = "Office Name",
                RoomName = "Room Name"
            });

            Dummy.Add(new AssignmentAssetItemVM
            {
                AssetDescription = "Asset Description",
                AssetNo = "Asset No",
                AssetSubNo = "Assets Sub No",
                FloorName = "Floor Name",
                Item = "Item",
                New = "New",
                OfficeName = "Office Name",
                RoomName = "Room Name"
            });


            var viewModel = new AssignmentAssetVM()
            {
                Header = new AssignmentAssetHeaderVM()
                {
                    TransactionType = "Transaction Type kkk",
                    Date = new DateTime(2015, 12, 21),
                    AssetHolder = new List<string>()
                    {
                        "List 1","List 2"
                    }


                }
            };
            viewModel.Items = new List<AssignmentAssetItemVM>();
            viewModel.Items = Dummy;
            

            return View(viewModel);
    }
}

}