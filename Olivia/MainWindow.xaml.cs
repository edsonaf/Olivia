using System.Windows;

namespace Olivia
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    public MainWindowViewModel MainWindowViewModel
    {
      set => DataContext = value;
    }
  }
}
