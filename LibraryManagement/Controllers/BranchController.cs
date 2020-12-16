using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryData;
using LibraryManagement.Models.Branch;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;

        public BranchController (ILibraryBranch branch)
        {
            _branch = branch;
        }
        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = _branch.IsBranchOpen(branch.Id),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count()
            });

            var model = new BranchIndexModel
            {
                Branch = branches
            };

            return View(model);
        }

        public IActionResult Detail(int Id)
        {
            var branch = _branch.Get(Id);

            var model = new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                MobileNumber = branch.MobileNumber,
                OpenDate = branch.OpenDate,
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count(),
                TotalAssetValue = _branch.GetAssets(Id).Sum(a => a.Cost),
                ImageURl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(Id)
            };

            return View(model);
        }
    }
}
