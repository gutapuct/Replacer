using Replacer.Business;
using Replacer.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Replacer
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReasonsController : ApiController
    {
        private static Business.Replacer _model = new Business.Replacer();
        // GET api/reasons
        public async Task<IHttpActionResult> GetAsync()
        {
            return Ok(await _model.GetAllReasonsAsync());
        }

        // GET api/reasons/5
        //public async Task<IHttpActionResult> GetAsync(int id)
        //{
        //    return Ok(await _model.GetReasonAsync(id));
        //}

        // POST api/reasons
        public async Task<IHttpActionResult> PostAsync([FromBody]Equipment reason)
        {
            var isSuccess = await _model.AddReasonAsync(reason);
            if (isSuccess)
                return Content(HttpStatusCode.Created, reason);
            else
                return BadRequest();
        }

        // PUT api/reasons/5
        //public async Task<IHttpActionResult> PutAsync(int id, [FromBody]string reason)
        //{
        //    var isSuccess = await _model.ChangeReasonAsync(id, reason);
        //    if (isSuccess)
        //        return Ok();
        //    else
        //        return NotFound();
        //}

        //// DELETE api/values/5
        //public async Task<IHttpActionResult> DeleteAsync (int id)
        //{
        //    var isSuccess = await _model.DeleteReasonAsync(id);
        //    if (isSuccess)
        //        return Ok();
        //    else
        //        return NotFound();
        //}
    }
}
