using MGEditor.DependencyModel;
using MGEditor.Services;
using MGEditor.Services.Contracts;
using MGEditor.ViewModels.Pages;
using MGEditor.ViewModels.Windows;
using MGEditor.Views.Pages;
using MGEditor.Views.Windows;

using Wpf.Ui.DependencyInjection;

namespace MGEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => 
            { 
                _ = c.SetBasePath(AppContext.BaseDirectory); 
            })
            .ConfigureServices(
            (_1, services) =>
            {
                _ = services.AddNavigationViewPageProvider();

                _ = services.AddHostedService<ApplicationHostService>();

                // Theme manipulation
                _ = services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                _ = services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                _ = services.AddSingleton<INavigationService, NavigationService>();

                _ = services.AddSingleton<MGConfigService>();
				_ = services.AddSingleton<AppSettingService>();
				_ = services.AddSingleton<TranslationService>();

				// Main window with navigation
				_ = services.AddSingleton<INavigationWindow, MainWindow>();
                _ = services.AddSingleton<MainWindowViewModel>();
                _ = services.AddSingleton<INavigationService, NavigationService>();
                _ = services.AddSingleton<ISnackbarService, SnackbarService>();
                _ = services.AddSingleton<IContentDialogService, ContentDialogService>();
                _ = services.AddSingleton<WindowsProviderService>();

                _ = services.AddSingleton<HomePage>();
                _ = services.AddSingleton<HomeViewModel>();
                _ = services.AddSingleton<SettingsPage>();
                _ = services.AddSingleton<SettingsViewModel>();

                // All other pages and view models
                _ = services.AddTransientFromNamespace("MGEditor.Views", GalleryAssembly.Asssembly);
                _ = services.AddTransientFromNamespace(
                    "MGEditor.ViewModels",
                    GalleryAssembly.Asssembly
                );
            }).Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }

        public static T GetRequiredService<T>() where T : class
        {
            return _host.Services.GetRequiredService<T>();
        }
    }
}
