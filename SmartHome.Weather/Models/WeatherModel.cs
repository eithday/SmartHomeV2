using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Weather.Models
{
  public class WeatherModel
  {
    public string City { get; set; }
    public string Region { get; set; }
    public string WindChill { get; set; }
    public string WindDirection { get; set; }
    public string WindSpeed { get; set; }
    public string Humidity { get; set; }
    public string Visibility { get; set; }
    public string Pressure { get; set; }
    public string Sunset { get; set; }
    public string Sunrise { get; set; }
    public string Temp { get; set; }
    public string Text { get; set; }
    public string Code { get; set; }
    public string Background { get; set; }
    public string Date { get; set; }
    public string Low { get; set; }
    public string High { get; set; }
    public string Day { get; set; }    
  }
}
