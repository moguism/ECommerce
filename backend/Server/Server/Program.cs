using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Mappers;
using Server.Models;
using Server.Services;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            
            // CONFIGURANDO JWT
            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        // INDICAMOS LA CLAVE
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<FarminhouseContext>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<UserMapper>();
            builder.Services.AddScoped<ProductMapper>();
            builder.Services.AddScoped<PasswordService>();


            // Permite CORS
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddCors(
                    options =>
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                            ;
                        })
                    );
            }

            var app = builder.Build();

            //PA QUE FUNCIONE EL WWWROOT NO LO TOQUEIS HIJOS DE PUTA
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                // Permite CORS
                app.UseCors();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                FarminhouseContext dbContext = scope.ServiceProvider.GetService<FarminhouseContext>();
                if (dbContext.Database.EnsureCreated())
                {
                    // Categorías
                    Category fruitsCategory = new Category { Name = "fruits" };
                    Category vegetablesCategory = new Category { Name = "vegetables" };
                    Category meatCategory = new Category { Name = "meat" };

                    // Frutas
                    var fruits = new List<Product>
                    {
                        new Product { Name = "Manzana", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 2.5, Stock = 150, Average = 2.5, image = "manzana.jpg", CategoryId = 1 },
                        new Product { Name = "Plátano", Description = "Una fuente rápida de energía, perfecta para llevar.", Price = 1.5, Stock = 200, Average = 4, image = "platano.jpg", CategoryId = 1 },
                        new Product { Name = "Naranja", Description = "Cítrico jugoso y refrescante, rico en vitamina C.", Price = 3, Stock = 180, Average = 3.5, image = "naranja.jpg", CategoryId = 1 },
                        new Product { Name = "Fresa", Description = "Fruta dulce y roja, excelente en postres y batidos.", Price = 4, Stock = 120, Average = 4.5, image = "fresa.jpg", CategoryId = 1 },
                        new Product { Name = "Kiwi", Description = "Fruta exótica con un sabor único y refrescante.", Price = 3.5, Stock = 90, Average = 4, image = "kiwi.jpg", CategoryId = 1 },
                        new Product { Name = "Pera", Description = "Fruta suave y jugosa, ideal para ensaladas.", Price = 2.8, Stock = 160, Average = 4.2, image = "pera.jpg", CategoryId = 1 },
                        new Product { Name = "Arandano", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 2.5, Stock = 150, Average = 2.5, image = "manzana.jpg", CategoryId = 1 },
                        new Product { Name = "Uva", Description = "Pequeñas frutas dulces, perfectas para picar o hacer vino.", Price = 5, Stock = 130, Average = 4.3, image = "uva.jpg", CategoryId = 1 },
                        new Product { Name = "Sandía", Description = "Fruta refrescante y jugosa, perfecta para el verano.", Price = 6, Stock = 75, Average = 4.8, image = "sandia.jpg", CategoryId = 1 },
                        new Product { Name = "Melón", Description = "Fruta dulce y jugosa, ideal para ensaladas de frutas.", Price = 4.5, Stock = 80, Average = 4.1, image = "melon.jpg", CategoryId = 1 },
                        new Product { Name = "Mango", Description = "Fruta tropical dulce, perfecta para smoothies y postres.", Price = 3.2, Stock = 110, Average = 4.4, image = "mango.jpg", CategoryId = 1 }
                    };

                    // Verduras
                    var vegetables = new List<Product>
                    {
                        new Product { Name = "Apio", Description = "Vegetal crujiente, ideal para dips y ensaladas.", Price = 3, Stock = 200, Average = 4, image = "apio.jpg", CategoryId = 2 },
                        new Product { Name = "Zanahoria", Description = "Dulce y crujiente, rica en vitamina A, perfecta para snacks.", Price = 1.5, Stock = 180, Average = 4.2, image = "zanahoria.jpg", CategoryId = 2 },
                        new Product { Name = "Tomate", Description = "Versátil y jugoso, excelente para ensaladas y salsas.", Price = 2, Stock = 150, Average = 4.5, image = "tomate.jpg", CategoryId = 2 },
                        new Product { Name = "Lechuga", Description = "Base fresca para ensaladas, ligera y nutritiva.", Price = 1.2, Stock = 220, Average = 4.1, image = "lechuga.jpg", CategoryId = 2 },
                        new Product { Name = "Cebolla", Description = "Sabor fuerte y característico, ideal para sazonar.", Price = 1.8, Stock = 170, Average = 4.3, image = "cebolla.jpg", CategoryId = 2 },
                        new Product { Name = "Pimiento", Description = "Dulce y crujiente, perfecto para saltear o asar.", Price = 2.5, Stock = 140, Average = 4, image = "pimiento.jpg", CategoryId = 2 },
                        new Product { Name = "Brócoli", Description = "Vegetal verde lleno de nutrientes, ideal al vapor.", Price = 2.8, Stock = 100, Average = 4.6, image = "brocoli.jpg", CategoryId = 2 },
                        new Product { Name = "Espinaca", Description = "Hoja verde rica en hierro, excelente para ensaladas y guisos.", Price = 3.2, Stock = 90, Average = 4.4, image = "espinaca.jpg", CategoryId = 2 },
                        new Product { Name = "Coliflor", Description = "Vegetal versátil, ideal para purés y como sustituto del arroz.", Price = 2.7, Stock = 80, Average = 4.3, image = "coliflor.jpg", CategoryId = 2 },
                        new Product { Name = "Berenjena", Description = "Sabor único, excelente para asar y guisar.", Price = 3.5, Stock = 70, Average = 4.1, image = "berenjena.jpg", CategoryId = 2 }
                    };

                    // Carnes
                    var meats = new List<Product>
                    {
                        new Product { Name = "Ternera", Description = "Carne tierna y jugosa, ideal para guisos y asados.", Price = 5.5, Stock = 100, Average = 3, image = "ternera.jpg", CategoryId = 3 },
                        new Product { Name = "Pollo", Description = "Carne magra, versátil y rica en proteínas.", Price = 4, Stock = 150, Average = 3.5, image = "pollo.jpg", CategoryId = 3 },
                        new Product { Name = "Cerdo", Description = "Carne sabrosa, ideal para parrillas y guisos.", Price = 6, Stock = 80, Average = 3.8, image = "cerdo.jpg", CategoryId = 3 },
                        new Product { Name = "Cordero", Description = "Carne rica y suculenta, perfecta para asados.", Price = 7, Stock = 60, Average = 4, image = "cordero.jpg", CategoryId = 3 },
                        new Product { Name = "Pavo", Description = "Alternativa magra al pollo, ideal para celebraciones.", Price = 5.5, Stock = 90, Average = 3.9, image = "pavo.jpg", CategoryId = 3 },
                        new Product { Name = "Conejo", Description = "Carne suave y saludable, ideal para guisos.", Price = 8, Stock = 50, Average = 4.2, image = "conejo.jpg", CategoryId = 3 },
                        new Product { Name = "Salchicha", Description = "Deliciosa y jugosa, perfecta para barbacoas.", Price = 2.5, Stock = 200, Average = 3.7, image = "salchicha.jpg", CategoryId = 3 },
                        new Product { Name = "Bacon", Description = "Crujiente y sabroso, ideal para desayunos y burgers.", Price = 4.5, Stock = 120, Average = 4.3, image = "bacon.jpg", CategoryId = 3 },
                        new Product { Name = "Panceta", Description = "Sabrosa y jugosa, perfecta para añadir sabor a tus platos.", Price = 5, Stock = 70, Average = 4, image = "panceta.jpg", CategoryId = 3 },
                        new Product { Name = "Atún", Description = "Pescado rico en omega-3, ideal para ensaladas y sushi.", Price = 6.5, Stock = 40, Average = 4.1, image = "atun.jpg", CategoryId = 3 }
                    };

                    // Añadir categorías y productos al contexto de la base de datos
                    dbContext.Categories.Add(fruitsCategory);
                    dbContext.Categories.Add(vegetablesCategory);
                    dbContext.Categories.Add(meatCategory);
                    dbContext.Products.AddRange(fruits);
                    dbContext.Products.AddRange(vegetables);
                    dbContext.Products.AddRange(meats);

                    // Guardar cambios en la base de datos
                    dbContext.SaveChanges();

                }
            }


            app.Run();
        }
    }
}
