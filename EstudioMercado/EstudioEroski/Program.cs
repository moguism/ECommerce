using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Xml;
using Microsoft.Playwright;

namespace EstudioEroski
{
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

            // Ir a la página de Eroski
            await page.GotoAsync("https://supermercado.eroski.es/");

            await Task.Delay(2500);
            // Escribimos en la barra de búsqueda lo que queremos buscar
            IElementHandle aceptarGalletas = await page.QuerySelectorAsync("#onetrust-accept-btn-handler");
            await aceptarGalletas.ClickAsync();
            
            // ponemos el buscador
            IElementHandle Buscador = await page.QuerySelectorAsync("#searchTerm");
            await Buscador.FillAsync("manzana");

            //paso extra porque Para que aparezca el botno para buscar
            IElementHandle botonform = await page.QuerySelectorAsync(".not_clickable");
            await botonform.ClickAsync();

            //boton para buscar
            IElementHandle botonbuscar = await page.QuerySelectorAsync(".search-button");
            await botonbuscar.ClickAsync();

            //botones paraa navegar hasta donde solo me aprecen manzanas que son fruta
            await Task.Delay(1500);
            IElementHandle botonfresco = await page.QuerySelectorAsync(".menu-selector >> text='Frescos (26) '");
            await botonfresco.ClickAsync();

            //botones paraa navegar hasta donde solo me aprecen manzanas que son fruta
            await Task.Delay(1500);
            IElementHandle botonfruta = await page.QuerySelectorAsync(".menu-selector >> text='Frutas (20) '");
            await botonfruta.ClickAsync();

            //botones para navegar hasta donde solo me aprecen manzanas que son fruta
            await Task.Delay(1500);
            IElementHandle botonmanzanas = await page.QuerySelectorAsync(".menu-selector >> text='Manzanas y peras  (13) '");
            await botonmanzanas.ClickAsync();

            await Task.Delay(3000);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            //await Task.Delay(-1);

            List<Product> products = new List<Product>();
            IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("div.product-item");
            int count = 0;
            foreach (IElementHandle productElement in productElements )
            {
                if (count == 10)
                {
                    break;
                }
                try
                {
                    
                    Product product = await GetProductAsync(productElement);
                    if (product!=null)
                    {
                    products.Add(product);
                    Console.WriteLine(product);
                    count++;
                    }
                    
                }
                catch (Exception e) { }

            }
            // Con los datos recolectados, buscamos el producto más barato
            Product cheapest = products.MinBy(p => p.Price);
            Console.WriteLine($"La oferta más barata es: {cheapest}");
            Product mayor = products.MaxBy(p => p.Price);
            Console.WriteLine($"La oferta más cara es: {mayor}");
            decimal media = products.Average(p => p.Price);
            Console.WriteLine($"La media de precio de los prodcutos es: {media}");
        }
        private static async Task<Product> GetProductAsync(IElementHandle element)
        {
            IElementHandle priceElement = await element.QuerySelectorAsync(".price-offer-now");
            if (priceElement==null)
            {
                return null;
            }
            string priceRaw = await priceElement.InnerTextAsync();
            //priceRaw = priceRaw.Replace("EUR", "", StringComparison.OrdinalIgnoreCase);
            priceRaw = priceRaw.Trim();
            priceRaw = priceRaw.Replace(".", "");
            decimal price = decimal.Parse(priceRaw);
            IElementHandle nameElement = await element.QuerySelectorAsync(".product-title a");
            string name = await nameElement.InnerTextAsync();


            return new Product(name,price);
        }
    }
}

