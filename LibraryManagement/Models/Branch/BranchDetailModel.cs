﻿using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Branch
{
    public class BranchDetailModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public DateTime OpenDate { get; set; }
        public string MobileNumber { get; set; }
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        public int NumberOfPatrons { get; set; }
        public int NumberOfAssets { get; set; }
        public decimal TotalAssetValue { get; set; }
        public string ImageURl { get; set; }
        public IEnumerable<string> HoursOpen { get; set; }

    }
}
