using Microsoft.Playwright;
using System.Diagnostics;

namespace EstudioMasMit_Carniceria;


internal class Program
{

    public static Dictionary<string, List<Product>> productos = new Dictionary<string, List<Product>>()
    {
        {"pechuga de pollo", new List<Product>()},
        {"muslo de pollo",  new List<Product>()},
        {"solomillo de cerdo",  new List<Product>()},
        {"cinta de lomo",  new List<Product>()}
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



        // Escribimos en la barra de búsqueda lo que queremos buscar
        IElementHandle searchInput = await page.QuerySelectorAsync("#leo_search_query_top");
        await searchInput.FillAsync("pollo");

        
        // Le damos al botón de buscar
        IElementHandle searchButton = await page.QuerySelectorAsync(".fa-search");
        await searchButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        

        /*
        // Recorremos la lista de productos y recolectamos los datos
        List<Product> products = new List<Product>();
        IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("ul li.s-item");

        //Recorre la lista con todos los elementos html de losproductos
        foreach (IElementHandle productElement in productElements)
        {
            try
            {
                //Añade la la lista con los productos el 
                Product product = await GetProductAsync(productElement);
                products.Add(product);
                Console.WriteLine(product);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        */
        // Espera infinita
        await Task.Delay(-1);

        // Cerrar el navegador
        await browser.CloseAsync();
    }
}
