using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
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

            builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<Settings>>().Value);

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

            // Para el modelo de IA
            builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
                .FromFile("IAFarminhouse.mlnet");

            builder.Services.AddScoped<FarminhouseContext>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<UserMapper>();
            builder.Services.AddScoped<ProductMapper>();
            builder.Services.AddScoped<PasswordService>();
            builder.Services.AddScoped<SmartSearchService>();
            builder.Services.AddScoped<ShoppingCartMapper>();
            builder.Services.AddScoped<ShoppingCartService>();
            builder.Services.AddScoped<ReviewService>();
            builder.Services.AddScoped<ReviewMapper>();
            builder.Services.AddScoped<TemporalOrderMapper>();
            builder.Services.AddScoped<TemporalOrderService>();
            builder.Services.AddScoped<CartContentMapper>();
            //builder.Services.AddHostedService<CleanTemporalOrdersService>();
            // Aqui esta la clave privada
            Stripe.StripeConfiguration.ApiKey = "sk_test_51QJzjI2MpRBL4z2Cyh3NiBYhF4kXzVk7QJppRv2cAwoM8vPFrDwUjKnwZOiIDw0yYZfzNxNybQWenGMmmj83NunP00UGENKK29";
            builder.Services.AddHostedService<CleanTemporalOrdersService>();

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
                        new Product { Name = "Manzana", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 250, Stock = 150, Average = 250, Image = "manzana.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Plátano", Description = "Una fuente rápida de energía, perfecta para llevar.", Price = 150, Stock = 200, Average = 400, Image = "platano.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Naranja", Description = "Cítrico jugoso y refrescante, rico en vitamina C.", Price = 300, Stock = 0, Average = 350, Image = "naranja.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Fresa", Description = "Fruta dulce y roja, excelente en postres y batidos.", Price = 400, Stock = 120, Average = 450, Image = "fresa.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Kiwi", Description = "Fruta exótica con un sabor único y refrescante.", Price = 350, Stock = 90, Average = 400, Image = "kiwi.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Pera", Description = "Fruta suave y jugosa, ideal para ensaladas.", Price = 280, Stock = 160, Average = 420, Image = "pera.jpg", CategoryId = 1, Category = fruitsCategory},
                        new Product { Name = "Arandano", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 250, Stock = 150, Average = 250, Image = "arandano.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Uva", Description = "Pequeñas frutas dulces, perfectas para picar o hacer vino.", Price = 500, Stock = 130, Average = 430, Image = "uva.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Sandía", Description = "Fruta refrescante y jugosa, perfecta para el verano.", Price = 600, Stock = 75, Average = 480, Image = "sandia.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Melón", Description = "Fruta dulce y jugosa, ideal para ensaladas de frutas.", Price = 450, Stock = 80, Average = 410, Image = "melon.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Mango", Description = "Fruta tropical dulce, perfecta para smoothies y postres.", Price = 320, Stock = 110, Average = 440, Image = "mango.jpg", CategoryId = 1, Category = fruitsCategory }
                    };

                    // Verduras
                    var vegetables = new List<Product>
                    {
                        new Product { Name = "Apio", Description = "Vegetal crujiente, ideal para dips y ensaladas.", Price = 300, Stock = 200, Average = 400, Image = "apio.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Zanahoria", Description = "Dulce y crujiente, rica en vitamina A, perfecta para snacks.", Price = 150, Stock = 180, Average = 420, Image = "zanahoria.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Tomate", Description = "Versátil y jugoso, excelente para ensaladas y salsas.", Price = 200, Stock = 150, Average = 450, Image = "tomate.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Lechuga", Description = "Base fresca para ensaladas, ligera y nutritiva.", Price = 120, Stock = 220, Average = 410, Image = "lechuga.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Cebolla", Description = "Sabor fuerte y característico, ideal para sazonar.", Price = 180, Stock = 170, Average = 430, Image = "cebolla.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Pimiento", Description = "Dulce y crujiente, perfecto para saltear o asar.", Price = 250, Stock = 140, Average = 400, Image = "pimiento.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Brócoli", Description = "Vegetal verde lleno de nutrientes, ideal al vapor.", Price = 280, Stock = 0, Average = 460, Image = "brocoli.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Espinaca", Description = "Hoja verde rica en hierro, excelente para ensaladas y guisos.", Price = 320, Stock = 90, Average = 440, Image = "espinaca.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Coliflor", Description = "Vegetal versátil, ideal para purés y como sustituto del arroz.", Price = 270, Stock = 80, Average = 430, Image = "coliflor.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Berenjena", Description = "Sabor único, excelente para asar y guisar.", Price = 350, Stock = 70, Average = 410, Image = "berenjena.jpg", CategoryId = 2, Category = vegetablesCategory }
                    };

                    // Carnes
                    var meats = new List<Product>
                    {
                        new Product { Name = "Ternera", Description = "Carne tierna y jugosa, ideal para guisos y asados.", Price = 550, Stock = 100, Average = 300, Image = "ternera.jpg", CategoryId = 3, Category = meatCategory},
                        new Product { Name = "Pollo", Description = "Carne magra, versátil y rica en proteínas.", Price = 400, Stock = 150, Average = 350, Image = "pollo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Cerdo", Description = "Carne sabrosa, ideal para parrillas y guisos.", Price = 600, Stock = 80, Average = 380, Image = "cerdo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Cordero", Description = "Carne rica y suculenta, perfecta para asados.", Price = 700, Stock = 60, Average = 400, Image = "cordero.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Pavo", Description = "Alternativa magra al pollo, ideal para celebraciones.", Price = 550, Stock = 90, Average = 390, Image = "pavo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Conejo", Description = "Carne suave y saludable, ideal para guisos.", Price = 800, Stock = 50, Average = 420, Image = "conejo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Salchicha", Description = "Deliciosa y jugosa, perfecta para barbacoas.", Price = 250, Stock = 200, Average = 370, Image = "salchicha.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Bacon", Description = "Crujiente y sabroso, ideal para desayunos y burgers.", Price = 450, Stock = 0, Average = 430, Image = "bacon.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Panceta", Description = "Sabrosa y jugosa, perfecta para añadir sabor a tus platos.", Price = 500, Stock = 70, Average = 400, Image = "panceta.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Atún", Description = "Pescado rico en omega-3, ideal para ensaladas y sushi.", Price = 650, Stock = 40, Average = 410, Image = "atun.jpg", CategoryId = 3, Category = meatCategory }
                    };


                   

                    // Añadir categorías y productos al contexto de la base de datos
                    dbContext.Categories.Add(fruitsCategory);
                    dbContext.Categories.Add(vegetablesCategory);
                    dbContext.Categories.Add(meatCategory);
                    dbContext.Products.AddRange(fruits);
                    dbContext.Products.AddRange(vegetables);
                    dbContext.Products.AddRange(meats);



                    // Obtener el producto de Arándano
                    var arandanoProduct = fruits.FirstOrDefault(p => p.Name == "Arandano");

                    if (arandanoProduct != null)
                    {
                        PasswordService passwordService = new PasswordService();
                        // Crear usuarios de ejemplo
                        var user1 = new User { Name = "Carlos", Email = "carlos@example.com", Password = passwordService.Hash("123456"), Role = "Admin", Address = "Calle 123" };
                        var user2 = new User { Name = "Ana", Email = "ana@example.com", Password = "pass456", Role = "Customer", Address = "Avenida 456" };

                        // Asegurarse de que los usuarios están añadidos al contexto
                        dbContext.Users.Add(user1);
                        dbContext.Users.Add(user2);

                        // Crear reseñas para el producto de arándano
                        var review1 = new Review
                        {
                            Text = "Los mejores arándanos que he probado, muy frescos y jugosos.",
                            Score = 5,
                            UserId = user1.Id,
                            ProductId = arandanoProduct.Id,
                            Product = arandanoProduct,
                            User = user1
                        };

                        var review2 = new Review
                        {
                            Text = "Buen sabor, pero algunos estaban un poco blandos.",
                            Score = 3,
                            UserId = user2.Id,
                            ProductId = arandanoProduct.Id,
                            Product = arandanoProduct,
                            User = user2
                        };

                        // Añadir reseñas al contexto de la base de datos
                        dbContext.Reviews.Add(review1);
                        dbContext.Reviews.Add(review2);

                        // Añadir reseñas a la colección de Reviews del producto de arándano
                        arandanoProduct.Reviews.Add(review1);
                        arandanoProduct.Reviews.Add(review2);

                        // Añadir reseñas a la colección de Reviews de cada usuario
                        user1.Reviews.Add(review1);
                        user2.Reviews.Add(review2);

                        dbContext.SaveChanges();

                    }








                    // Guardar cambios en la base de datos
                    dbContext.SaveChanges();

                }
            }


            app.Run();
        }
    }
}
