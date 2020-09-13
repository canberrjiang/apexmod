using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
  public class StoreContextSeed
  {
    public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
      try
      {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!context.Tags.Any())
        {
          var tagsData =
              File.ReadAllText(path + @"/Data/SeedData/tag.json");

          var tags = JsonSerializer.Deserialize<List<Tag>>(tagsData);

          foreach (var item in tags)
          {
            context.Tags.Add(item);
          }

          await context.SaveChangesAsync();
        }

        if (!context.ProductCategories.Any())
        {
          var categoriesData =
              File.ReadAllText(path + @"/Data/SeedData/category.json");

          var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);

          foreach (var item in categories)
          {
            context.ProductCategories.Add(item);
          }

          await context.SaveChangesAsync();
        }


        if (!context.BaseProducts.Any())
        {
          var productsData =
              File.ReadAllText(path + @"/Data/SeedData/products.json");

          var products = JsonSerializer.Deserialize<List<ProductSeedModel>>(productsData);

          foreach (var item in products)
          {
            var pictureFileName = item.PictureUrl.Substring(16);
            var product = new Product
            {
              Name = item.Name,
              Description = item.Description,
              Price = item.Price,
              PictureUrl = item.PictureUrl,
              ProductCategoryId = item.ProductCategoryId
            };
            product.AddPhoto(item.PictureUrl, pictureFileName);
            context.BaseProducts.Add(product);
          }

          await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
          var dmData =
              File.ReadAllText(path + @"/Data/SeedData/delivery.json");

          var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

          foreach (var item in methods)
          {
            context.DeliveryMethods.Add(item);
          }

          await context.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<StoreContextSeed>();
        logger.LogError(ex.Message);
      }
    }
  }
}