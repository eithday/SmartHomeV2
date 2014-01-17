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
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

namespace SmartHome.Movies.ViewModels
{
  public class Movies_ContentVM : VMBase
  {
    #region Variables
    private List<string> genreList;
    public ObservableCollection<List<MediaModel>> MovieCollection {get; set;}
    public int GenreSelectedIndex
    {
      get { return genreSelectedIndex; }
      set
      {
        genreSelectedIndex = value;
        NotifyPropertyChanged("GenreSelectedIndex");
      }
    } int genreSelectedIndex;
    public static int MovieSelectedIndex
    {
      get { return movieSelectedIndex; }
      set
      {
        movieSelectedIndex = value;
      }
    } static int movieSelectedIndex;
    #endregion

    public Movies_ContentVM()
    {
      Messenger.Default.Register<KeyEventArgs>(this, "SmartHome.Movies.Views.Movies_Content", shell_keydown);
      SetGenreList();
      MovieCollection = new ObservableCollection<List<MediaModel>>();
      RefreshFullCatalog();
      GenreSelectedIndex = 0;
      MovieSelectedIndex = 0;
    }

    private void shell_keydown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Right:
          MovieSelectedIndex++;
          break;
        case Key.Left:
          if (MovieSelectedIndex > 0)
          MovieSelectedIndex--;
          break;
        case Key.Up:
          if (GenreSelectedIndex > 0)          
            GenreSelectedIndex--;          
          break;
        case Key.Down:
          if (GenreSelectedIndex < MovieCollection.Count() - 1)         
            GenreSelectedIndex++;          
          break;
      }
      Messenger.Default.Send(GenreSelectedIndex, "Movies_Content_SetItemFocus");
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
