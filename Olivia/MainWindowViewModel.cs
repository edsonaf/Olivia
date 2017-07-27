using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using Olivia.Properties;

namespace Olivia
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public MainWindowViewModel()
    {
      InputFolderPath = Settings.Default.LastUsedInputFolderPath;
      OutputFolderPath = Settings.Default.LastUsedOutputFolderPath;
      ShowMessage = Settings.Default.ShowMessage;
    }

    
    #region Properties

    private string _inputFolderPath;
    public string InputFolderPath
    {
      get => _inputFolderPath;
      set
      {
        _inputFolderPath = value;
        RaisePropertyChanged(nameof(InputFolderPath));
      }
    }

    private string _outputFolderPath;
    public string OutputFolderPath
    {
      get => _outputFolderPath;
      set
      {
        _outputFolderPath = value;
        RaisePropertyChanged(nameof(OutputFolderPath));
      }
    }

    private bool _includeSubFolders;
    public bool IncludeSubFolders
    {
      get => _includeSubFolders;
      set
      {
        _includeSubFolders = value;
        RaisePropertyChanged(nameof(IncludeSubFolders));
      }
    }

    public bool ShowMessage { get; set; }

    #endregion Properties


    #region Commands

    private RelayCommand _browseFolderCommand;

    public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new RelayCommand(param =>
    {
      var isInput = false;

      if (param != null)
      {
        isInput = (string)param == "true";
      }

      OpenFolder(isInput);
    }));


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

          CopyToOutputFolder();


          Settings.Default.Save();


        }));
      }
    }

    #endregion Commands


    #region Private Functions

    private void OpenFolder(bool isInput)
    {
      using (var dialog = new FolderBrowserDialog())
      {
        var result = dialog.ShowDialog();

        if (result == DialogResult.OK)
        {
          if (isInput) InputFolderPath = dialog.SelectedPath;
          else OutputFolderPath = dialog.SelectedPath;
        }
      }
    }

    private void CopyToOutputFolder()
    {
      // TODO: If output folder does not exist. create it.
      // TODO: search for files in subfolders

      if (IncludeSubFolders)
      {
        
      }

      var files = Directory.GetFiles(InputFolderPath, "*.txt");
      foreach (var file in files)
      {
       File.Copy(file, OutputFolderPath + file.Substring(InputFolderPath.Length), true); 
      }

      var dialogResult = MessageBox.Show(@"Finished. Open folder?", @"Finished copying", MessageBoxButtons.YesNo);
    }

#endregion Private Functions


    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}