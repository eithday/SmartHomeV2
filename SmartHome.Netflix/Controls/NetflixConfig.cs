using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Netflix.Controls
{
  class NetflixConfig
  {
    private static string consumerSecret = "EewyCMRzyX";
    private static string consumerKey = "5rydjvyd2ygzsdjuqv6d4tpq";
    public static int defaultItemsCount = 30;
    public static string timeFormatStr = "{0} min";
    private static string token;
    private static string tokenSecret;

    public static string ConsumerKey
    {
      get
      {
        return NetflixConfig.consumerKey;
      }
    }

    public static string ConsumerSecret
    {
      get
      {
        return NetflixConfig.consumerSecret;
      }
    }

    public static string Token
    {
      get
      {
        return NetflixConfig.token;
      }
      set
      {
        NetflixConfig.token = value;
      }
    }

    public static string TokenSecret
    {
      get
      {
        return NetflixConfig.tokenSecret;
      }
      set
      {
        NetflixConfig.tokenSecret = value;
      }
    }

    static NetflixConfig()
    {
    }
  }
}
