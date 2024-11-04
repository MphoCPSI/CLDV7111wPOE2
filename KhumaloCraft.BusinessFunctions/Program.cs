using KhumaloCraft.Business.Interfaces;
using KhumaloCraft.Business.Services;
using KhumaloCraft.Data.Data;
using KhumaloCraft.Data.Repositories.Implementations;
using KhumaloCraft.Data.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var dbConnection = context.Configuration.GetConnectionString("DefaultConnection");

        services.AddHttpClient();

        services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(dbConnection));

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register other services
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<CartService>();
    })
    .Build();

host.Run();
