using System.Windows.Input;

namespace Olivia
{
  public class MainWindowViewModel
  {
    public MainWindowViewModel()
    {
    }

    public string InputFolderPath { get; set; }

    public string OutputFolderPath { get; set; }
    
    private RelayCommand _browseFolderCommand;
    public ICommand BrowseFolderCommand
    {
      get
      {
        return _browseFolderCommand ?? (_browseFolderCommand = new RelayCommand(param =>
        {

        }));
      }
    }
  }
}
