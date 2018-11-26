using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Replacer
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReasonsController : ApiController
    {
        private static List<string> _reasons = new List<string> { "val1", "val2" };
        // GET api/reasons
        public IEnumerable<string> Get()
        {
            return _reasons;
        }

        // GET api/reasons/5
        public string Get(int id)
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

        // POST api/reasons
        public void Post([FromBody]string reason)
        {
            if (!string.IsNullOrWhiteSpace(reason))
            {
                _reasons.Add(reason);
            }
        }

        // PUT api/reasons/5
        public void Put(int id, [FromBody]string reason)
        {
            if (_reasons.Count > id && id >= 0)
            {
                _reasons[id] = reason;
            }
        }

        // DELETE api/values/5
        public void Delete (int id)
        {
            if (_reasons.Count > id && id >= 0)
            {
                _reasons.RemoveAt(id);
            }
        }
    }
}
