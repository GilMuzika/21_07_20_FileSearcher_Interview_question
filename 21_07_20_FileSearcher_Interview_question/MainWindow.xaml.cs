using _21_07_20_FileSearcher_Interview_question.VMandLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _21_07_20_FileSearcher_Interview_question
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string OneItemDependency
        {
            get
            {
                return SafeInvoke(() => 
                {
                    return (string)this.GetValue(OneItemDependencyProperty);
                });
                
            }
            set
            {
                SafeInvoke(() => 
                {
                    this.SetValue(OneItemDependencyProperty, value);
                });
                
            }
        }
        public static readonly DependencyProperty OneItemDependencyProperty =
        DependencyProperty.Register("OneItemDependency", typeof(string), typeof(MainWindow), new PropertyMetadata(String.Empty));




        public bool ExtensionInFileNameDependency
        {
            get => (bool)this.GetValue(ExtensionInFileNameDependencyProperty);
            set => this.SetValue(ExtensionInFileNameDependencyProperty, value);
        }
        public static readonly DependencyProperty ExtensionInFileNameDependencyProperty =
        DependencyProperty.Register("ExtensionInFileNameDependency", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));



        public MainWindow()
        {
            InitializeComponent();
            Initialize();                       
        }
        private void Initialize()
        {
            this.DataContext = new ViewModel();

            Binding listViewItemBinding = new Binding("LstViewItem");
            listViewItemBinding.Source = this.DataContext;
            listViewItemBinding.Mode = BindingMode.OneWay;
            SetBinding(OneItemDependencyProperty, listViewItemBinding);

            Binding checkBoxBinding = new Binding("ExtensionInFileName");
            checkBoxBinding.Source = this.DataContext;
            checkBoxBinding.Mode = BindingMode.TwoWay;
            SetBinding(ExtensionInFileNameDependencyProperty, checkBoxBinding);


            INotifyPropertyChanged viewModel = (INotifyPropertyChanged)this.DataContext;
            viewModel.PropertyChanged += async (object sender, PropertyChangedEventArgs args) => 
            {
                await Task.Run(() => 
                {
                    if (args.PropertyName.Equals("LstViewItem"))
                    {
                        SafeInvoke(() =>
                        {
                            resultsListView.Items.Add(OneItemDependency);
                        });
                    }
                    /*if (args.PropertyName.Equals("ClearListView"))
                    {
                        SafeInvoke(() =>
                        {
                            resultsListView.ItemsSource = null;
                            resultsListView.Items.Clear();                            
                        });
                    }*/
                });
            };

        }

        private void SafeInvoke(Action work)
        {
            if (Dispatcher.CheckAccess())
            {
                work.Invoke();
                return;
            }
            this.Dispatcher.BeginInvoke(work);
        }
        private T SafeInvoke<T>(Func<T> work) where T: class
        {
            if (Dispatcher.CheckAccess())
            {
                return work.Invoke();
                //return;
            }
            return this.Dispatcher.BeginInvoke(work).Result as T;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            resultsListView.ItemsSource = null;
            resultsListView.Items.Clear();
        }


    }



}
