
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Repositories;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Marketplace.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/marketplace-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up the application...");

                var builder = WebApplication.CreateBuilder(args);

                // Add user secrets in development
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.AddUserSecrets<Program>();
                }

                // Add services to the container
                builder.Services.AddControllersWithViews();

                // Configure DbContext
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // Register Repositories
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
                builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

                var app = builder.Build();

                // Configure the HTTP request pipeline
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}