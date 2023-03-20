using IMS.Plugins.EFCore;
using IMS.UseCases;
using IMS.UseCases.Interfaces;
using IMS.UseCases.Inventories;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products;
using IMS.WebApp.Areas.Identity;
using IMS.WebApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using IProduceProductUseCase = IMS.UseCases.Interfaces.IProduceProductUseCase;

namespace IMS.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services
                .AddScoped<AuthenticationStateProvider,
                    RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            //add DataContext
            builder.Services.AddDbContext<IMSContext>(options => { options.UseInMemoryDatabase("IMS"); });

            //DI use case
            builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>();
            builder.Services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
            builder.Services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
            builder.Services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();

            builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
            builder.Services.AddTransient<IAddProductUseCase, AddProductUseCase>();
            builder.Services.AddTransient<IViewProductByIdUseCase, ViewProductByIdUseCase>();
            builder.Services.AddTransient<IEditProductUseCase, EditProductUseCase>();
            builder.Services.AddTransient<IDeleteProductUseCase, DeleteProductUseCase>();
            builder.Services.AddTransient<IPurchaseInventoryUseCase, PurchaseInventoryUseCase>();
            builder.Services
                .AddTransient<IValidateEnoughInventoriesForProducingUseCase,
                    ValidateEnoughInventoriesForProducingUseCase>();
            builder.Services.AddTransient<IProduceProductUseCase, ProduceProductUseCase>();

            //DI repositories
            builder.Services.AddTransient<IInventoryRepository, InventoryRepository>();
            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<IInventoryTransactionRepository, InventoryTransactionRepository>();
            builder.Services.AddTransient<IProductTransactionRepository, ProductTransactionRepository>();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var imsContext = scope.ServiceProvider.GetRequiredService<IMSContext>();

            //EF core in memory test
            imsContext.Database.EnsureDeleted();
            imsContext.Database.EnsureCreated();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}