using Microsoft.Playwright;

namespace EstudioMercado;

internal class Program
{
    public static Dictionary<string, Dictionary<string, decimal>> productos = new Dictionary<string, Dictionary<string, decimal>>()
    {
        {"manzana", new Dictionary<string, decimal>(){
            {"precioMinimo", 0},
            {"precioMaximo", 0},
            {"media", 0}
        }},
        {"cinta de lomo", new Dictionary<string, decimal>(){
            {"precioMinimo", 0},
            {"precioMaximo", 0},
            {"media", 0}
        }},
        {"chuleta de cordero", new Dictionary<string, decimal>(){
            {"precioMinimo", 0},
            {"precioMaximo", 0},
            {"media", 0}
        }},
        {"pera", new Dictionary<string, decimal>(){
            {"precioMinimo", 0},
            {"precioMaximo", 0},
            {"media", 0}
        }}
    };

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

        foreach(var item in productos)
        {
            IElementHandle searchInput = await page.QuerySelectorAsync(".dia-search__bar");
            await searchInput.FillAsync(item.Key);

            await Task.Delay(1000); // Necesario, por lo menos en DIA
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            List<Product> products = new List<Product>();
            IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("ul li.search-product-card-list__item-container");

            int productosVistos = 0;

            foreach (IElementHandle productElement in productElements)
            {
                if(productosVistos == 10)
                {
                    break;
                }

                try
                {
                    Product product = await GetProductAsync(productElement);
                    if (product != null)
                    {
                        products.Add(product);
                        productosVistos++;
                        //Console.WriteLine(product);
                    }
                }
                catch (Exception e)
                {
                    // Para poder monitorear excepciones
                }
            }

            Product cheapest = products.MinBy(p => p.Price);
            Product mostExpensive = products.MaxBy(p => p.Price);
            decimal average = products.Average(p => p.Price);
            productos[item.Key]["precioMinimo"] = cheapest.Price;
            productos[item.Key]["precioMaximo"] = mostExpensive.Price;
            productos[item.Key]["media"] = average;

            Console.WriteLine($"EL PRODUCTO \"{item.Key}\" MÁS BARATO ES: \n{cheapest}\nCuesta {productos[item.Key]["precioMinimo"]}");
            Console.WriteLine($"EL PRODUCTO \"{item.Key}\" MÁS CARO ES: \n{mostExpensive}\nCuesta {productos[item.Key]["precioMaximo"]}");
            Console.WriteLine($"LA MEDIA DE PRECIOS DEL PRODUCTO \"{item.Key}\" ES: {productos[item.Key]["media"]}");
            Console.WriteLine("");
        }

        await Task.Delay(-1);
    }

    private static async Task<Product> GetProductAsync(IElementHandle element)
    {
        IElementHandle priceElement = await element.QuerySelectorAsync("p.search-product-card__active-price");
        if (priceElement == null) return null;
        string priceRaw = await priceElement.InnerTextAsync();

        priceRaw = priceRaw.Replace("€", "").Replace("(", "").Replace(")", "");
        priceRaw = priceRaw.Replace(".", ",");
        priceRaw = priceRaw.Trim();

        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".search-product-card__product-name");
        string name = await nameElement.InnerTextAsync();

        return new Product(name, price);
    }
}

