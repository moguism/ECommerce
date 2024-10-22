using System.Diagnostics;
using Microsoft.Playwright;

namespace EstudioLaFruteria;

internal class Program {
    public static Dictionary<string, List<Product>> productos = new Dictionary<string, List<Product>>()
    {
        {"manzana",  new List<Product>()},
        {"pera",  new List<Product>()}

    };
    static async Task Main(string [] args) {

        // Necesario para instalar los navegadores
        Microsoft.Playwright.Program.Main(["install"]);

        using IPlaywright playwright = await Playwright.CreateAsync();
        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions() {
            Headless = false // Se indica falso para poder ver el navegador
        };
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options);
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        foreach (var item in productos.Keys)
        {
            List<decimal> listaPrecios = new List<decimal>();
            // Ir a la página de La Fruteria
            await page.GotoAsync("https://www.lafruteria.es/buscar?search_query=" + item + "&submit_search=&orderby=quantity&orderway=desc");

            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            await Task.Delay(2000);

            // Recorremos la lista de productos y recolectamos los datos
            
            IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync(".ajax_block_product");
            foreach (IElementHandle productElement in productElements)
            {
                try
                {
                    Product product = await GetProductAsync(productElement);
                    if (product != null && product.Name.ToLower().Contains(item))
                    {
                        productos[item].Add(product);
                        listaPrecios.Add(product.Price);
                        Console.WriteLine(product);
                    }
                }
                catch (Exception e)
                {
                }
            }

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"Maximo : {listaPrecios.Max()} ----- Mínimo : {listaPrecios.Min()} ------ Media : {listaPrecios.Average()}");
            Console.WriteLine("--------------------------------------------------------------------------------");

        }
        

    }

    private static async Task<Product>? GetProductAsync(IElementHandle element) {
            IElementHandle priceElement = await element.QuerySelectorAsync(".product-price");

            if (priceElement == null) { return null; }

            string priceRaw = await priceElement.InnerTextAsync();
            priceRaw = priceRaw.Replace("kg", "", StringComparison.OrdinalIgnoreCase);
            priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);
            priceRaw = priceRaw.Replace("/", "", StringComparison.OrdinalIgnoreCase);
            //priceRaw = priceRaw.Replace(",", ".");
            priceRaw = priceRaw.Trim();
            decimal price = decimal.Parse(priceRaw);

            IElementHandle nameElement = await element.QuerySelectorAsync(".product-name");
            string name = await nameElement.InnerTextAsync();

            return new Product(name, price);
        
    }
}
