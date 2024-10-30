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
                    Category category = new Category {Name="fruits" };
                    Category category1 = new Category { Name = "vegetables" };
                    Category category2 = new Category { Name = "meat" };
                    Product product = new Product { Name = "Manzana", Description="No lo voy ha hacer serio esto es una prueba y ya esta", Price=2.5, Stock=150, Average=2.5,image="manzana.jpg", CategoryId=1 };
                    Product product1 = new Product { Name = "Apio",Description= "No lo voy ha hacer serio esto es una prueba y ya esta",Price=3,Stock=200,Average=4,image="apio.jpg",CategoryId=2};
                    Product product2 = new Product { Name = "Ternera",Description= "No lo voy ha hacer serio esto es una prueba y ya esta",Price=5.5,Stock=100,Average=3, image="carne.jpg", CategoryId=3 };
                    dbContext.Categories.Add(category);
                    dbContext.Categories.Add(category1);
                    dbContext.Categories.Add(category2);
                    dbContext.Products.Add(product);
                    dbContext.Products.Add(product1);
                    dbContext.Products.Add(product2);
                    dbContext.SaveChanges();
                }
            }


            app.Run();
        }
    }
}
