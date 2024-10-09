using System.Diagnostics;
using Microsoft.Playwright;

namespace EstudioLaFruteria;

internal class Program {
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

        // Ir a la página de La Fruteria
        await page.GotoAsync("https://www.lafruteria.es/");

        // Escribimos en la barra de búsqueda lo que queremos buscar
        IElementHandle searchInput = await page.QuerySelectorAsync("#search_query_top");
        await searchInput.FillAsync("manzana");

        // Le damos al botón de buscar
        IElementHandle searchButton = await page.QuerySelectorAsync("#searchbox > button");
        await searchButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Recorremos la lista de productos y recolectamos los datos
        List<Product> products = new List<Product>();
        IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("#ajax_block_product");
        foreach (IElementHandle productElement in productElements) {
            try {
                Product product = await GetProductAsync(productElement);
                products.Add(product);
                Console.WriteLine(product);
            } catch (Exception e)
            { 
            }
        }

        // Con los datos recolectados, buscamos el producto más barato
        Product cheapest = products.MinBy(p => p.Price);
        Console.WriteLine($"La oferta más barata es: {cheapest}");

    }

    private static async Task<Product> GetProductAsync(IElementHandle element) {
        IElementHandle priceElement = await element.QuerySelectorAsync(".product-price");
        string priceRaw = await priceElement.InnerTextAsync();
        priceRaw = priceRaw.Replace("kg", "", StringComparison.OrdinalIgnoreCase);
        priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);
        priceRaw = priceRaw.Replace("/", "", StringComparison.OrdinalIgnoreCase);
        priceRaw = priceRaw.Replace(",", ".");
        priceRaw = priceRaw.Trim();
        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".product-name title");
        string name = await nameElement.InnerTextAsync();

        return new Product(name, price);
    }
}
