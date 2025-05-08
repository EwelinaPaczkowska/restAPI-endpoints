using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
//using webAPI.Repositories;
using webAPI.Services;

namespace webAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        //builder.Services.AddScoped<ITripService, TripService>();
        //builder.Services.AddScoped<ITripRepository, TripRepository>();
        //builder.Services.AddScoped<IClientService, ClientService>();
        //builder.Services.AddScoped<IClientRepository, ClientRepository>();
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "webAPI", Version = "v1" });            
        });

        //builder.Services.AddOpenApi();

        var app = builder.Build();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
