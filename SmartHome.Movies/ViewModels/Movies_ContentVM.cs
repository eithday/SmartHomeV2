using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Common.Utilities;
using SmartHome.Movies.Models;
using SmartHome.Movies.Controls;
using MediaBrowser.ApiInteraction;
using System.Collections.ObjectModel;

namespace SmartHome.Movies.ViewModels
{
  public class Movies_ContentVM : VMBase
  {
    #region Variables
    private List<string> genreList;
    public ObservableCollection<List<MediaModel>> MovieCollection {get; set;}      
    #endregion

    public Movies_ContentVM()
    {
      SetGenreList();
      MovieCollection = new ObservableCollection<List<MediaModel>>();
      RefreshFullCatalog();
    }

    #region Data Access/Update

    private async void RefreshFullCatalog()
    {
      SetGenreList();
      List<MediaModel> tempCatalog;
      MediaAPI MAPI = new MediaAPI();
      foreach (string genre in genreList)
      {
        tempCatalog = await MAPI.RefreshMediaList(genre);
        if (tempCatalog.Count != 0)
        {
          tempCatalog[0].ParentCategory = genre;
          MovieCollection.Add(tempCatalog);
        }
      }
    }

    private void SetGenreList()
    {
      string[] Genres = { "Action", "Comedy", "Family", "Sci-Fi & Fantasy", "Thriller" };
      genreList = new List<string>(Genres);
    }

    #endregion
  }
}
