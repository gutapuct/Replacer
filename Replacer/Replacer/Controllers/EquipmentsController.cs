using Replacer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Replacer
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EquipmentsController : ApiController
    {
        private static Business.Replacer _model = new Business.Replacer();

        // GET api/equipments
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync()
        {
            return Ok(await _model.GetAllEquipmentsAsync());
        }

        // GET api/equipments/5c025e1346d9e5403cf6abaf
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync(string id)
        {
            var resultMessage = await _model.GetEquipmentByIdAsync(id);

            if (resultMessage.Errors.Count == 0)
                return Ok(resultMessage);
            else
                return Content(HttpStatusCode.NotFound, resultMessage);
        }

        // POST api/equipments
        [HttpPost]
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
        public async Task<IHttpActionResult> PostEquipmentNamesAsync([FromBody]EquipmentShot equipment)
        {
            var resultMessage = await _model.ReplaceNamesAsync(equipment);

            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.NotModified, resultMessage);
        }

        [HttpPost]
        [Route("~/api/equipment/reasons/{id}")]
        public async Task<IHttpActionResult> PostChangeReasonsAsync(string id, [FromBody]IEnumerable<Reason> reasons)
        {
            var resultMessage = await _model.ReplaceReasonsAsync(id, reasons);
            
            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpPost]
        [Route("~/api/equipment/typename/{id}/{typeName}")]
        public async Task<IHttpActionResult> PostSaveTypeNameAsync(string id, string typeName)
        {
            var resultMessage = await _model.ReplaceTypeNameByIdAsync(id, typeName);

            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(string id)
        {
            var resultMessage = await _model.DeleteEquipmentAsync(id);

            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpPost]
        [Route("~/api/equipment/order/{id}/{value}")]
        public async Task<IHttpActionResult> ChangeOrderAsync(string id, int value)
        {
            var resultMessage = await _model.ChangeOrderAsync(id, value);

            if (resultMessage.Errors.Count == 0)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpPost]
        [Route("~/api/importdb")]
        public async Task<IHttpActionResult> ImportDbAsync()
        {
            var resultMessage = await _model.ImportDbAsync(Request.Content);

            if (resultMessage.Errors.Count == 0)
                return Ok(resultMessage);
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }

        [HttpPost]
        [Route("~/api/start")]
        public async Task<IHttpActionResult> StartAsync()
        {
            var resultMessage = await _model.StartAsync(Request.Content);

            if (resultMessage.Errors.Count == 0)
                return Ok(resultMessage.Object);
            else
                return Content(HttpStatusCode.BadRequest, resultMessage);
        }
    }
}
