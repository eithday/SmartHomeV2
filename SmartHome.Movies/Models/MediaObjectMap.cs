using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Movies.Models
{
  public class MediaObjectMap
  {
    List<MediaModel> MMList;
    MediaModel MM;

    public List<MediaModel> MapObject(List<BaseItemDto> items, ApiClient client)
    {
      MMList = new List<MediaModel>();
      var imageoptions = new ImageOptions
      {
        ImageType = ImageType.Primary,
        Quality = 100
      };

      var backdropImage = new ImageOptions
      {
        ImageType = ImageType.Backdrop,
        Quality = 100
      };

      foreach (BaseItemDto item in items)
      {
        if (item.HasPrimaryImage)
        {
          initializeCatalog();
          MM.LargeCoverArt = client.GetImageUrl(item, imageoptions);
          MM.BackDrop = client.GetImageUrl(item, backdropImage);
          MM.Title = item.Name;
          MM.Id = item.Path;
          MM.ReleaseYear = item.ProductionYear.ToString();
          MM.Synopsis = item.Overview;
          MM.AverageRating = item.OfficialRating;
          foreach (string g in item.Genres)
          {
            MM.Category.Add(g);
          }
          foreach (BaseItemPerson p in item.People)
          {
            MM.Cast.Add(p.Name);
          }
          MMList.Add(MM);
        }
      }
      return MMList;
    }

    private void initializeCatalog()
    {
      MM = new MediaModel();
      MM.Category = new List<string>();
      MM.Cast = new List<string>();
      MM.Directors = new List<string>();
    }
  }
}

