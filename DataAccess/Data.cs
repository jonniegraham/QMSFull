using System.Runtime.InteropServices;
using Database;

namespace DataAccess
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IData
    {
        Products Products();
        Quotes Quotes();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Data : IData
    {
        private static Data _instance;
        private static IDatabase _database;
        private static Products _products;
        private static Quotes _quotes;

        private Data() { }

        public static Data Instance()
        {
            return _instance ?? (_instance = new Data());
        }

        [ComVisible(true)]
        public Products Products()
        {
            InitialiseDatabase();
            return _products ?? (_products = new Products(_database));
        }

        [ComVisible(true)]
        public Quotes Quotes()
        {
            InitialiseDatabase();
            return _quotes ?? (_quotes = new Quotes(_database));
        }

        private void InitialiseDatabase()
        {
            if (_database == null)
                _database = new MySqlDatabase();

        }
    }
}