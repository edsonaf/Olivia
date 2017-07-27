using System.Windows;

namespace Olivia
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e); MainWindow window = new MainWindow();
      
      // Create the ViewModel to which 
      // the main window binds. 
      var viewModel = new MainWindowViewModel();

      // Allow all controls in the window to 
      // bind to the ViewModel by setting the 
      // DataContext, which propagates down 
      // the element tree. 
      window.DataContext = viewModel;
      window.Show();
    }
  }
}
