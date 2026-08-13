using MGEditor.Views.Pages;
using MGEditor.Views.Windows;

namespace MGEditor.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private INavigationWindow _navigationWindow;
        private MGConfigService _mGConfigService;
        private AppSettingService  _appSettingService;
        private TranslationService _translationService;

        public ApplicationHostService(
            IServiceProvider serviceProvider, 
            MGConfigService mGConfigService,
            AppSettingService appSettingService,
            TranslationService translationService)
        {
            _serviceProvider = serviceProvider;
            _mGConfigService = mGConfigService;
            _appSettingService = appSettingService;
            _translationService = translationService;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _mGConfigService.LoadConfig();
            _appSettingService.LoadSetting();
            _appSettingService.ApplySettings();
            TranslationService.Instance = _translationService;
            _translationService.ResolveAndLoad(_appSettingService.EditorSetting.Personalized?.Language);
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _mGConfigService.SaveConfig();
            _appSettingService.SaveSetting();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                _navigationWindow = (
                    _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                )!;
                _navigationWindow!.ShowWindow();

                _navigationWindow.Navigate(typeof(HomePage));
            }

            await Task.CompletedTask;
        }
    }
}
