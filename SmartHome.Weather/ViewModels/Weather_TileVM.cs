using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace SmartHome.Weather.ViewModels
{
  public class Weather_TileVM : INotifyPropertyChanged
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
    
    private System.Timers.Timer updateWeatherTimer;

    public string CurrentTemp
    {
      get { return currentTemp; }
      set
      {
        currentTemp = value;
        NotifyPropertyChanged("CurrentTemp");
      }
    } string currentTemp;

    public Weather_TileVM()
    {
      updateWeatherTimer = new Timer(5000);
      updateWeatherTimer.Elapsed += new ElapsedEventHandler(updateWeather);
      updateWeatherTimer.Start();
    }

    private void updateWeather(object sender, ElapsedEventArgs e)
    {
      Application.Current.Dispatcher.Invoke((Action)delegate()
      {
        updateWeather();
      });
    }

    private void updateWeather()
    {
      CurrentTemp = "40 Deg F";
    }
  }
}
