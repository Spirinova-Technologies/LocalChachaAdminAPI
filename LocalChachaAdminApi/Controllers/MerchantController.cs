using AutoMapper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Controllers
{
    [Route("api/merchant")]
    [ApiController]
    public class MerchantController : BaseController
    {
        private readonly IBulkInsertService bulkInsertService;
        private readonly IMerchantService merchantService;
        private readonly IQuickBloxService quickBloxService;
        private readonly IMapper mapper;

        public MerchantController(IBulkInsertService bulkInsertService, IMerchantService merchantService,
            IMapper mapper, IQuickBloxService quickBloxService)
        {
            this.bulkInsertService = bulkInsertService;
            this.merchantService = merchantService;
            this.mapper = mapper;
            this.quickBloxService = quickBloxService;
        }

        [HttpGet("builkinsert")]
        public async Task<ActionResult> InsertData()
        {
            try
            {
                //TODO: remove try catch and add globalexception filter
                await bulkInsertService.InsertBulkData();
                return Ok(new { message = "Data inserted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("getmerchants")]
        public async Task<ActionResult> GetMerchants()
        {
            try
            {
                var merchants = await merchantService.GetMerchants();
                var mappersViewModel = mapper.Map<List<MerchantRequestModel>, List<MerchantViewModel>>(merchants);
                return Ok(merchants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("savemerchants")]
        public async Task<ActionResult> SaveMerchants(List<MerchantViewModel> merchantViewModels)
        {
            try
            {
                var merchants = mapper.Map<List<MerchantViewModel>, List<MerchantRequestModel>>(merchantViewModels);
                var response = await merchantService.SaveMerchants(merchants);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("deletemerchants")]
        public async Task<ActionResult> DeleteMerchants()
        {
            try
            {
                await merchantService.DeleteMerchants();
                return Ok(new CommonResponseModel { Message = "Merchants deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}