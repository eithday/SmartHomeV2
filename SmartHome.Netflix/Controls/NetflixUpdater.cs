using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Netflix.Controls
{
  public class NetflixUpdater : OAuthBase
  {    
    NetflixAuth _auth = new NetflixAuth("accessInfo.xml");
    Hashtable _authItems = new Hashtable();

    public  NetflixUpdater()
    {
      LoadCatalog();
    }

    private async void LoadCatalog()
    {
      // string requestUrl = "http://api-public.netflix.com/catalog/titles/streaming?v=2.0&expand=@seasons,@episodes,@awards";
      string requestUrl = "http://api-public.netflix.com/catalog/titles/streaming";
      NetflixRequest request =  new NetflixRequest(NetflixConfig.ConsumerKey, NetflixConfig.ConsumerSecret,
                                                _auth.Token, _auth.Secret);
      try
      {
        //Check if file exists if not make request to retrieve full streaming catalog
        string filePath = "c:\\temp\\netflixStreamingCatalog.xml";       
        if (!File.Exists(filePath) || File.GetCreationTime(filePath) < DateTime.Now.AddDays(-7))
        {
          if (File.Exists(filePath))
            File.Delete(filePath);
          Messenger.Default.Send("Updating", "LoadStatus");
          await Task.Run(() =>  request.CatalogRequest(requestUrl, "GET"));
        }
        Messenger.Default.Send("Successful", "LoadStatus");
      }
      catch (Exception ex)
      {
        Messenger.Default.Send("UnSuccessful", "LoadStatus");
      }
    }
  }
}