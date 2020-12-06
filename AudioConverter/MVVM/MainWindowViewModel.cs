using FFmpeg.NET;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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
        private const string EnginePath = "ffmpeg.exe";

        private IEnumerable<string> _filePathInputFiles;

        private string _filePathforOutputFile;

        private string _fileNames;

        private bool _isChangeBitRate;

        private bool _isCutAudio;

        private int? cutTo;

        private int? cutFrom;

        private int? _audioBitRate;

        private readonly Engine _engine;

        private ConversionOptions _conversionOptions;

        private CommonOpenFileDialog _commonOpenFileDialog;

        /// <summary>
        /// Creates instance of <see cref="MainWindowViewModel"/>
        /// </summary>
        public MainWindowViewModel()
        {
            _conversionOptions = new ConversionOptions();
            _engine = new Engine(EnginePath);

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
            AudioBitRate = null;
        }

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
        /// How long to cut the video
        /// </summary>
        public int? CutTo
        {
            get => cutTo;
            set
            {
                cutTo = value;
                OnPropertyChanged(nameof(CutTo));
            }
        }

        /// <summary>
        /// Start cut from value
        /// </summary>
        public int? CutFrom
        {
            get => cutFrom;
            set
            {
                cutFrom = value;
                OnPropertyChanged(nameof(CutFrom));
            }
        }

        /// <summary>
        /// Flag which means will be audio cut or not
        /// </summary>
        public bool IsCutAudio
        {
            get => _isCutAudio;
            set
            {
                _isCutAudio = value;

                if (!IsCutAudio)
                {
                    CutFrom = null;
                    CutTo = null;
                }

                OnPropertyChanged(nameof(CutTo));
                OnPropertyChanged(nameof(CutFrom));
                OnPropertyChanged(nameof(IsCutAudio));
            }
        }

        /// <summary>
        /// Bit rate of audio that will be set on files
        /// </summary>
        public int? AudioBitRate
        {
            get => _audioBitRate;
            set
            {
                _audioBitRate = value;
                OnPropertyChanged(nameof(AudioBitRate));
            }
        }

        /// <summary>
        /// Flag which means will be bit rate changed or not
        /// </summary>
        public bool IsChangeBitRate
        {
            get => _isChangeBitRate;
            set
            {
                _isChangeBitRate = value;

                if (!_isChangeBitRate)
                {
                    AudioBitRate = null;
                }

                OnPropertyChanged(nameof(AudioBitRate));
                OnPropertyChanged(nameof(IsChangeBitRate));
            }
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

            for (int i = 0; i < FilePathInputFiles?.Count(); i++)
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

            if (IsChangeBitRate && AudioBitRate != null)
            {
                _conversionOptions.AudioBitRate = AudioBitRate;
            }

            if (IsCutAudio && CutTo != null && CutFrom != null)
            {
                _conversionOptions.CutMedia(TimeSpan.FromSeconds(double.Parse(CutFrom.ToString())),
                    TimeSpan.FromSeconds(double.Parse(CutTo.ToString())));
            }

            foreach (var item in FilePathInputFiles)
            {
                inputMediaFiles.Add(new MediaFile($@"{item}"));
                outputMediaFiles.Add(new MediaFile($@"{FilePathForOutputFile}\{Path.GetFileNameWithoutExtension(item)}_{AudioBitRate}.{SelectedFormatValue}"));
            }

            for (int i = 0; i < inputMediaFiles.Count; i++)
            {
                await _engine.ConvertAsync(inputMediaFiles[i], outputMediaFiles[i], _conversionOptions);
            }

            MessageBox.Show("Done");
            ResetForm();
        }

        /// <summary>
        /// Resets form after running conversion
        /// </summary>
        private void ResetForm()
        {
            CutTo = null;
            CutFrom = null;
            IsCutAudio = false;
            AudioBitRate = null;
            IsChangeBitRate = false;
            FileNames = null;
            FilePathInputFiles = null;
            FilePathForOutputFile = null;
        }
    }
}
