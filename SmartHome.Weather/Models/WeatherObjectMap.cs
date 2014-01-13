using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartHome.Weather.Models
{
  public class WeatherObjectMap
  {
    List<WeatherModel> WDList;
    WeatherModel WD;
    bool firstDay;

    public List<WeatherModel> MapObject(List<XElement> el)
    {
      WDList = new List<WeatherModel>();
      firstDay = true;
      foreach (var E in el.Elements("channel").Descendants())
      {
        switch (E.Name.LocalName)
        {
          case "location":
            WD = new WeatherModel();
            WD.City = E.Attribute("city").Value;
            WD.Region = E.Attribute("region").Value;
            break;
          case "wind":
            WD.WindChill = E.Attribute("chill").Value;
            WD.WindDirection = E.Attribute("direction").Value;
            WD.WindSpeed = E.Attribute("speed").Value;
            break;
          case "atmosphere":
            WD.Humidity = E.Attribute("humidity").Value;
            WD.Visibility = E.Attribute("visibility").Value;
            WD.Pressure = E.Attribute("pressure").Value;
            break;
          case "astronomy":
            WD.Sunrise = E.Attribute("sunrise").Value;
            WD.Sunset = E.Attribute("sunset").Value;
            break;
          case "condition":
            WDList = new List<WeatherModel>();
            WDList.Add(WD);
            WD.Text = E.Attribute("text").Value;
            WD.Code = @"http://l.yimg.com/a/i/us/we/52/" + E.Attribute("code").Value + ".gif";
            WD.Temp = E.Attribute("temp").Value;
            WD.Date = E.Attribute("date").Value;
            WD.Background = Directory.GetCurrentDirectory() + @"\Images\Weather_Backgrounds\" + E.Attribute("code").Value + ".jpg";
            break;
          case "forecast":
            WD = new WeatherModel();
            if (firstDay)
            {
              WDList[0].Day = E.Attribute("day").Value;
              WDList[0].High = E.Attribute("high").Value;
              WDList[0].Low = E.Attribute("low").Value;
            }
            WD.Day = E.Attribute("day").Value;
            WD.Date = E.Attribute("date").Value;
            WD.Low = E.Attribute("low").Value;
            WD.High = E.Attribute("high").Value;
            WD.Text = E.Attribute("text").Value;
            WD.Code = @"http://l.yimg.com/a/i/us/we/52/" + E.Attribute("code").Value + ".gif";;
            WDList.Add(WD);
            firstDay = false;
            break;
        }
      }
      return WDList;
    }
  }
}
