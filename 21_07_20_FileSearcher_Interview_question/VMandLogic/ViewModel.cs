using _21_07_20_FileSearcher_Interview_question.VMandLogic.composite;
using Prism.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace _21_07_20_FileSearcher_Interview_question.VMandLogic
{
    class ViewModel : ViewModelBase
    {

        private static string _oneItem;
        private bool _isChildrenCounted = false;
        private double overallChildrenNumber = 0;
        public static double _notifyIfMatchExecutionsNumber = 0;
        public static object Locker { get; private set; } = new object();


        public static List<string> MutualPlace { get; set; } = new List<string>();
        public List<string> ResultsColl { get; set; } = new List<string>();

        public static void AddResult(string oneResultLine)
        {
            lock (Locker)
            {                
                _oneItem = oneResultLine;
                MutualPlace.Add(oneResultLine);
            }
        }

        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        private bool _clearListView = true;
        public bool ClearListView
        {
            get => _clearListView;
            set
            {
                if (_clearListView != value)
                    _clearListView = value;
                OnPropertyChanged();
            }
        }

        public static bool ExtensionInFileNameStatic { get; set; }

        private bool _extensionInFileName = true;
        public bool ExtensionInFileName
        {
            get => _extensionInFileName;
            set
            {
                if (_extensionInFileName != value)
                    _extensionInFileName = value;
                OnPropertyChanged();
            }
        }


        private double _progressBarValue = 0;
        public double ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                if (_progressBarValue != value)
                    _progressBarValue = value;
                OnPropertyChanged();
            }
        }
        private double _progressBarMax = 100;
        public double ProgressBarMax
        {
            get => _progressBarMax;
            set
            {
                if (_progressBarMax != value)
                    _progressBarMax = value;
                OnPropertyChanged();
            }
        }

        private string _listViewItem;
        public string LstViewItem
        {
            get => _listViewItem;
            set
            {
                if (_listViewItem != value)
                    _listViewItem = value;               
                OnPropertyChanged();
            }
        }

        private List<string> _resultCollProp;
        public List<string> ResultCollProp
        {
            get => _resultCollProp;
            set
            {
                if (_resultCollProp != value)
                   _resultCollProp = value;                
                OnPropertyChanged();
            }

            
        }

        private string _startSearchPath;
        public string StartSearchPath
        {
            get => _startSearchPath;
            set
            {                
                if(_startSearchPath != value) 
                    _startSearchPath = value;
                OnPropertyChanged();
            }
        }

        private string _searchPattern;
        public string SearchPattern
        {
            get => _searchPattern;
            set
            {
                if (_searchPattern != value)
                    _searchPattern = value;
                OnPropertyChanged();
            }
        }


        public DelegateCommand TakeStartSearchPathWithDialog_DelegComm { get; set; }
        public ICommand StartSearch_RelayComm { get; set; }
        public ICommand Clear_RelayComm { get; set; }
        public ICommand ExtensionInFilename_RelayComm { get; set; }

        private bool _isSearchCompleted = true;
        public ViewModel()
        {
            _timer.Interval = 1;
            string oldItem = string.Empty;
            Queue<string> queue = new Queue<string>(2);
            _timer.Tick += (object sender, EventArgs e) => 
            {
                ExtensionInFileNameStatic = ExtensionInFileName;
                Thread.Sleep(2);

                if (_isChildrenCounted == true)
                {

                    ProgressBarValue = _notifyIfMatchExecutionsNumber;                    

                    if (ProgressBarValue >= ProgressBarMax)
                        _isChildrenCounted = false;
                }
                
                if (!String.IsNullOrEmpty(_oneItem) && !String.IsNullOrWhiteSpace(_oneItem))
                {
                    queue.Enqueue(_oneItem);
                    if(queue.Count > 1) oldItem = queue.Dequeue(); 
                    if (oldItem != _oneItem)
                    {
                        LstViewItem = _oneItem;
                        ResultsColl.Add(_oneItem);
                    }
                }
            };
            _timer.Start();



            if (File.Exists("searchPath.txt"))            
                StartSearchPath = File.ReadAllText("searchPath.txt");

            CheckStartpath(false);


            Clear_RelayComm = new RelayCommand((object o) => 
            {
                
                
            }, (object o) => { return ClearListView; });



            StartSearch_RelayComm = new RelayCommand(async(object o) => 
            {
                ClearListView = ClearListView ? false : true;

                do
                {
                    if (!CheckStartpath(true))
                    {
                        MessageBox.Show("Please type some search path or take it with a dialog");
                        return;
                    }
                }
                while (!CheckStartpath(true));

                if (String.IsNullOrEmpty(SearchPattern))
                    MessageBox.Show("Please type some search pattern.");
                else if (String.IsNullOrWhiteSpace(SearchPattern))
                    MessageBox.Show("Search pattern can't be white space symbols");
                else
                {
                    ProgressBarValue = 0;
                    _isSearchCompleted = false;
                    lock (Locker)
                    {
                        MutualPlace.Clear();
                        MutualPlace = new List<string>();
                    }

                    Task tsk = Search(SearchPattern);
                    await tsk;
                    if (tsk.IsCompleted)
                    {
                        _isSearchCompleted = true;
                        ClearListView = ClearListView ? false : true;
                    }
                }



            }, 
            (object o) => 
            {
                return _isSearchCompleted;
            });




            TakeStartSearchPathWithDialog_DelegComm = new DelegateCommand(() => 
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                CheckStartpath(true);

                dialog.SelectedPath = StartSearchPath;                
                dialog.ShowDialog();
                StartSearchPath = dialog.SelectedPath;
            },
            () => { return true; });
        }

        private bool CheckStartpath(bool showMessageBox)
        {
            if (!Directory.Exists(StartSearchPath))
            {
                if (!String.IsNullOrEmpty(StartSearchPath) && !String.IsNullOrWhiteSpace(StartSearchPath))
                    if(showMessageBox== true)
                        MessageBox.Show($"\"{StartSearchPath}\" isn't a path!");

                StartSearchPath = string.Empty;
                return false;
            }

            File.WriteAllText("searchPath.txt", StartSearchPath);

            return true;
        }

        private async Task Search(string searchPattern)
        {

            overallChildrenNumber++;
            Branch rootBranch = new Branch(StartSearchPath, searchPattern);
            rootBranch = await GetFilesAndDirectories(rootBranch, searchPattern);

            _isChildrenCounted = true;

            ProgressBarMax = overallChildrenNumber;

            await rootBranch.NotifyIfMatch();

            
            var r2  = MutualPlace;
            //var t = MutualPlace[MutualPlace.Count - 1];
                //MutualPlace = Statics.MutualPlace;
            

            
        }


        private async Task<Branch> GetFilesAndDirectories(Branch rootBranch, string searchPattern)
        {
            return await Task.Run(async () =>
            {
                string[] files = Directory.GetFiles(rootBranch.FileOrDirectory);
                string[] folders = Directory.GetDirectories(rootBranch.FileOrDirectory);                

                foreach (string s in files)
                {
                    rootBranch.AddChild(new Leaf(s, searchPattern));
                    overallChildrenNumber++;
                }
                foreach (string s in folders)
                {
                    Branch branch = new Branch(s, searchPattern);
                    overallChildrenNumber++;
                    try
                    {

                        branch = await GetFilesAndDirectories(branch, searchPattern);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    rootBranch.AddChild(branch);

                }

                return rootBranch;
            });
        }


    }
}
