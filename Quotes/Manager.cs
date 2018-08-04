using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Quotes.View;
using TakeoffWrapper;
using Application = System.Windows.Application;

namespace Quotes
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IManager
    {
        void Initialize(Takeoff takeoff);
        void Initialize(_Worksheet worksheet);
        void Show();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Manager : IManager
    {
        private Takeoff _takeoff;
        [ComVisible(true)]
        private QuotesView _quotesView;

        [ComVisible(true)]
        public void Initialize(Takeoff takeoff)
        {
            _takeoff = takeoff;
            _quotesView = new QuotesView(ref takeoff);
            _quotesView.Closing += _quotesView_Closing;
            _quotesView.Closed += _quotesView_Closed;
        }

        private void _quotesView_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Closed");
        }

        [ComVisible(true)]
        public void Initialize(_Worksheet worksheet)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _takeoff = new Takeoff();
                    _takeoff.Initialize(worksheet);
                    if (Application.Current == null)
                        new Application();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _quotesView = new QuotesView(ref _takeoff);
                        _quotesView.Closing += _quotesView_Closing;
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        [ComVisible(true)]
        [STAThread]
        public void Show()
        {
            if (_quotesView == null)
                throw new NullReferenceException("Quotes Manager is not initialized.");
            if (Application.Current == null)
                new Application();
            _quotesView.ShowDialog();
        }

        private void _quotesView_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _quotesView.Hide();
            });
            e.Cancel = true;
        }
    }
}
