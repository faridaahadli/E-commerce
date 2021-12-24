using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Address
{
    public class AddressResponse
    {
        public int AddressId { get; set; }
        public int AddressTypeId { get; set; }
        public bool IsDefault { get; set; }
        public string Address { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public int PostCode { get; set; }
        public override string ToString()
        {
            return $"{nameof(AddressId)}: {AddressId}, {nameof(AddressTypeId)}: {AddressTypeId}, {nameof(IsDefault)}: {IsDefault}, {nameof(Address)}: {Address}, {nameof(Latitude)}: {Latitude}, {nameof(Longitude)}: {Longitude}, {nameof(Country)}: {Country}, {nameof(City)}: {City}, {nameof(Title)}: {Title}, {nameof(PostCode)}: {PostCode}";
        }
    }
}