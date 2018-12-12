using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkaratakBot
{
    [Serializable]
    public class UpdateParameters
    {
        public int updatePropertyID { get; set; }
        public int updateSize { get; set; }
        public int updateFloorCount { get; set; }
        public int updateFloorLevel { get; set; }
        public int updateCountryCurrentCount { get; set; }
        public int updateBedRoomCount { get; set; }
        public int updateBathRoomCount { get; set; }
        public int updateSalePrice { get; set; }
        public int updateRentPrice { get; set; }
        public string updatePhoneNumber { get; set; }
        public string updateCountry { get; set; }
        public string updateCity { get; set; }
        public string updateCategory { get; set; }
        public string updateType { get; set; }
        public string updateAddress { get; set; }
        public string updateZipCode { get; set; }
        public string updateOtherDetails { get; set; }
        public string updatePhotoPath { get; set; }
        public string updateLocation { get; set; }
        public bool updateHasGarden { get; set; }
        public bool updateHasGarage { get; set; }
        public PhotoParameters PhotoParameters { get; set; }

    }
    public class PhotoParameters
    {
        /// <summary>
        /// If We Are Adding More Photos Or Updating Current
        /// </summary>
        public bool Add { get; set; }
        /// <summary>
        /// Number of exsisting photos
        /// </summary>
        public int MinCount { get; set; }
        public ICollection<UploadPhoto> Photos { get; set; }
    }
    public class UploadPhoto
    {
        public string PublicId { get; set; }
        public string PhotoPath { get; set; }
    }
}