using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandsService;

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
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMen"));
        services.AddScoped<ICommandRepo, CommandRepo>();
        
        services.AddControllers();
        
        services.AddHostedService<MessageBusSubscriber>();
        
        services.AddSingleton<IEventProcessor, EventProcessor>(); 
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddScoped<IPlatformDataClient, PlatformDataClient>();
        
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
        });
        
        PrepDb.PrepPopulation(app);
    }
}