using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Interfaces;

namespace CRMHalalBackEnd.Helpers
{
    public class FactoryProvider
    {
        public static IProvider Build(string provider)
        {
            if (provider.ToLower().Equals("google"))
            {
                return new GoogleProvider();
            }
            else if (provider.ToLower().Equals("facebook"))
            {
                return new FacebookProvider();
            }
            return null;
        }
    }
}