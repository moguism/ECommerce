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

        await Task.Delay(-1);

        // Recorremos la lista de productos y recolectamos los datos
        List<Product> products = new List<Product>();
        IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("#center_column > ul");
        foreach (IElementHandle productElement in productElements) {
            try {
                Product product = await GetProductAsync(productElement);
                products.Add(product);
                Console.WriteLine(product);
            } catch { }
        }

        // Con los datos recolectados, buscamos el producto más barato
        Product cheapest = products.MinBy(p => p.Price);
        Console.WriteLine($"La oferta más barata es: {cheapest}");

        // Abrimos el navegador con la oferta más barata
        ProcessStartInfo processInfo = new ProcessStartInfo() {
            FileName = cheapest.Url,
            UseShellExecute = true
        };
        Process.Start(processInfo);

    }

    private static async Task<Product> GetProductAsync(IElementHandle element) {
        IElementHandle priceElement = await element.QuerySelectorAsync(".s-item__price");
        string priceRaw = await priceElement.InnerTextAsync();
        priceRaw = priceRaw.Replace("EUR", "", StringComparison.OrdinalIgnoreCase);
        priceRaw = priceRaw.Trim();
        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".s-item__title");
        string name = await nameElement.InnerTextAsync();

        IElementHandle urlElement = await element.QuerySelectorAsync("a");
        string url = await urlElement.GetAttributeAsync("href");

        return new Product(name, url, price);
    }
}
