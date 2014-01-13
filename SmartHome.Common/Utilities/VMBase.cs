using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Common.Utilities
{
  public class VMBase : INotifyPropertyChanged
  {

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }

    protected virtual void NotifyPropertyChangedAll(object inOjbect)
    {
      foreach (PropertyInfo pi in inOjbect.GetType().GetProperties())
      {
        NotifyPropertyChanged(pi.Name);
      }
    }
    public virtual void Refresh()
    {
      NotifyPropertyChangedAll(this);
    }
    #endregion // INotifyPropertyChanged Members
  }
}
