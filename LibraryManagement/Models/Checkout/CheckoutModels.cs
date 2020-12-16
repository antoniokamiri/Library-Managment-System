using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Models.Checkout
{
    public class CheckoutModels
    {
        public string LibraryCardId { get; set; }
        public string Title { get; set; }
        public int AssetId { get; set; }
        public string ImageURl { get; set; }
        public int HoldCount { get; set; }
        public bool IsCheckedOut { get; set; }

    }
}
