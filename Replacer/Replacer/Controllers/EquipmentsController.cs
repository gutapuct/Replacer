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
    public class EquipmentsController : ApiController
    {
        private static Business.Replacer _model = new Business.Replacer();
        // GET api/equipments
        public async Task<IHttpActionResult> GetAsync()
        {
            return Ok(await _model.GetAllEquipmentsAsync());
        }

        // GET api/equipments/5c025e1346d9e5403cf6abaf
        public async Task<IHttpActionResult> GetAsync(string id)
        {
            var resultMessage = await _model.GetEquipmentById(id);

            if (resultMessage.Errors.Count == 0)
                return Ok(resultMessage);
            else
                return Content(HttpStatusCode.NotFound, resultMessage);
        }

        // POST api/equipments
        public async Task<IHttpActionResult> PostAsync([FromBody]string equipment)
        {
            var resultMessage = await _model.AddEquipmentAsync(equipment);

            if (resultMessage.Errors.Count == 0)
                return Content(HttpStatusCode.Created, resultMessage.Object);
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpPost]
        [Route("~/api/equipments/names")]
        public async Task<IHttpActionResult> PostEquipmentNames([FromBody]EquipmentShot equipment)
        {
            var resultMessage = await _model.ReplaceNames(equipment);

            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.NotModified, resultMessage);
        }

        // PUT api/equipments/5
        //public async Task<IHttpActionResult> PutAsync(int id, [FromBody]string reason)
        //{
        //    var isSuccess = await _model.ChangeReasonAsync(id, reason);
        //    if (isSuccess)
        //        return Ok();
        //    else
        //        return NotFound();
        //}

        //// DELETE api/equipments/5
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
