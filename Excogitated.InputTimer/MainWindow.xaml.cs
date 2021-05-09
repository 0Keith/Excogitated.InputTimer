using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Gma.System.MouseKeyHook;

namespace Excogitated.InputTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IKeyboardMouseEvents _events;
        private readonly Stopwatch _watch;
        private readonly Thread _thread;
        private ScrollViewer _view;

        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            _watch = Stopwatch.StartNew();
            _events = Hook.GlobalEvents();
            _events.KeyDown += _events_KeyDown;
            _events.KeyUp += _events_KeyUp;
            _events.MouseDown += _events_MouseDown;
            _events.MouseUp += _events_MouseUp;
            _thread = new Thread(TimerThread);
            _thread.Start();
        }


        private void TimerThread()
        {
            using (_events)
                try
                {
                    while (!Dispatcher.HasShutdownStarted && !Dispatcher.HasShutdownFinished)
                    {
                        Dispatcher.Invoke(() => _timer.Content = $"{_watch.ElapsedMilliseconds}ms");
                        Thread.Sleep(5);
                    }
                }
                catch (Exception) { }
        }

        private void UpdateEventsAndRestartTimer(string eventDescription)
        {
            if (!Dispatcher.HasShutdownStarted && !Dispatcher.HasShutdownFinished)
                Dispatcher.Invoke(() =>
                {
                    var elapsed = _watch.ElapsedMilliseconds;
                    _watch.Restart();
                    _title.Content = WindowInfo.GetActiveWindowName();
                    if (_text.Items.Count > 10)
                        _text.Items.Clear();
                    _text.Items.Add($"{eventDescription} - {elapsed}ms");
                    if (_view is null)
                        _view = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(_text, 0), 0);
                    _view.ScrollToBottom();
                });
        }

        private void _events_MouseUp(object sender, MouseEventArgs e) => UpdateEventsAndRestartTimer($"{e.Button} Up");
        private void _events_MouseDown(object sender, MouseEventArgs e) => UpdateEventsAndRestartTimer($"{e.Button} Down");

        private void _events_KeyUp(object sender, KeyEventArgs e) => UpdateEventsAndRestartTimer($"{e.KeyCode} Up");
        private void _events_KeyDown(object sender, KeyEventArgs e) => UpdateEventsAndRestartTimer($"{e.KeyCode} Down");
    }
}
