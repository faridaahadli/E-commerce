using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Courier
{
    public class CourierInsDto
    {
        public bool IsDeliveryExist { get; set; }
        public decimal? MinPriceForFree { get; set; }
        public string Note { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public int RegionDeliveryId { get; set; }
        public decimal Price { get; set; }
    }
}