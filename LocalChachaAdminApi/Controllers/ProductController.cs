using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
using LocalChachaAdminApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalChachaAdminApi.Controllers
{
    [Route("api/product")]
   // [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProductController(IProductService productService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.productService = productService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("getsuggestedproducts")]
        public async Task<ActionResult> GetSuggestedProducts(SearchFilterModel searchFilterModel)
        {
            try
            {
                var searchFilter = mapper.Map<SearchFilterModel, SearchFilter>(searchFilterModel);
                var suggestedProducts = await productService.GetSuggestedItems(searchFilter);
                var suggestedProductsModel = mapper.Map<SuggestedItemResult, SuggestedItemResultModel>(suggestedProducts);
                return Ok(suggestedProductsModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("uploadsuggestedproductsexcel")]
        public async Task<ActionResult> UploadSuggestedProductsExcel()
        {
            try
            {
                var files = httpContextAccessor.HttpContext.Request.Form.Files;
                if (files.Count == 0)
                    return BadRequest("Invalid csv file");

                var file = files[0];

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var suggestedProducts = csv.GetRecords<CsvSuggestedItemModel>();
                        var suggestedProductsModel = mapper.Map<List<CsvSuggestedItemModel>, List<SuggestedItem>>(suggestedProducts.ToList());

                        await productService.BulkInsertSuggestedItems(suggestedProductsModel);
                    }
                }

                return Ok(new { message = "Products uploaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
