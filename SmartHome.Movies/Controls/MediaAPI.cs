using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using SmartHome.Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHome.Movies.Controls
{
  public class MediaAPI
  {
    private ApiClient apiClient;

    private async Task setApiReference()
    {
      while (apiClient == null)
      {
        apiClient = BaseMediaBrowserAPI.Instance.publicAPIClient;
      }
    }

    public async Task<List<MediaModel>> RefreshMediaList(string searchRequest)
    {
      await setApiReference();
      List<MediaModel> LMM = new List<MediaModel>();
      MediaObjectMap MOM = new MediaObjectMap();
      string[] genreSearch = new string[1];
      genreSearch[0] = searchRequest;
      var totalItems = 30;
      try
      {
        var result = await apiClient.GetItemsAsync(new ItemQuery
        {
          UserId = apiClient.CurrentUserId,
          IncludeItemTypes = new[] { "Movie" },
          Genres = genreSearch,
          Limit = totalItems,
          SortBy = new[] { ItemSortBy.DateCreated },
          SortOrder = MediaBrowser.Model.Entities.SortOrder.Descending,
          Recursive = true,
          ImageTypes = new[] { ImageType.Backdrop },
          Filters = new[] { ItemFilter.IsUnplayed },
          Fields = new[] {
              ItemFields.Path,
              ItemFields.MediaStreams,
              ItemFields.Genres,
              ItemFields.Overview,
              ItemFields.People,
              }
        });
        var items = result.Items.ToList();
        LMM = MOM.MapObject(items, apiClient);
      }
      catch (Exception e)
      {
        // System.Windows.MessageBox.Show("Error Accessing Media in MediaAPI:  " + e.Message);
      }
      return LMM;
    }
  }
}
