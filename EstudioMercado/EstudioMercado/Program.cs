using Microsoft.Playwright;
using System.Diagnostics;

namespace EstudioMercado;

    internal class Program
    {
    static async Task Main(string[] args)
    {
        Microsoft.Playwright.Program.Main(["install"]);
        using IPlaywright playwright = await Playwright.CreateAsync();
        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions()
        {
            Headless = false
        };
        
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options);
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        await page.GotoAsync("https://www.dia.es/");
        await Task.Delay(2000); // Para que salte la confirmacíón de cookies

        IElementHandle? acceptButton = await page.QuerySelectorAsync("#onetrust-accept-btn-handler");
        if (acceptButton != null) await acceptButton.ClickAsync();

        IElementHandle searchInput = await page.QuerySelectorAsync(".dia-search__bar");
        await searchInput.FillAsync("patatas");

        await Task.Delay(1000); // Necesario, por lo menos en DIA

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        List<Product> products = new List<Product>();

        IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("ul li.search-product-card-list__item-container");

        foreach (IElementHandle productElement in productElements)
        {
            try
            {
                Product product = await GetProductAsync(productElement);
                if(product != null) products.Add(product);
                Console.WriteLine(product);
            }
            catch (Exception e)
            {
                // Para poder monitorear excepciones
            }
        }

        Product cheapest = products.MinBy(p => p.Price);
        Console.WriteLine($"La oferta más barata es: {cheapest}");
    }

    private static async Task<Product> GetProductAsync(IElementHandle element) 
    {
        IElementHandle priceElement = await element.QuerySelectorAsync("p.search-product-card__active-price");
        if (priceElement == null) return null;
        string priceRaw = await priceElement.InnerTextAsync();

        priceRaw = priceRaw.Replace("/KILO", "", StringComparison.OrdinalIgnoreCase).Replace("€", "").Replace("(", "").Replace(")", "");
        priceRaw = priceRaw.Replace(",", ".");
        priceRaw = priceRaw.Trim();

        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".search-product-card__product-name");
        string name = await nameElement.InnerTextAsync();

        return new Product(name, price);
    }
}

