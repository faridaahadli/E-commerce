using System;

namespace CRMHalalBackEnd.Models.Employee
{
    public class EmployeeTokenData
    {
        public int ActiveUserId { get; set; }
        public string TenantId { get; set; }
        public string Role { get; set; } = String.Empty;
        public string Permission { get; set; } = String.Empty;
        public string Token { get; set; }
        public bool IsStore { get; set; }

        public override string ToString()
        {
            return $"{nameof(ActiveUserId)}: {ActiveUserId}, {nameof(TenantId)}: {TenantId}, {nameof(Role)}: {Role}, {nameof(Permission)}: {Permission}, {nameof(Token)}: {Token}";
        }
    }
}