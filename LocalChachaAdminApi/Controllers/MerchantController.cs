using LocalChachaAdminApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Controllers
{
    [Route("api/merchant")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly IBulkInsertService bulkInsertService;

        public MerchantController(IBulkInsertService bulkInsertService)
        {
            this.bulkInsertService = bulkInsertService;
        }

        [HttpGet("builkinsert")]
        public async Task<ActionResult> InsertData()

        {
            try
            {
                //TODO: remove try catch and add globalexception filter
               await bulkInsertService.InsertBulkData();
                return Ok(new { message ="Data inserted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}