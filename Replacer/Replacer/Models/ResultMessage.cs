using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public class ResultMessage
    {
        public List<string> Errors { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }

        public ResultMessage()
        {
            Message = String.Empty;
            Errors = new List<string>();
        }

        public void AddErrorsFromException(Exception exception)
        {
            this.Errors.Add($"Message: {exception.Message}");
            this.Errors.Add($"Inner Exception: {exception.InnerException}");
            this.Errors.Add($"StackTrace: {exception.StackTrace}");
        }
    }
}
