using System.ComponentModel;
using System;
using Microsoft.SemanticKernel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plugins;
public class ProductPlugin
{
    [KernelFunction]
    [Description("Returns a list of products.")]
    public async Task<string> GetProducts()
    {
        var url = Settings.LoadUrlFromFile();
       
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        return content;
    }

    
    [KernelFunction]
    [Description("Returns a specific product, from a list of products, by the product ID.")]
    public async Task<string> GetProductById(
        [Description("The ID of the Product")] int id
    )
    {
        var url = Settings.LoadUrlFromFile();
        
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
    
        return content;
    }



    [KernelFunction]
    [Description("Creates a new product")]
    public async Task<string> CreateProduct(
        [Description("The name of the product")] string name,
        [Description("The category of the product")] string category,
        [Description("The price of the product")] string price,
        [Description("The stock of the product")] int stock

    )
    {
        var url = Settings.LoadUrlFromFile();
        
        HttpClient client = new HttpClient();

        // Create the new product
        var product = new { name = name, category = category, price = price, stock = stock };

        // Convert the product to JSON
        var json = JsonConvert.SerializeObject(product);

        var data = new StringContent(json, Encoding.UTF8, "application/json");

        // Getting a response
        var response = await client.PostAsync(url, data);

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}