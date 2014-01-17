using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartHome.Netflix.Models
{
  class QueueTitleObjectMap
  {
    List<CatalogTitleModel> CTList;
    CatalogTitleModel CT;

    public List<CatalogTitleModel> MapObject(List<XElement> el)
    {
      CTList = new List<CatalogTitleModel>();
      foreach (var E in el.Elements())
      {
        switch (E.Name.LocalName)
        {
          case "queue_item":
            //On the first element need to create new model             
            parseItem(E);
            break;
        }
      }
      return CTList;
    }

    private void parseItem(XElement E)
    {
      foreach (var i in E.Elements())
      {
        switch (i.Name.LocalName)
        {
          case "id":
            initializeCatalog();
            CT.Id = i.Value;
            CT.HDBoxArt = "http://cdn-1.nflximg.com/us/boxshots/ghd/" + i.Value.Substring(i.Value.Count()-8,8) + ".jpg";           
            break;
          case "link":
            parseLink(i);
            break;
          case "box_art":
            CT.SmallCoverArt = i.Attribute("small").Value;
            CT.MediumCoverArt = i.Attribute("medium").Value;
            CT.LargeCoverArt = i.Attribute("large").Value;
            break;
          case "title":
            CT.Title = i.Attribute("regular").Value;
            break;
          case "release_year":
            CT.ReleaseYear = i.Value;
            break;
          case "average_rating":
            CT.AverageRating = i.Value;
            CTList.Add(CT); //May Need to move this to parseLink if want to capture some of the link items
            break;
          case "category":
            CT.Category.Add(i.Attribute("label").Value);
            break;
        }
      }
    }

    private void parseLink(XElement E)
    {
      //Add Later if necessary links included links to synopsis and similars etc...
    }

    private void initializeCatalog()
    {
      CT = new CatalogTitleModel();
      CT.Category = new List<string>();
      CT.Cast = new List<string>();
      CT.Directors = new List<string>();
    }
  }
}
