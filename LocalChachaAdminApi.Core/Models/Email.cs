using System.Collections.Generic;

namespace LocalChachaAdminApi.Core.Models
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Tos { get; set; }
    }
}