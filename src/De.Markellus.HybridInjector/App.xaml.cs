using System;
using System.Windows;
using De.Markellus.HybridInjector.Misc;
using De.Markellus.HybridInjector.Properties;

namespace De.Markellus.HybridInjector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread()]
        static void Main()
        {
            App app = new App();
            app.InitializeComponent();

            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = Application.Current.FindResource(typeof(Window))
            });

            try
            {
                MusicPlayer.RestartMusic(null, null);
                app.Run();
            }
            catch (Exception ex)
            {
#if(DEBUG)
                MessageBox.Show(ex.Message);
#else
                MessageBox.Show("The injector encountered an unexpected problem.", "FATAL ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                Environment.Exit(3217);
#endif
            }

            Settings.Default.Save();
            MusicPlayer.Dispose();
            Environment.Exit(0);
        }
    }
}
