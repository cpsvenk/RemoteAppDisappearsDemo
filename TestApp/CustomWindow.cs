using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace TestApp
{
    /// <remarks>https://blogs.msdn.microsoft.com/wpfsdk/2010/08/25/experiments-with-windowchrome/</remarks>
    public class CustomWindow : Window
    {
        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseExecuted));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeExecuted));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeExecuted));
            SourceInitialized += WindowSourceInitialized;
            StateChanged += WindowStateChanged;
        }

        private static readonly DependencyPropertyKey ResizeBorderThicknessPropertyKey = DependencyProperty.RegisterReadOnly("ResizeBorderThickness",
            typeof(Thickness), typeof(CustomWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="ResizeBorderThickness"/>
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty = ResizeBorderThicknessPropertyKey.DependencyProperty;

        /// <summary>
        /// Größe von dem Border
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessPropertyKey, value); }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            UpdateResizeBorder();
        }
        private void WindowSourceInitialized(object sender, EventArgs args)
        {
            //WindowSizing.WindowInitialized(this);
            UpdateResizeBorder();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateResizeBorder();
        }

        private void OnMaximizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                SystemCommands.MaximizeWindow(this);
            else if (WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(this);
        }

        private void OnMinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void UpdateCaptionHeight()
        {
            WindowChrome chrome = WindowChrome.GetWindowChrome(this);
            Thickness thickness = SystemParameters.WindowResizeBorderThickness;
            if (chrome != null) chrome.CaptionHeight = 40 - thickness.Top;
        }

        private void UpdateResizeBorder()
        {
            var thickness = SystemParameters.WindowResizeBorderThickness;
            if (WindowState == WindowState.Maximized)
            {
                thickness = new Thickness(thickness.Left,
                        thickness.Top, thickness.Right,
                        thickness.Bottom);
            }

            WindowChrome chrome = WindowChrome.GetWindowChrome(this);
            if (chrome != null)
            {
                chrome.ResizeBorderThickness = thickness;
            }

            ResizeBorderThickness = new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom);
            UpdateCaptionHeight();
        }
    }
}
