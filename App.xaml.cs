using FichaIBBV.Services;
using FichaIBBV.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FichaIbbv
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>()
               .AddScoped<IMembrosService, MembrosService>()
               .AddScoped<IEnderecosService, EnderecosService>()
               .AddScoped<INaoMembroService, NaoMembroService>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
