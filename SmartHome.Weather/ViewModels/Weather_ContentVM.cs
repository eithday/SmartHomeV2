using SmartHome.Weather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Common.Utilities;
using SmartHome.Weather.Controls;

namespace SmartHome.Weather.ViewModels
{
  class Weather_ContentVM : VMBase
  {
    #region Variables
    private string location;
    private string lastUpdate;
    private List<WeatherModel> FullWeatherData;

    public List<WeatherModel> ForecastWeatherdata
    {
      get { return forecastWeatherdata; }
      set
      {
        forecastWeatherdata = value;
        NotifyPropertyChanged("ExtendedWeatherdata");
      }
    } List<WeatherModel> forecastWeatherdata;

    public WeatherModel CurrentWeatherData
    {
      get { return currentWeatherData; }
      set
      {
        currentWeatherData = value;
        NotifyPropertyChanged("CurrentWeatherData");
      }
    } WeatherModel currentWeatherData;
    #endregion

    public Weather_ContentVM()
    {
      if (CurrentWeatherData == null)
      {
        SetLocation();
        YahooWeatherAPI YWAPI = new YahooWeatherAPI();
        FullWeatherData = YWAPI.RefreshWeatherData(location);
        SeparateWeatherData(FullWeatherData);
      }
    }
    private void SetLocation()
    {
      location = "2390624"; //WOEID for Defiance OH
    }

    private void SeparateWeatherData(List<WeatherModel> fullData)
    {
      currentWeatherData = fullData[0];
      fullData.RemoveAt(0);
      forecastWeatherdata = fullData;
    }

    private void SetImage()
    {
     // http://l.yimg.com/a/i/us/we/52/39.gif
    }
  }
}
