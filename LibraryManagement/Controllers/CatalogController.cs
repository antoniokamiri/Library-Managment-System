using LibraryData;
using LibraryManagement.Models.Catalog;
using LibraryManagement.Models.Checkout;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        private ICheckout _checkouts;
        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _assets = assets;
            _checkouts = checkouts;

        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();

            var ListingResult = assetModels.Select(result => new AssetIndexListingModel
            {
                Id = result.Id,
                ImageURL = result.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                Title = result.Title,
                Type = _assets.GetType(result.Id)

            });

            var model = new AssetIndexModel()
            {
                Assets = ListingResult
            };

            return View(model);
        }

        public IActionResult Detail(int Id)
        {
            var asset = _assets.GetById(Id);
            var currentHolds = _checkouts.GetCurrentHolds(Id).Select(a => new AssetHoldModel
            {
                HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id), //.ToString("d"),
                PatronName = _checkouts.GetCurrentHoldPatronName(a.Id)
            });

            var model = new AssetDetailModel
            {
                AssetId = Id,
                Title = asset.Title,
                Type = _assets.GetType(Id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(Id),
                CurrentLocation = _assets.GetCurrentLocation(Id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(Id),
                CheckOutHistories = _checkouts.GetCheckOutHistories(Id),
                ISBN = _assets.GetIsbn(Id),
                LastestCheckout = _checkouts.GetLatestCheckout(Id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(Id),
                CurrentHolds = currentHolds

            };

            return View(model);

        }

        public IActionResult Checkout(int Id)
        {
            var asset = _assets.GetById(Id);
            var model = new CheckoutModels
            {
                AssetId = Id,
                ImageURl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(Id)
            };
            return View(model);
        }

        public IActionResult Hold(int Id)
        {
            var asset = _assets.GetById(Id);

            var model = new CheckoutModels
            {
                AssetId = Id,
                ImageURl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                HoldCount = _checkouts.GetCurrentHolds(Id).Count()
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
           _checkouts.CheckInItem(id);
           return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkLost(int id)
        {
            _checkouts.MarkLost(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkFound(int id)
        {
            _checkouts.MarkFound(id);
            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkouts.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { Id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { Id = assetId });
        }
    }
}
