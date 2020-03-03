using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(nameof(Duration), typeof(int), typeof(MainWindow), new PropertyMetadata(6000));
        public static readonly DependencyProperty NewTitleProperty =
            DependencyProperty.Register(nameof(NewTitle), typeof(string), typeof(MainWindow), new PropertyMetadata("New title"));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public int Duration
        {
            get => (int)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public string NewTitle
        {
            get => (string)GetValue(NewTitleProperty);
            set => SetValue(NewTitleProperty, value);
        }

        private void DoWorkOnUIThread()
        {
            Cursor = Cursors.Wait;
            try
            {
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < Duration)
                {
                    ;
                }
            }
            finally
            {
                Cursor = null;
            }
        }

        private void ChangeClicked(object sender, RoutedEventArgs e)
        {
            Title = NewTitle;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Style = (Style)Resources["GadgetStyle"];
        }

        private void DoWorkClicked(object sender, RoutedEventArgs e)
        {
            DoWorkOnUIThread();
        }
    }
}
