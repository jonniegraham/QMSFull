using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Database;
using Model;
using ModelWrapper;

namespace DataAccess
{
    public class Products
    {

        public enum Unit { M, S }

        private readonly IDatabase _database;
        public Products(IDatabase database)
        {
            _database = database ?? throw new ArgumentException("IDatabase object cannot be null.");
        }

        public async Task<ObservableCollection<T>> GetCategories<T, T2>() where T2 : ProductWapper where T : CategoryWrapper<T2>
        {
            var categoriesData = await _database.GetRowsAsync("category", new Dictionary<string, dynamic> { { "is_deleted", 0 }, });
            var categories = new ObservableCollection<T>();

            foreach (var categorydata in categoriesData)
            {
                var t = Activator.CreateInstance(typeof(T), new Category
                {
                    Id = categorydata["category_id"],
                    ShortDescr = categorydata["short_description"] is DBNull ? "" : categorydata["short_description"],
                    LongDescr = categorydata["long_description"] is DBNull ? "" : categorydata["long_description"]
                }) as T;

                categories.Add(t);
            }
            return categories;
        }

        public async Task<ProductWapper> GetProductBySkuAsync(string sku)
        {
            var product = await _database.GetRowsAsync("product", new Dictionary<string, dynamic>
           {
               {"sku", sku},
               {"is_deleted", 0}
           });

            if (product.Count == 0)
                return null;

            var productWrapper = new ProductWapper(
                new Product
                {
                    Id = product[0]["product_id"],
                    Sku = product[0]["sku"],
                    Description = product[0]["description"],
                    Retail = (double)product[0]["retail"],
                    Discount = (double?)(product[0]["discount"] is double ? product[0]["discount"] : null),
                    Cost = (double?)(product[0]["cost"] is double ? product[0]["cost"] : null),
                    Date = Convert.ToDateTime(product[0]["last_updated"]).Date,
                    MUnit = await GetUnitAsync(product[0], Unit.M),
                    SUnit = await GetUnitAsync(product[0], Unit.S),
                    MattFactor = (double)product[0]["mat_factor"],
                    Waste = (double?)(product[0]["waste"] is double ? product[0]["waste"] : null),
                    Cover = (double?)(product[0]["cover"] is double ? product[0]["cover"] : null),
                    Notes = product[0]["notes"].ToString(),
                    LRate = (double?)(product[0]["l_rate"] is double ? product[0]["l_rate"] : null),
                    RoundFactor = (double?)(product[0]["round_factor"] is double ? product[0]["round_factor"] : null),
                    Retail16 = (double?)(product[0]["retail16"] is double ? product[0]["retail16"] : null)
                });

            return productWrapper;
        }

        public async Task<string> GetUnitAsync(Dictionary<string, dynamic> product, Unit unit)
        {
            var unitTask = await _database.GetRowsAsync("unit", new Dictionary<string, dynamic>
           {
               {"unit_id", product[unit == Unit.S ? "s_unit_id" : "m_unit_id"] }
           });
            if (unitTask.Count == 0)
                return null;
            return unitTask[0]["unit"];

        }

        public async Task<ChangeTrackingCollection<T>> GetProductsBySearchTermAsync<T>(string productSearchTerm) where T : ProductWapper
        {
            var productsData = await _database.SearchRowsAsync(
                $"product",
                new List<string> { "*" },
                new List<string> { "sku", "description", "notes" },
                $"{productSearchTerm}");
            var productsList = new List<T>();
            foreach (var productData in productsData)
            {
                productsList.Add(Activator.CreateInstance(typeof(T), new Product
                {
                    Id = productData["product_id"],
                    Sku = productData["sku"],
                    Description = productData["description"],
                    Retail = (double)productData["retail"],
                    Discount = (double?)(productData["discount"] is double ? productData["discount"] : null),
                    Cost = (double?)(productData["cost"] is double ? productData["cost"] : null),
                    Date = Convert.ToDateTime(productData["last_updated"]).Date,
                    MUnit = await GetUnitAsync(productData, Unit.M),
                    SUnit = await GetUnitAsync(productData, Unit.S),
                    MattFactor = (double)productData["mat_factor"],
                    Waste = (double?)(productData["waste"] is double ? productData["waste"] : null),
                    Cover = (double?)(productData["cover"] is double ? productData["cover"] : null),
                    Notes = productData["notes"].ToString(),
                    LRate = (double?)(productData["l_rate"] is double ? productData["l_rate"] : null),
                    RoundFactor = (double?)(productData["round_factor"] is double ? productData["round_factor"] : null),
                    Retail16 = (double?)(productData["retail16"] is double ? productData["retail16"] : null)
                }) as T);
            }
            return new ChangeTrackingCollection<T>(productsList);
        }

        public async Task<ChangeTrackingCollection<T>> GetProductsByCategoryIdAsync<T>(int categoryId) where T : ProductWapper
        {
            var productsData = await _database.GetRowsAsync("product", new Dictionary<string, dynamic>
            {
                {"category_id", categoryId},
                {"is_deleted", 0}
            });

            var productsList = new List<T>();
            foreach (var product in productsData)
            {
                productsList.Add(Activator.CreateInstance(typeof(T), new Product
                {
                    Id = product["product_id"],
                    Sku = product["sku"],
                    Description = product["description"],
                    Retail = (double)product["retail"],
                    Discount = (double?)(product["discount"] is double ? product["discount"] : null),
                    Cost = (double?)(product["cost"] is double ? product["cost"] : null),
                    Date = Convert.ToDateTime(product["last_updated"]).Date,
                    MUnit = await GetUnitAsync(product, Unit.M),
                    SUnit = await GetUnitAsync(product, Unit.S),
                    MattFactor = (double)product["mat_factor"],
                    Waste = (double?)(product["waste"] is double ? product["waste"] : null),
                    Cover = (double?)(product["cover"] is double ? product["cover"] : null),
                    Notes = product["notes"].ToString(),
                    LRate = (double?)(product["l_rate"] is double ? product["l_rate"] : null),
                    RoundFactor = (double?)(product["round_factor"] is double ? product["round_factor"] : null),
                    Retail16 = (double?)(product["retail16"] is double ? product["retail16"] : null)
                }) as T);
            }
            return new ChangeTrackingCollection<T>(productsList);
        }

        public async Task<ObservableCollection<T1>> SearchCategoriesByKeywordAsync<T1, T2>(string categorySearchTerm) where T2 : ProductWapper where T1 : CategoryWrapper<T2>
        {
            var categoriesData = await _database.SearchRowsAsync(
                $"category",
                new List<string> { "*" },
                new List<string> { "short_description", "long_description" },
                $"{categorySearchTerm}");
            var categories = new ObservableCollection<T1>();
            foreach (var categorydata in categoriesData)
            {
                var t = Activator.CreateInstance(typeof(T1), new Category
                {
                    Id = categorydata["category_id"],
                    ShortDescr = categorydata["short_description"] is DBNull ? "" : categorydata["short_description"],
                    LongDescr = categorydata["long_description"] is DBNull ? "" : categorydata["long_description"]
                }) as T1;

                categories.Add(t);
            }
            return categories;
        }
    }
}