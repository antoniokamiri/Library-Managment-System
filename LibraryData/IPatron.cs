using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface IPatron
    {
        Patron Get(int Id);
        IEnumerable<Patron> GetAll();
        void Add(Patron newPatron);

        IEnumerable<CheckOutHistory> GetCheckOutHistories(int patronId);
        IEnumerable<Holds> GetHolds(int patronId);
        IEnumerable<CheckOuts> GetCheckOuts(int patronId);

    }
}
