using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
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

    private readonly ObservableCollection<CopyFile> _copiedFiles = new ObservableCollection<CopyFile>();
    public ObservableCollection<CopyFile> CopiedFiles => _copiedFiles;

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
    public ICommand ProcessCommand => _processCommand ?? (_processCommand = new RelayCommand(async param =>
    {
      // Save last used folder paths
      Settings.Default.LastUsedInputFolderPath = InputFolderPath;
      Settings.Default.LastUsedOutputFolderPath = OutputFolderPath;

      await CopyToOutputFolder();

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

    private async Task CopyToOutputFolder()
    {
      if (!Directory.Exists(OutputFolderPath))
      {
        Directory.CreateDirectory(OutputFolderPath);
      }

      CopiedFiles.Clear();

      var files = IncludeSubFolders ? Directory.GetFiles(InputFolderPath, "*.txt", SearchOption.AllDirectories) : Directory.GetFiles(InputFolderPath, "*.txt");

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

        CopiedFiles.Add(new CopyFile(onlyFileName, (int)(new FileInfo(file).Length / 1024)));

        await Task.Run(async () =>
        {
          await Task.Run(() => File.Copy(file, OutputFolderPath + onlyFileName, true));
          Thread.Sleep(50);
        });
          

        RaisePropertyChanged(nameof(CopiedFiles));
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
        MessageBox.Show($@"Failed to open directory. Exception {e}", @"Error", MessageBoxButtons.OK);
      }
    }

    #endregion Private Functions


    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class CopyFile
  {
    public CopyFile(string name, int size)
    {
      Name = name;
      Size = size;
    }

    public string Name { get; set; }

    public int Size { get; set; }
  }

  public enum FileType
  {
    Text,
    Image,
    Document,
    Music
  }
}