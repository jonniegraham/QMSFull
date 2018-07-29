using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// Base class where property-change notifications are required.
    /// i.e.    1.) ModelWrapper objects.
    ///         2.) ViewModel object.
    /// </summary>
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
