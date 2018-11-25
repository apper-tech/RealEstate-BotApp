using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot
{
    [Serializable]
    public class InsertParameters
    {
        public SearchEntry currentEntry { get; set; }
        public int insertSize { get; set; }
        public int insertFloorCount { get; set; }
        public int insertFloorLevel { get; set; }
        public int insertCountryCurrentCount { get; set; }
        public int insertBedRoomCount { get; set; }
        public int insertBathRoomCount { get; set; }
        public int insertSalePrice { get; set; }
        public int insertRentPrice { get; set; }
        public string insertPhoneNumber { get; set; }
        public string insertCountry { get; set; }
        public string insertCity { get; set; }
        public string insertCategory { get; set; }
        public string insertType { get; set; }
        public string insertAddress { get; set; }
        public string insertZipCode { get; set; }
        public string insertOtherDetails{ get; set; }
        public string insertPhotoPath { get; set; }
        public string insertLocation { get; set; }
        public bool insertHasGarden { get; set; }
        public bool insertHasGarage { get; set; }
      
    }
}