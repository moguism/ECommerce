using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Server.Services.Blockchain;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configuramos cultura invariante para que al pasar los decimales a texto no tengan comas
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var builder = WebApplication.CreateBuilder(args);

            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<Settings>>().Value);

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);

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

            /*builder.Services.AddScoped<FarminhouseContext>();
            builder.Services.AddScoped<UnitOfWork>();*/
            builder.Services.AddScoped<UserMapper>();

            builder.Services.AddScoped<ProductMapper>();
            builder.Services.AddScoped<SmartSearchService>();
            builder.Services.AddScoped<ShoppingCartMapper>();
            builder.Services.AddScoped<ShoppingCartService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<ReviewService>();
            builder.Services.AddScoped<TemporalOrderMapper>();
            builder.Services.AddScoped<TemporalOrderService>();
            builder.Services.AddScoped<CartContentMapper>();
            builder.Services.AddScoped<BlockchainService>();
            builder.Services.AddScoped<EmailService>();

            builder.Services.AddScoped<ProductsToBuyMapper>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<OrderMapper>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<ImageService>();
            builder.Services.AddScoped<CategoryService>();


            //builder.Services.AddHostedService<CleanTemporalOrdersService>();
            // Aqui esta la clave privada
            Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:Key"];

            builder.Services.AddHostedService<CleanTemporalOrdersService>();


            // Permite CORS
            builder.Services.AddCors(
                options =>
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        ;
                    })
                );


            var app = builder.Build();

            //PA QUE FUNCIONE EL WWWROOT NO LO TOQUEIS HIJOS DE PUTA
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            });

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            // Permite CORS
            app.UseCors();

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
                        new Product { Name = "Manzana", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 250, Stock = 150, Average = 0, Image = "/images/manzana.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Plátano", Description = "Una fuente rápida de energía, perfecta para llevar.", Price = 150, Stock = 200, Average = 0, Image = "/images/platano.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Naranja", Description = "Cítrico jugoso y refrescante, rico en vitamina C.", Price = 300, Stock = 0, Average = 0, Image = "/images/naranja.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Fresa", Description = "Fruta dulce y roja, excelente en postres y batidos.", Price = 400, Stock = 120, Average = 0, Image = "/images/fresa.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Kiwi", Description = "Fruta exótica con un sabor único y refrescante.", Price = 350, Stock = 90, Average = 0, Image = "/images/kiwi.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Pera", Description = "Fruta suave y jugosa, ideal para ensaladas.", Price = 280, Stock = 160, Average = 0, Image = "/images/pera.jpg", CategoryId = 1, Category = fruitsCategory},
                        new Product { Name = "Arandano", Description = "Una fruta crujiente y dulce, ideal para snacks.", Price = 250, Stock = 150, Average = 0, Image = "/images/arandano.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Uva", Description = "Pequeñas frutas dulces, perfectas para picar o hacer vino.", Price = 500, Stock = 130, Average = 0, Image = "/images/uva.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Sandía", Description = "Fruta refrescante y jugosa, perfecta para el verano.", Price = 600, Stock = 75, Average = 0, Image = "/images/sandia.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Melón", Description = "Fruta dulce y jugosa, ideal para ensaladas de frutas.", Price = 450, Stock = 80, Average = 0, Image = "/images/melon.jpg", CategoryId = 1, Category = fruitsCategory },
                        new Product { Name = "Mango", Description = "Fruta tropical dulce, perfecta para smoothies y postres.", Price = 320, Stock = 110, Average = 0, Image = "/images/mango.jpg", CategoryId = 1, Category = fruitsCategory }
                    };

                    // Verduras
                    var vegetables = new List<Product>
                    {
                        new Product { Name = "Apio", Description = "Vegetal crujiente, ideal para dips y ensaladas.", Price = 300, Stock = 200, Average = 0, Image = "/images/apio.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Zanahoria", Description = "Dulce y crujiente, rica en vitamina A, perfecta para snacks.", Price = 150, Stock = 180, Average = 0, Image = "/images/zanahoria.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Tomate", Description = "Versátil y jugoso, excelente para ensaladas y salsas.", Price = 200, Stock = 150, Average = 0, Image = "/images/tomate.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Lechuga", Description = "Base fresca para ensaladas, ligera y nutritiva.", Price = 120, Stock = 220, Average = 0, Image = "/images/lechuga.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Cebolla", Description = "Sabor fuerte y característico, ideal para sazonar.", Price = 180, Stock = 170, Average = 0, Image = "/images/cebolla.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Pimiento", Description = "Dulce y crujiente, perfecto para saltear o asar.", Price = 250, Stock = 140, Average = 0, Image = "/images/pimiento.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Brócoli", Description = "Vegetal verde lleno de nutrientes, ideal al vapor.", Price = 280, Stock = 0, Average = 0, Image = "/images/brocoli.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Espinaca", Description = "Hoja verde rica en hierro, excelente para ensaladas y guisos.", Price = 320, Stock = 0, Average = 0, Image = "/images/espinaca.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Coliflor", Description = "Vegetal versátil, ideal para purés y como sustituto del arroz.", Price = 270, Stock = 0, Average = 0, Image = "/images/coliflor.jpg", CategoryId = 2, Category = vegetablesCategory },
                        new Product { Name = "Berenjena", Description = "Sabor único, excelente para asar y guisar.", Price = 350, Stock = 70, Average = 0, Image = "/images/berenjena.jpg", CategoryId = 2, Category = vegetablesCategory }
                    };

                    // Carnes
                    var meats = new List<Product>
                    {
                        new Product { Name = "Ternera", Description = "Carne tierna y jugosa, ideal para guisos y asados.", Price = 550, Stock = 100, Average = 0, Image = "/images/ternera.jpg", CategoryId = 3, Category = meatCategory},
                        new Product { Name = "Pollo", Description = "Carne magra, versátil y rica en proteínas.", Price = 400, Stock = 150, Average = 0, Image = "/images/pollo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Cerdo", Description = "Carne sabrosa, ideal para parrillas y guisos.", Price = 600, Stock = 80, Average = 0, Image = "/images/cerdo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Cordero", Description = "Carne rica y suculenta, perfecta para asados.", Price = 700, Stock = 60, Average = 0, Image = "/images/cordero.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Pavo", Description = "Alternativa magra al pollo, ideal para celebraciones.", Price = 550, Stock = 90, Average = 0, Image = "/images/pavo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Conejo", Description = "Carne suave y saludable, ideal para guisos.", Price = 800, Stock = 50, Average = 0, Image = "/images/conejo.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Salchicha", Description = "Deliciosa y jugosa, perfecta para barbacoas.", Price = 250, Stock = 200, Average = 0, Image = "/images/salchicha.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Bacon", Description = "Crujiente y sabroso, ideal para desayunos y burgers.", Price = 450, Stock = 0, Average = 0, Image = "/images/bacon.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Panceta", Description = "Sabrosa y jugosa, perfecta para añadir sabor a tus platos.", Price = 500, Stock = 70, Average = 0, Image = "/images/panceta.jpg", CategoryId = 3, Category = meatCategory },
                        new Product { Name = "Atún", Description = "Pescado rico en omega-3, ideal para ensaladas y sushi.", Price = 650, Stock = 40, Average = 0, Image = "/images/atun.jpg", CategoryId = 3, Category = meatCategory }
                    };


                    // Añadir categorías y productos al contexto de la base de datos
                    dbContext.Categories.Add(fruitsCategory);
                    dbContext.Categories.Add(vegetablesCategory);
                    dbContext.Categories.Add(meatCategory);
                    dbContext.Products.AddRange(fruits);
                    dbContext.Products.AddRange(vegetables);
                    dbContext.Products.AddRange(meats);

                    PasswordService passwordService = new PasswordService();
                    // Crear usuarios de ejemplo
                    var user = new User { Name = builder.Configuration["AdminUser:Name"], Email = builder.Configuration["AdminUser:Email"], Password = passwordService.Hash(builder.Configuration["AdminUser:Password"]), Role = builder.Configuration["AdminUser:Role"], Address = builder.Configuration["AdminUser:Address"] };

                    // Asegurarse de que los usuarios están añadidos al contexto
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    // Guardar cambios en la base de datos
                    dbContext.SaveChanges();

                }
            }


            app.Run();
        }
    }
}
