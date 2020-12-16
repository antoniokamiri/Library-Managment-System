using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class CheckoutServices : ICheckout
    {
        private LibraryContext _context;
        public CheckoutServices(LibraryContext context)
        {
            _context = context;
        }

        public void Add(CheckOuts newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(i => i.Id == assetId);
            //  _context.Update(item); because "UpdateAssestStatus has the _context.Update(item); call thats why we remove the update call here"

            // remove any existing checkout on the item
            RemoveExistingCheckouts(assetId);
            // close any existing checkout history 
            CloseExistingCheckoutHistory(assetId, now);
            // look for existing holds on the item 
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);
            // if there are hold => checkout the item to the librarycard with the earliest hold
            
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }

            // Otherwise, update the item status to available 
            UpdateAssetStatus(assetId, "Available");

            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Holds> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(h => h.HoldPlaced).FirstOrDefault();

            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                // Add logic herer to handle feedback to the user.                   
            }
            var item = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);
            UpdateAssetStatus(assetId, "Checked Out");
            var libraryCard = _context.LibraryCards.Include(card => card.CheckOuts).FirstOrDefault(card => card.Id == libraryCardId);

            var now = DateTime.Now;

            var checkout = new CheckOuts
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckOutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard,
            };
            _context.Add(checkoutHistory);
            _context.SaveChanges();

        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public IEnumerable<CheckOuts> GetAll()
        {
            return _context.CheckOuts;
        }

        public CheckOuts GetById(int Id)
        {
            return GetAll().FirstOrDefault(c => c.Id == Id);
        }

        public IEnumerable<CheckOutHistory> GetCheckOutHistories(int Id)
        {
            return _context.CheckOutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == Id);                

        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);
            var cardId = hold?.LibraryCard.Id;  // the ? is used to not allow null or if its null to return empty string 
            var patron = _context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName +" "+ patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds.Include(h => h.LibraryAsset).Include(h => h.LibraryAsset).FirstOrDefault(h => h.Id == holdId).HoldPlaced;

        }

        public IEnumerable<Holds> GetCurrentHolds(int Id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)                
                .Where(h => h.LibraryAsset.Id == Id);
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
            
            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId, now);            

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);
            _context.Update(item);
            item.Status = _context.Statuses
                .FirstOrDefault(s => s.Name == newStatus);            
        }

        private void CloseExistingCheckoutHistory(int assetId, DateTime now)
        {
            // close existing checkout histroy 
            var history = _context.CheckOutHistories
                .FirstOrDefault(history => history.LibraryAsset.Id == assetId && history.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            //remove any checkout that might have been made
            var checkout = _context.CheckOuts
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {            
            UpdateAssetStatus(assetId, "Lost");            
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;
            var asset = _context.LibraryAssets.Include(a=> a.Status).FirstOrDefault(a => a.Id == assetId);
            var card = _context.LibraryCards.FirstOrDefault(lc => lc.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Holds
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public int GetNumberOfCopies(int id)
        {
            return _context.LibraryAssets.First(a => a.Id == id).NumberOfCopies;

        }

        public bool IsCheckedOut(int assetId)
        {
            var IsCheckOut = _context.CheckOuts.Where(co => co.LibraryAsset.Id == assetId).Any(); 
            return IsCheckOut; //Instead of this you can just write return then the agrument (_context.CheckOuts.Where......)
        }

        public CheckOuts GetLatestCheckout(int assetId)
        {
            return _context.CheckOuts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
         
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            
            if (checkout == null)
            {
                return "";
            }

            var cardId = checkout.LibraryCard.Id;

            var partron = _context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return partron.FirstName + " " + partron.LastName;
        }

        private CheckOuts GetCheckoutByAssetId(int assetId)
        {
            return _context.CheckOuts.Include(co => co.LibraryAsset).Include(co => co.LibraryCard).FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }
    }
}
