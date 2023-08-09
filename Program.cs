using Microsoft.EntityFrameworkCore;
using SolidBrokerTest.Repository.DB;


namespace Test2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           

            Repository.DB.SQLHandlers.SetConnectionString(builder.Configuration.GetConnectionString("MSSQL") ?? string.Empty);


            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}