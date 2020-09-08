using System;
using System.Collections.Generic;
using System.Text;

namespace LocalChachaAdminApi.Core.Models
{
   public class CommonResponseModel
    {
        public CommonResponseModel()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public string Message { get; set; }
    }
}
