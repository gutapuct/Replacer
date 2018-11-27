using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer
{
    public class Reason
    {
        private readonly List<string> _reasons;

        public Reason()
        {
             _reasons = new List<string> { "val1", "val2" };
        }

        public IEnumerable<string> GetAllReasons()
        {
            return _reasons;
        }

        public string GetReason(int id)
        {
            if (_reasons.Count > id && id >= 0)
            {
                return _reasons[id];
            }
            else
            {
                return "Error. We couldn't find that reason.";
            }
        }

        public void AddReason(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _reasons.Add(value);
            }
        }

        public void ChangeReason(int id, string value)
        {
            if (_reasons.Count > id && id >= 0)
            {
                _reasons[id] = value;
            }
        }

        public void DeleteReason(int id)
        {
            if (_reasons.Count > id && id >= 0)
            {
                _reasons.RemoveAt(id);
            }
        }
    }
}
