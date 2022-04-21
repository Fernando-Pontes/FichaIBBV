using FichaIBBV.Services;
using FichaIBBV.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;
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



            _ = services.AddSingleton<MainWindow>()
               .AddScoped<IMembrosService, MembrosService>()
               .AddScoped<IEnderecosService, EnderecosService>()
               .AddScoped<INaoMembroService, NaoMembroService>()
               .AddScoped<IMembros_NaoMembrosService, Membros_NaoMembrosService>();

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
               .AddJsonFile("appsettings.json")
               .Build();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
