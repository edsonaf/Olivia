using System;
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
      IncludeSubFolders = Settings.Default.IncludeSubfolders;
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
        if (_includeSubFolders == value) return;

        _includeSubFolders = value;

        Settings.Default.IncludeSubfolders = value;
        Settings.Default.Save();

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
    public ICommand ProcessCommand => _processCommand ?? (_processCommand = new RelayCommand(param =>
    {
      // Save last used folder paths
      Settings.Default.LastUsedInputFolderPath = InputFolderPath;
      Settings.Default.LastUsedOutputFolderPath = OutputFolderPath;

      CopyToOutputFolder();
          
      Settings.Default.Save();
    }));

    private RelayCommand _stopCommand;
    public ICommand StopCommand => _stopCommand ?? (_stopCommand = new RelayCommand(param =>
    {

    }));

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
      if (!Directory.Exists(OutputFolderPath))
      {
        Directory.CreateDirectory(OutputFolderPath);
      }

      var files = IncludeSubFolders ? Directory.GetFiles(InputFolderPath, "*.txt", System.IO.SearchOption.AllDirectories) : Directory.GetFiles(InputFolderPath, "*.txt");
      foreach (var file in files)
      {
        string onlyFileName;
        var startOfFileName = file.LastIndexOf("\\", StringComparison.Ordinal);
        if (startOfFileName > 0) onlyFileName = file.Substring(startOfFileName);
        else
        {
          startOfFileName = file.LastIndexOf("/", StringComparison.Ordinal);
          onlyFileName = file.Substring(startOfFileName > 0 ? startOfFileName : InputFolderPath.Length);
        }

        File.Copy(file, OutputFolderPath + onlyFileName, true);
      }

      var dialogResult = MessageBox.Show(@"Finished. Open folder?", @"Finished copying", MessageBoxButtons.YesNo);

      if (dialogResult != DialogResult.Yes) return;
      
      // Open folder
      try
      {
        System.Diagnostics.Process.Start(OutputFolderPath);
      }
      catch (Exception e)
      {
        Console.WriteLine($@"Failed to open directory. Exception {e}");
      }
    }

    #endregion Private Functions


    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public enum FileType
  {
    Text,
    Image,
    Document,
    Music
  }
}