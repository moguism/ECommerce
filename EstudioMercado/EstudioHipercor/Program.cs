using Microsoft.Playwright;
using System.Diagnostics;

namespace EstudioHipercor;

internal class Program
{
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

        // Ir a la página de ebay
        await page.GotoAsync("https://www.hipercor.es/supermercado/?gad_source=1&gclid=Cj0KCQjwsJO4BhDoARIsADDv4vARhSCsZmFi7rccsdRM9BTpZNylQtcYNaOg02BlwymNl1DpGohiNb0aAi5xEALw_wcB&gclsrc=aw.ds");
 

        // Escribimos en la barra de búsqueda lo que queremos buscar
        IElementHandle searchInput = await page.QuerySelectorAsync(".search-input");
        await searchInput.FillAsync("patata");

        /*
        // Le damos al botón de buscar
        IElementHandle searchButton = await page.QuerySelectorAsync(".search");
        await searchButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        */

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

    private static async Task<Product> GetProductAsync(IElementHandle element)
    {
        IElementHandle priceElement = await element.QuerySelectorAsync("p.search-product-card__active-price");
        if (priceElement == null) return null;
        string priceRaw = await priceElement.InnerTextAsync();

        priceRaw = priceRaw.Replace("/KILO", "", StringComparison.OrdinalIgnoreCase).Replace("€", "").Replace("(", "").Replace(")", "");
        priceRaw = priceRaw.Replace(".", ",");
        priceRaw = priceRaw.Trim();

        decimal price = decimal.Parse(priceRaw);

        IElementHandle nameElement = await element.QuerySelectorAsync(".search-product-card__product-name");
        string name = await nameElement.InnerTextAsync();

        return new Product(name, price);
    }















}


