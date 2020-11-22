using FFmpeg.NET;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AudioConverter.MVVM
{
    internal class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Paths to input files
        /// </summary>
        private IEnumerable<string> _filePathInputFiles;

        /// <summary>
        /// Names of files
        /// </summary>
        private string _fileNames;

        /// <summary>
        /// File path for output file
        /// </summary>
        private string _filePathforOutputFile;

        /// <summary>
        /// Allowing the user to select one or more files.
        /// </summary>
        private CommonOpenFileDialog _commonOpenFileDialog;

        /// <summary>
        /// Audio formats for converting audio files
        /// </summary>
        public IEnumerable<string> AudioFormats { get; set; }

        /// <summary>
        /// Selected format value from combobox
        /// </summary>
        public string SelectedFormatValue { get; set; }

        /// <summary>
        /// Command for openning dialog for choosing input file(s)
        /// </summary>
        public ICommand OpenDialogForInputFileCommand { get; set; }

        /// <summary>
        /// Command for openning dialog for choosing output directory
        /// </summary>
        public ICommand OpenDialogForOutputDirectory { get; set; }

        /// <summary>
        /// Command for running conversion
        /// </summary>
        public ICommand RunConversionCommand { get; set; }

        /// <summary>
        /// Creates instance of <see cref="MainWindowViewModel"/>
        /// </summary>
        public MainWindowViewModel()
        {
            _commonOpenFileDialog = new CommonOpenFileDialog();
            _commonOpenFileDialog.IsFolderPicker = true;

            OpenDialogForInputFileCommand = new RelayCommand(OpenDialogForInputFile);
            OpenDialogForOutputDirectory = new RelayCommand(OpenDialogForOutputFile);

            RunConversionCommand = new AsyncCommand(
                async () =>
                {
                    await RunConversionAsync();
                });

            AudioFormats = new List<string>()
            {
                "mp3",
                "flac",
                "wav",
                "aac",
                "ac3"
            };

            SelectedFormatValue = AudioFormats.ElementAt(0);
        }

        /// <summary>
        /// Names of files
        /// </summary>
        public string FileNames
        {
            get => _fileNames;
            set
            {
                _fileNames = value;
                OnPropertyChanged(nameof(FileNames));
            }
        }

        /// <summary>
        /// Paths to input files
        /// </summary>
        public IEnumerable<string> FilePathInputFiles
        {
            get => _filePathInputFiles;
            set
            {
                _filePathInputFiles = value;
                OnPropertyChanged(nameof(FilePathInputFiles));
            }
        }

        /// <summary>
        /// File path for output file
        /// </summary>
        public string FilePathForOutputFile
        {
            get => _filePathforOutputFile;
            set
            {
                _filePathforOutputFile = value;
                OnPropertyChanged(nameof(FilePathForOutputFile));
            }
        }

        /// <summary>
        /// Opens dialog for choosing input file(s)
        /// </summary>
        private void OpenDialogForInputFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathInputFiles = openFileDialog.FileNames;
            }

            for (int i = 0; i < FilePathInputFiles.Count(); i++)
            {
                FileNames += Path.GetFileName(FilePathInputFiles.ElementAt(i)) + " | ";
            }
        }

        /// <summary>
        /// Opens dialog for choosing output directory
        /// </summary>
        private void OpenDialogForOutputFile()
        {
            var result = _commonOpenFileDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                FilePathForOutputFile = _commonOpenFileDialog.FileName;
            }
        }

        /// <summary>
        /// Runs conversion between files
        /// </summary>
        private async Task RunConversionAsync()
        {
            var inputMediaFiles = new List<MediaFile>();
            var outputMediaFiles = new List<MediaFile>();

            foreach (var item in FilePathInputFiles)
            {
                inputMediaFiles.Add(new MediaFile($@"{item}"));
                outputMediaFiles.Add(new MediaFile($@"{FilePathForOutputFile}\{Path.GetFileNameWithoutExtension(item)}.{SelectedFormatValue}"));
            }

            var ffmpeg = new Engine("ffmpeg.exe");

            for (int i = 0; i < inputMediaFiles.Count; i++)
            {
                await ffmpeg.ConvertAsync(inputMediaFiles[i], outputMediaFiles[i]);
            }

            MessageBox.Show("Done");

        }
    }
}
