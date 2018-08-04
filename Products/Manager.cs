using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Products.View;
using Spinner;
using TakeoffWrapper;
using Application = System.Windows.Application;

namespace Products
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IManager
    {
        void Initialize(Takeoff takeoff);
        void Initialize(_Worksheet worksheet);
        void Show();
        void SetCurrentRow(int row);
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Manager : IManager
    {
        private Takeoff _takeoff;
        [ComVisible(true)]
        private ProductsView _productsView;

        [ComVisible(true)]
        public void Initialize(Takeoff takeoff)
        {
            _takeoff = takeoff;
            _productsView = new ProductsView(ref takeoff);
            _productsView.Closing += _productsView_Closing;
        }
        
        [ComVisible(true)]
        [STAThread]
        public void Initialize(_Worksheet worksheet)
        {
                var s = new Animation();
                s.Start(close =>
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
                                _productsView = new ProductsView(ref _takeoff);
                            });


                        close();
                    }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });

                _productsView.Closing += _productsView_Closing;
        }
        
        [ComVisible(true)]
        [STAThread]
        public void Show()
        {
            if (_productsView == null)
                throw new NullReferenceException("Products Manager is not initialized.");
            if (Application.Current == null)
                new Application();

            _productsView.ShowDialog();
        }

        [ComVisible(true)]
        public void SetCurrentRow(int row)
        {
            _takeoff.Worksheet.Rows.Number = row;
        }

        private void _productsView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => _productsView.Hide());
            e.Cancel = true;
        }
    }
}
