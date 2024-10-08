using Microsoft.Playwright;
using System.Diagnostics;

namespace EstudioMasMit_Carniceria;


internal class Program
{

    public static Dictionary<string, List<Product>> productos = new Dictionary<string, List<Product>>()
    {
        {"pechuga de pollo", new List<Product>() },
        {"muslo de pollo",  new List<Product>()},
        {"solomillo de cerdo",  new List<Product>()},
        {"cinta de lomo",  new List<Product>()},
        {"chuleta de cordero",  new List<Product>()}

    };

    static async Task Main(string[] args)
    {
        // Necesario para instalar los navegadores
        Microsoft.Playwright.Program.Main(["install"]);

        using IPlaywright playwright = await Playwright.CreateAsync();

        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions()
        {
            Headless = false // Se indica falso para poder ver el navegador
        };

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options);
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        // Ir a la página de la carniceria Masmit
        await page.GotoAsync("https://masmit.com/");

        // Para que salte la confirmacíón de cookies
        await Task.Delay(1000);
        IElementHandle? acceptButton = await page.QuerySelectorAsync(".accept-button");
        if (acceptButton != null) await acceptButton.ClickAsync();
        await Task.Delay(2000);

        //Recorre la lista con todos los productos que queremos buscar
        foreach (var item in productos)
        {
            List<decimal> listaPrecios = new List<decimal>();

            // Escribimos en la barra de búsqueda lo que queremos buscar
            IElementHandle searchInput = await page.QuerySelectorAsync("#leo_search_query_top");
            await searchInput.FillAsync(item.Key);

            // Le damos al botón de buscar
            IElementHandle searchButton = await page.QuerySelectorAsync(".fa-search");
            await searchButton.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            await Task.Delay(1000); //Para poder ver los productos 

            //Lista con los productos en formato HTML
            IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync(".ajax_block_product");

            //Recorre los productos 
            foreach(var element in productElements)
            {
                try
                {
                    Product product = await GetProductAsync(element);
                    if (product != null)
                    {
                        item.Value.Add(product); //Añade a la lista un nuevo producto
                        listaPrecios.Add(product.Price);
                        Console.WriteLine(product);
                    }
                }
                catch (Exception e)
                {
                    // Para poder monitorear excepciones
                }
            }

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"Maximo : {listaPrecios.Max()} ----- Mínimo : {listaPrecios.Min()} ------ Media : {listaPrecios.Average()}");
            Console.WriteLine("--------------------------------------------------------------------------------");



        }

        // Espera infinita
        await Task.Delay(-1);
        // Cerrar el navegador
        await browser.CloseAsync();
    }

    private static async Task<Product> GetProductAsync(IElementHandle element)
    {
        IElementHandle priceElement = await element.QuerySelectorAsync(".product-price");
        string priceRaw = await priceElement.InnerTextAsync(); // Obtiene el texto de dentro de la etiqueta

        priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);
        //priceRaw = priceRaw.Replace(".", ",");
        priceRaw = priceRaw.Trim();

        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".product-name");
        string name = await nameElement.InnerTextAsync();


        return new Product(name, price);
    }

}
