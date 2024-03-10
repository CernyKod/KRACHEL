using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KRACHEL.WPF.Views.ViewsManager;
using Serilog;
using KRACHEL.WPF.Services;

namespace KRACHEL.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        private ILogger<App> _logger;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigureSerilog();
            
            try
            {
                _host = BuildHost();
                await _host.StartAsync();

                _logger = _host.Services.GetService<ILogger<App>>();

                this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;

                _host.Services.GetService<Views.MainWindow>().Show();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            Log.CloseAndFlush();
        }

        private IHost BuildHost()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    configurationBuilder.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Infrastructure.AppSettings.AppSettings>(context.Configuration);
                    services.AddTransient<Core.IVideoBuilder, Infrastructure.FFvideoBuilder.FFVideoBuilder>();
                    services.AddTransient<Core.Service.IVideoService, Core.Service.VideoService>();
                    services.AddSingleton<IViewsManager, ViewsManager>();
                    services.AddSingleton<IDialogService, WindowsDialogService>();
                    services.AddTransient(typeof(Views.MainWindow));
                    services.AddTransient(typeof(Views.AudioExtractorWindow));
                    services.AddTransient(typeof(Views.VideoEditorWindow));
                    services.AddTransient(typeof(Views.VideoPartWindow));
                })
                /*.NET default logging*/
                //.ConfigureLogging((context, logging) =>
                //{
                //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                //    logging.AddConsole();
                //    logging.AddEventLog();
                //})
                /*Serolig logging*/
                .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                )
                .Build();
        }

        private void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(@"startup.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError($"{e.Exception.Message} \n {e.Exception.InnerException} \n {e.Exception.StackTrace}");

            MessageBox.Show($"{e.Exception.Message} Detail chyby naleznete v logu aplikace.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
