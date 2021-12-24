using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Contact
{
    public class NewContact
    {
        public int ContactId { get; set; }
        public int ContactTypeId { get; set; }
        public string Text { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public bool Whatsapp { get; set; }

        public override string ToString()
        {
            return $"{nameof(ContactId)}: {ContactId}," +
                $" {nameof(ContactTypeId)}: {ContactTypeId}, {nameof(Text)}: {Text}, " +
                $"{nameof(Note)}: {Note}";
            }
        }
    }
