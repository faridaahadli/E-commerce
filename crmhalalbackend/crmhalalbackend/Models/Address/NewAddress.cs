using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Address
{
    public class NewAddress
    {
        public int AddressId { get; set; }
        //public int UserId { get; set; }
        //public int CompanyId { get; set; }
        //public int StoreId { get; set; }
        public int AddressTypeId { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int PostCode { get; set; }
        public string Title { get; set; }
        public override string ToString()
        {
            return $"{nameof(AddressId)}: {AddressId}, {nameof(AddressTypeId)}: {AddressTypeId}, {nameof(Address)}: {Address}, {nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}, {nameof(Country)}: {Country}, {nameof(City)}: {City}, {nameof(PostCode)}: {PostCode}, {nameof(Title)}: {Title}";
        }
    }
}