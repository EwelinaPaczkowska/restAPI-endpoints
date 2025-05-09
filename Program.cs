using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using webAPI.Middlewares;
using webAPI.Repositories;
using webAPI.Models;
using webAPI.Services;
using webAPI.Repositories;
using webAPI.Services;
using webAPI.Repositories;
using webAPI.Services;

namespace webAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<ITripRepository, TripRepository>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "webAPI",
                Description = "aplikacja do podrozy",
                Contact = new OpenApiContact
                {
                    Name="Api Support",
                    Email="webapii@gmail.com",
                    Url = new Uri("https://github.com/apiSupport")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });
        
        var app = builder.Build();

        app.UseGlobalExceptionHandling();
        
        app.UseSwagger();
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "webTripApi");
            
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelExpandDepth(0);
            c.DisplayRequestDuration();
            c.EnableFilter();
            
        });
        

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
