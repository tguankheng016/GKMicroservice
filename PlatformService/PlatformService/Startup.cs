using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        if (_env.IsProduction())
        {
            Console.WriteLine($"Using Sql Server");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Default")));
        }
        else
        {
            Console.WriteLine($"Using In Memory Db");
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }
        
        services.AddScoped<IPlatformRepo, PlatformRepo>();
        
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        
        services.AddGrpc();
        
        services.AddControllers();
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseHttpsRedirection();
        
        app.UseStaticFiles();
        
        app.UseRouting();

        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
            endpoints.MapGrpcService<GrpcPlatformService>();

            endpoints.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            });
        });
        
        PrepDb.PrepPopulation(app, _env.IsProduction());
    }
}