using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<CheckOuts> GetAll();
        CheckOuts GetById(int checkoutId);
        void Add(CheckOuts newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId );
        IEnumerable<CheckOutHistory> GetCheckOutHistories(int Id);
        void PlaceHold(int assetId, int libraryCardId);
        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatronName(int Id);
        DateTime GetCurrentHoldPlaced(int Id);
        IEnumerable<Holds> GetCurrentHolds(int Id);        
        int GetNumberOfCopies(int id);
        bool IsCheckedOut(int id);
        void MarkLost(int assetId);
        void MarkFound(int assetId);
        CheckOuts GetLatestCheckout(int assetId);
       
    }
}
