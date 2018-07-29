using System;
using System.Runtime.InteropServices;
using System.Windows;
using ProgressBar.View;

namespace ProgressBar
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAnimation
    {
        void Begin(Delegates.LongProcess longProcess);
        event RoutedEventHandler Loaded;
        void Initialize(string heading);
        void Show();
        void Increment();
        void Close();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Animation : IAnimation
    {
        private ProgressBarView _progressBarView;

        public event RoutedEventHandler Loaded
        {
            add => _progressBarView.Loaded += value;
            remove => _progressBarView.Loaded -= value;
        }

        [ComVisible(true)]
        public void Initialize(string heading)
        {
            _progressBarView = new ProgressBarView(heading);
        }

        [ComVisible(true)]
        public void Begin(Delegates.LongProcess longProcess)
        {
            longProcess.Invoke(() => { Application.Current.Dispatcher.Invoke(Increment);
            });
        }

        [ComVisible(true)]
        public void Show()
        {
            if (_progressBarView == null)
                throw new NullReferenceException("ProgressBar has not been initialized.");
            _progressBarView.ShowDialog();
        }

        [ComVisible(true)]
        public void Increment()
        {
            if (_progressBarView == null)
                throw new NullReferenceException("ProgressBar has not been initialized.");
            _progressBarView.Increment();
        }

        [ComVisible(true)]
        public void Close()
        {
            if (_progressBarView == null)
                throw new NullReferenceException("ProgressBar has not been initialized.");
            _progressBarView.Close();
        }
    }
}
