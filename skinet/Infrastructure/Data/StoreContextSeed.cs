using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
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
        // var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!context.ProductGraphic.Any())
        {
          var brandsData =
              File.ReadAllText("../Infrastructure/Data/SeedData/graphic.json");

          var brands = JsonSerializer.Deserialize<List<ProductGraphic>>(brandsData);

          foreach (var item in brands)
          {
            context.ProductGraphic.Add(item);
          }

          await context.SaveChangesAsync();
        }

        if (!context.ProductPlatform.Any())
        {
          var typesData =
              File.ReadAllText("../Infrastructure/Data/SeedData/platform.json");

          var types = JsonSerializer.Deserialize<List<ProductPlatform>>(typesData);

          foreach (var item in types)
          {
            context.ProductPlatform.Add(item);
          }

          await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
          var productsData =
              File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

          var products = JsonSerializer.Deserialize<List<ProductSeedModel>>(productsData);

          foreach (var item in products)
          {
            var pictureFileName = item.PictureUrl.Substring(16);
            var product = new Product
            {
              Name = item.Name,
              Description = item.Description,
              Price = item.Price,
              ProductGraphicId = item.ProductGraphicId,
              ProductPlatformId = item.ProductPlatformId
            };
            product.AddPhoto(item.PictureUrl, pictureFileName);
            context.Products.Add(product);
          }

          await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
          var dmData =
              File.ReadAllText("../Infrastructure/Data/SeedData/delivery.json");

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