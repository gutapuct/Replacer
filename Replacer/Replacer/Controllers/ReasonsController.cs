using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Replacer
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReasonsController : ApiController
    {
        private static Reason _model = new Reason();
        // GET api/reasons
        public IEnumerable<string> Get()
        {
            return _model.GetAllReasons();
        }

        // GET api/reasons/5
        public string Get(int id)
        {
            return _model.GetReason(id);
        }

        // POST api/reasons
        public void Post([FromBody]string reason)
        {
            _model.AddReason(reason);
        }

        // PUT api/reasons/5
        public void Put(int id, [FromBody]string reason)
        {
            _model.ChangeReason(id, reason);
        }

        // DELETE api/values/5
        public void Delete (int id)
        {
            _model.DeleteReason(id);
        }
    }
}
