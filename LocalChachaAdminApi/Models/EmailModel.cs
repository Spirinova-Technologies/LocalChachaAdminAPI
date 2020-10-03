using System.Collections.Generic;

namespace LocalChachaAdminApi.Models
{
    public class EmailModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Tos { get; set; }
    }
}