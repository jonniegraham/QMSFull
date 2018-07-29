using System;
using System.Runtime.InteropServices;
using System.Windows;
using Spinner.View;

namespace Spinner
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAnimation
    {
        void Start(Animation.LongProcess l);
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Animation : IAnimation
    {
        [ComVisible(true)]
        public delegate void LongProcess(Action a);

        private readonly WaitView _waitView;

        public Animation()
        {
            _waitView = new WaitView();
        }

        [ComVisible(true)]
        public void Start(LongProcess longProcess)
        {
            if (Application.Current == null)
                new Application();

            _waitView.Loaded += (sender, e) =>
            {
                longProcess.Invoke(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => { _waitView.Close(); });
                });
            };

            _waitView.ShowDialog();
        }
    }
}
