using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Common
{
    public class UserDesign
    {
    
        public int UserDesignId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public override string ToString()
        {
            return $"{nameof(UserDesignId)}: {UserDesignId}, {nameof(UserId)}: {UserId}, {nameof(Status)}: {Status}, {nameof(Properties)}: {Properties}";
        }
    }
}


//public string FontName { get; set; }
//public string FontSize { get; set; }
//public string MainColor { get; set; }
//public string SecondaryColor { get; set; }
//public string LightTonedColor { get; set; }
//public string BackgroundColor { get; set; }
//public string BackgroundImage { get; set; }
//public string BackgroundImageForSpecialOffer { get; set; }