using SmartHome.Common.Utilities;
using SmartHome.Netflix.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SmartHome.Netflix.Controls
{
  public class NetflixAPI
  {
    private List<CatalogTitleModel> searchResults;
    NetflixAuth _auth = new NetflixAuth("accessInfo.xml");
    Hashtable _authItems = new Hashtable();
    Hashtable _titleList = new Hashtable();
    string filePath = "c:\\temp\\netflixStreamingCatalog.xml";
    string _etag = "";	// opaque db sync state handle   

    public List<CatalogTitleModel> RefreshNetflixData(string searchRequest, string searchType)
    {
      switch (searchType)
      {
        case "genre":
          searchResults = genreRequest(searchRequest);
          break;
        case "person":
          searchResults = personRequest(searchRequest);
          break;
      }
      return searchResults;
    }

    private List<CatalogTitleModel> genreRequest(string searchRequest)
    {
      List<CatalogTitleModel> CTM = new List<CatalogTitleModel>();
      CatalogTitleObjectMap CTOM = new CatalogTitleObjectMap();
      LinqToXmlQuery LXQ = new LinqToXmlQuery();
      var queryResults = LXQ.QueryData(filePath, "catalog_title", "category", "label", searchRequest, 20);
      CTM = CTOM.MapObject(queryResults);
      return CTM;
    }

    private List<CatalogTitleModel> personRequest(string searchRequest)
    {
      List<CatalogTitleModel> CTM = new List<CatalogTitleModel>();
      CatalogTitleObjectMap CTOM = new CatalogTitleObjectMap();
      LinqToXmlQuery LXQ = new LinqToXmlQuery();
      var queryResults = LXQ.QueryData(filePath, "catalog_title", "link", "people", searchRequest, 20);
      CTM = CTOM.MapObject(queryResults);
      return CTM;
    }

    public async Task<List<CatalogTitleModel>> RefreshNetlixMyList()
    {
      List<CatalogTitleModel> CTM = new List<CatalogTitleModel>();
      NetflixRequest request = new NetflixRequest(NetflixConfig.ConsumerKey, NetflixConfig.ConsumerSecret,
                                                  _auth.Token, _auth.Secret);
      // override the default of 25 results - 100 is the max allowed
      request.AddQueryParameter("max_results", "100");
      string requestUrl = Constants.baseAPIUrl + _auth.UserID + "/queues/instant/available";
      XmlDocument xDoc;
      try
      {
        xDoc = await request.ProtectedRequestXml(requestUrl, "GET");
      }
      catch (Exception ex)
      {
       // System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
       // System.Windows.Forms.MessageBox.Show("error loading queue " + ex.Message);
        return null;
      }
      QueueTitleObjectMap QTOM = new QueueTitleObjectMap();
      LinqToXmlQuery LXQ = new LinqToXmlQuery();
      var queryResults = LXQ.QueryData(xDoc);
      CTM = QTOM.MapObject(queryResults);
      return CTM;
    }

    private async void UpdateInstantQueue(string TitleID, int Position, int EOQ, string Action)
    {
      // prepare update request
      NetflixRequest request = new NetflixRequest(NetflixConfig.ConsumerKey, NetflixConfig.ConsumerSecret,
                                       _auth.Token, _auth.Secret);
      string requestUrl = Constants.baseAPIUrl + _auth.UserID + "/queues/instant";
      request.AddQueryParameter("etag", _etag);
      string titleRef = "http://api-public.netflix.com/catalog/titles/movies/" + TitleID;
      request.AddQueryParameter("title_ref", titleRef);
      // dispatch on action-specific operations
      switch (Action)
      {
        case "POST":
          if (Position != 999)
          {
            // add title at requested position
            request.AddQueryParameter("position", Position.ToString());
          }
          // For an Add, the new title is just placed at the
          // end of the queue if no position is specified.
          break;
        case "MOVE":
          if (Position != 999)
          {
            // move title to requested position
            request.AddQueryParameter("position", Position.ToString());
          }
          else
          {
            // For a Move if you don't include the position the
            // update just leaves it where it is, so we have to tell it
            // specifically to move it to the end of the queue.
            request.AddQueryParameter("position", EOQ.ToString());
          }
          Action = "POST";
          break;
        case "DELETE":
          // for Delete, you have to specify the exact queue name
          requestUrl += "/available/" + TitleID;
          break;
      }
      try
      {
        XmlDocument xDoc;
        xDoc = await request.ProtectedRequestXml(requestUrl, Action);
      }
      catch (Exception ex)
      {
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        System.Windows.Forms.MessageBox.Show(ex.Message);
        return;
      }
      // reload to show changes (does its own wait cursor)
      //RefreshNetlixMyList();
    }
  }
}
