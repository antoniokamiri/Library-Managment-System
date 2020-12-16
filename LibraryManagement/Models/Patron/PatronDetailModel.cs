using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Models.Patron
{
    public class PatronDetailModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int LibraryCardId { get; set; }
        public string Address { get; set; }
        public DateTime MemberSince { get; set; }
        public string MobileNumber { get; set; }
        public string HomeLibraryBranch { get; set; }
        public Decimal OverDueFees { get; set; }
        public IEnumerable<CheckOuts> AssetCheckedOut { get; set; }
        public IEnumerable<CheckOutHistory> CheckoutHistory { get; set; }
        public IEnumerable<Holds> Hold{ get; set; }
    }
}
