using System;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyMainPageDto
    {
        public int CompanyId { get; set; }
        public string Name{ get; set; } = String.Empty;
        public string TenantId { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(CompanyId)}: {CompanyId}, {nameof(Name)}: {Name}, {nameof(TenantId)}: {TenantId}";
        }
    }
}