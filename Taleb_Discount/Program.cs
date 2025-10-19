
using System.Text.Json;
using Taleb_Discount.Extentions;

namespace Taleb_Discount
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Services
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            builder.Services.AddPressentionServices();
            builder.Services.AddCoreServices(builder.Configuration);
            builder.Services.AddInfrasturctureServices(builder.Configuration);
            #endregion  
            var app = builder.Build();
            app.UseCustomMiddleWare();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
