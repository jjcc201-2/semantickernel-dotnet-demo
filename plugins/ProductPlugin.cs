using System.ComponentModel;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace Plugins;
public class ProductPlugin
{

    [KernelFunction]
    [Description("Returns a list of products.")]
    public async Task<List<Product>> GetProducts()
    {
        var url = "https://demoproductapi.azurewebsites.net/product";
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<List<Product>>(content);
        
        return result;
    }
}