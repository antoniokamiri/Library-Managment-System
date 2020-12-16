using LibraryData;
using LibraryData.Models;
using LibraryManagement.Models.Patron;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class PatronController : Controller
    {
        private IPatron _patron;

        public PatronController(IPatron patron)
        {
            _patron = patron;
        }

        public IActionResult Index()
        {
            var allPatrons = _patron.GetAll();

            var patronModels = allPatrons.Select(p => new PatronDetailModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                OverDueFees = p.LibraryCard.Fees,
                HomeLibraryBranch = p.HomeLibraryBranch.Name
            }).ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };

            return View(model);
        }
         public IActionResult Detail(int Id)
        {
            var patron = _patron.Get(Id);

             var model = new PatronDetailModel
            {
                LastName = patron.LastName,
                FirstName = patron.FirstName,
                Address = patron.Adress,
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                MemberSince = patron.LibraryCard.Created,
                OverDueFees = patron.LibraryCard.Fees,
                LibraryCardId = patron.LibraryCard.Id,
                MobileNumber = patron.MobileNumber,
                AssetCheckedOut = _patron.GetCheckOuts(Id).ToList() ?? new List<CheckOuts>(),
                CheckoutHistory = _patron.GetCheckOutHistories(Id),
                Hold = _patron.GetHolds(Id)
            };

            return View(model);
        }
    }
}
