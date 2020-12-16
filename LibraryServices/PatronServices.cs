using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class PatronServices : IPatron
    {
        private LibraryContext _context;
        public PatronServices(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public Patron Get(int Id)
        {
            return _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).FirstOrDefault(p => p.Id == Id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch);
        }

        public IEnumerable<CheckOutHistory> GetCheckOutHistories(int patronId)
        {
            var cardId = _context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.Id == patronId).LibraryCard.Id;

            return _context.CheckOutHistories
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .Where(co => co.LibraryCard.Id == cardId)
                .OrderByDescending(co=> co.CheckedOut);
        }

        public IEnumerable<CheckOuts> GetCheckOuts(int patronId)
        {
            var cardId = _context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.Id == patronId).LibraryCard.Id;

            return _context.CheckOuts.Include(co => co.LibraryCard).Include(co => co.LibraryAsset).Where(co => co.LibraryCard.Id == cardId);
        }

        public IEnumerable<Holds> GetHolds(int patronId)
        {
            var cardId = _context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.Id == patronId).LibraryCard.Id;

            return _context.Holds
                .Include(h => h.LibraryCard)
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryCard.Id == cardId)
                .OrderByDescending(h => h.HoldPlaced);
        }
    }
}
