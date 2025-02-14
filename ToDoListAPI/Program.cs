using ToDoListAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(s =>
    {
        s.AddSingleton<ToDoListService>(); // atau AddScoped jika perlu per instance per request
    })
    .Build();

host.Run();
