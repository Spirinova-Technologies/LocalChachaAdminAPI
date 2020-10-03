using Dapper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Repositories
{
    public class ProductRepository : ConnectionRepository, IProductRepository
    {
        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task InsertSuggestedProduct(SuggestedItem suggestedItem)
        {
            var sqlQuery = @"INSERT INTO suggest_items (name, description, maxQuantity, image, isActive, isPublished, created, modified)
                             VALUES (@name, @description, @maxQuantity, @image, @isActive, @isPublished, @created, @modified)";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                await connection.QueryAsync(sqlQuery,
                                 new
                                 {
                                     name = suggestedItem.Name,
                                     description = suggestedItem.Description,
                                     maxQuantity = suggestedItem.MaxQuantity,
                                     image = suggestedItem.Image,
                                   //  price = suggestedItem.Price,
                                     isActive = suggestedItem.IsActive,
                                     isPublished = suggestedItem.IsPublished,
                                     created = DateTime.Now,
                                     modified = DateTime.Now
                                 });
            }
        }

        public async Task<SuggestedItemResult> GetSuggestedItems(SearchFilter filter)
        {
            if (filter.PageIndex == 0)
            {
                filter.PageIndex = 1;
            }

            if (filter.PageSize == 0)
            {
                filter.PageSize = 10;
            }

            var suggestedItemResult = new SuggestedItemResult();
            var sqlQuery = $@"select count(id) from suggest_items;
                             select * from suggest_items order by {filter.OrderBy ?? "id"} LIMIT {filter.PageSize * (filter.PageIndex - 1)}, {filter.PageSize}";

            using (IDbConnection connection = await OpenConnectionAsync())
            {
                using (var response = await connection.QueryMultipleAsync(sqlQuery))
                {
                    suggestedItemResult.TotalRecords = response.Read<int>().Single();
                    suggestedItemResult.SuggestedItems = response.Read<SuggestedItem>().ToList();

                    return suggestedItemResult;
                }
            }
        }
    }
}