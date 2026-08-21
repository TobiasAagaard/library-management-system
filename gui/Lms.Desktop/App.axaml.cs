using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Lms.Desktop.ViewModels;
using Lms.Desktop.Views;
using Lms.Infrastructure.Database;
using Lms.Infrastructure.Migrations;

namespace Lms.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var database = DatabaseConnection.FromConfiguration();
            Migrator.MigrateDatabase();
            desktop.Exit += (_, _) => database.Dispose();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(database),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
