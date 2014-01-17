using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartHome.Netflix.Models
{
  public class CatalogTitleObjectMap
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
          case "id":
            //On the first element need to create new model 
            initializeCatalog();
            CT.Id = E.Value;
            CT.HDBoxArt = "http://cdn-1.nflximg.com/us/boxshots/ghd/" + E.Value.Replace("http://api-public.netflix.com/catalog/titles/movies/", "") + ".jpg";
            break; 
          case "title":
            CT.Title = E.Attribute("short").Value;
            break;
          case "link":
            parseLink(E);
            break;
          case "release_year":
            CT.ReleaseYear = E.Value;
            break;
          case "category":
            CT.Category.Add(E.Attribute("label").Value);
            break;
          case "average_rating":
            CT.AverageRating = E.Value;
            break;
          case "updated":
            CTList.Add(CT);
            break;
        }
      }
      return CTList;
    }

    private void parseLink(XElement E)
    {
      switch (E.Attribute("title").Value)
      {
        case "box art":
          foreach (var b in E.Element("box_art").Descendants("link"))
          {
            switch (b.Attribute("title").Value)
            {
              case "64pix width box art":
                CT.SmallCoverArt = b.Attribute("href").Value;
                break;
              case "150pix width box art":
                CT.MediumCoverArt = b.Attribute("href").Value;
                break;
              case "210pix width box art":
                CT.LargeCoverArt = b.Attribute("href").Value;
                break;
            }
          }
          break;
        case "cast":
          if (E.Element("people") != null)
          {
            foreach (var p in E.Element("people").Descendants("link"))
            {
              CT.Cast.Add(p.Attribute("title").Value);
            }
          }
          break;
        case "directors":
          if (E.Element("people") != null)
          {
            foreach (var p in E.Element("people").Descendants("link"))
            {
              CT.Directors.Add(p.Attribute("title").Value);
            }
          }
          break;
        case "synopsis":
          CT.Synopsis = E.Element("synopsis").Value;
          break;
        case "short synopsis":
          CT.ShortSynopsis = E.Element("short_synopsis").Value;
          break;
      }
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