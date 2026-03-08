using Microsoft.EntityFrameworkCore;

using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.Data.Seed;
using TradingApp.Services.Core;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            //adding custom services to the container
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductOperationsService, ProductOperationsService>();
            builder.Services.AddScoped<IProductBoolsService, ProductBoolsService>();
            builder.Services.AddScoped<IProductFileService, ProductFileService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IBalanceService, BalanceService>();
            


            
            //add the seeder classes to the services
            builder.Services.AddTransient<UserSeeder>();
            builder.Services.AddTransient<BalanceSeeder>();
            builder.Services.AddTransient<ProductSeeder>();
            builder.Services.AddTransient<OrderRequestSeeder>();
            builder.Services.AddTransient<SellOrderSeeder>();
            builder.Services.AddTransient<SellOrderSuggestionSeeder>();
            




            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();           

            //seed some data to the database
            await new Program().SeedDataAsync(app);

            await app.RunAsync();            
        }


        //<in testing state
        //seeding some data to the database
        public async Task SeedDataAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                UserSeeder userSeeder = services.GetRequiredService<UserSeeder>();
                await userSeeder.SeedAsync();

                BalanceSeeder balanceSeeder = services.GetRequiredService<BalanceSeeder>();
                await balanceSeeder.SeedAsync();

                ProductSeeder productSeeder = services.GetRequiredService<ProductSeeder>();
                await productSeeder.SeedAsync();

                OrderRequestSeeder purchaseOrderSeeder = services.GetRequiredService<OrderRequestSeeder>();
                await purchaseOrderSeeder.SeedAsync();

                SellOrderSeeder sellOrderSeeder = services.GetRequiredService<SellOrderSeeder>();
                await sellOrderSeeder.SeedAsync();

                SellOrderSuggestionSeeder sellOrderSuggestionSeeder = services.GetRequiredService<SellOrderSuggestionSeeder>();
                await sellOrderSuggestionSeeder.SeedAsync();
            }
        }
        //in testing state>
    }
}
