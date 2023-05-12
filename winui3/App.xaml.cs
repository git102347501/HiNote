using HiNote.Activation;
using HiNote.Common;
using HiNote.Contracts.Services;
using HiNote.Core.Contracts.Services;
using HiNote.Core.Services;
using HiNote.Helpers;
using HiNote.Models;
using HiNote.Notifications;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Services;
using HiNote.Services;
using HiNote.ViewModels;
using HiNote.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System.Reflection;
using Windows.Globalization;
using Windows.Storage;

namespace HiNote;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

#if !UNIVERSAL
    private static Window startupWindow;
#endif
    // Get the initial window created for this app
    // On UWP, this is simply Window.Current
    // On Desktop, multiple Windows may be created, and the StartupWindow may have already
    // been closed.
    public static Window StartupWindow
    {
        get
        {
#if UNIVERSAL
                return Window.Current;
#else
            return startupWindow;
#endif
        }
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
    private string strCurrentLanguage = "";
    public App()
    {
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        var currentLanguage = localSettings.Values["CurrentLanguage"];
        if (currentLanguage != null)
        {
            if (currentLanguage.ToString() == "auto")
            {
                ApplicationLanguages.PrimaryLanguageOverride = LanguageHelper.GetCurLanguage();
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = currentLanguage.ToString();
            }
        }
        else
        {
            ApplicationLanguages.PrimaryLanguageOverride = strCurrentLanguage = LanguageHelper.GetCurLanguage();
        }
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<IWebViewService, WebViewService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<IOpenAIService, OpenAIService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IExchangeCodeService, ExchangeCodeService>();
            services.AddSingleton<ICurrencyService, CurrencyService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<WebViewViewModel>();
            services.AddTransient<WebViewPage>();
            services.AddSingleton<ListDetailsViewModel>();
            services.AddSingleton<ListDetailsPage>();
            services.AddScoped<ShellPage>();
            services.AddSingleton<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddSingleton<ListDetailsDetailModel>();
            services.AddSingleton<UserInfoViewModel>();
            services.AddSingleton<SelectTreeViewModel>();
            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        //App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);

#if UNIVERSAL
        WindowHelper.TrackWindow(Window.Current);
#else
        startupWindow = WindowHelper.CreateWindow();
#endif
    }

    public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
    {
        if (!typeof(TEnum).GetTypeInfo().IsEnum)
        {
            throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
        }
        return (TEnum)Enum.Parse(typeof(TEnum), text);
    }

}
