using SmartHome.Common.Utilities;
using SmartHome.Weather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Weather.Controls
{
  public class YahooWeatherAPI
  {
    public List<WeatherModel> RefreshWeatherData(string location)
    {
      List<WeatherModel> WM = new List<WeatherModel>();
      WeatherObjectMap WOM = new WeatherObjectMap();      
      LinqToXmlQuery LXQ = new LinqToXmlQuery();

      var queryResults = LXQ.QueryData("http://weather.yahooapis.com/forecastrss?w=" + location);
      WM = WOM.MapObject(queryResults);
      return WM;
    }
  }
}
