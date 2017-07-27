using System.Windows.Input;
using Olivia.Properties;

namespace Olivia
{
  public class MainWindowViewModel
  {
    public MainWindowViewModel()
    {
      InputFolderPath = Settings.Default.LastUsedInputFolderPath;
      OutputFolderPath = Settings.Default.LastUsedOutputFolderPath;
      ShowMessage = Settings.Default.ShowMessage;
    }

    public string InputFolderPath { get; set; }

    public string OutputFolderPath { get; set; }

    public bool ShowMessage { get; set; }

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

    private RelayCommand _processCommand;
    public ICommand ProcessCommand
    {
      get
      {
        return _processCommand ?? (_processCommand = new RelayCommand(param =>
        {
          // Save last used folder paths
          Settings.Default.LastUsedInputFolderPath = InputFolderPath;
          Settings.Default.LastUsedOutputFolderPath = OutputFolderPath;



          Settings.Default.Save();


        }));
      }
    }
  }
}