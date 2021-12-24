using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMHalalBackEnd.Interfaces
{
    public interface IProvider
    {
        Task<bool> TokenValidate(string token);
    }
}
