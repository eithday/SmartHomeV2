using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Common.Utilities;
using SmartHome.Netflix.Controls;
using System.Diagnostics;
using SmartHome.Netflix.Models;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Client;

namespace SmartHome.Netflix.ViewModels
{
  public class Netflix_ContentVM : VMBase
  {
    #region Variables;
    private int currentGenre;
    private string currentSelectedItemID;
    private TMDbClient client;
    public ObservableCollection<List<CatalogTitleModel>> NFMovieCollection { get; set; }
    public string NFStatus
    {
      get { return nfStatus; }
      set
      {
        nfStatus = value;
        NotifyPropertyChanged("NFStatus");
      }
    } string nfStatus;
    public string NFSelectedTitle
    {
      get { return nfSelectedTitle; }
      set
      {
        nfSelectedTitle = value;
        NotifyPropertyChanged("NFSelectedTitle");
      }
    } string nfSelectedTitle;
    public string NFSelectedDescription
    {
      get { return nfSelectedDescription; }
      set
      {
        nfSelectedDescription = value;
        NotifyPropertyChanged("NFSelectedDescription");
      }
    } string nfSelectedDescription;
    public string NFSelectedImageSource
    {
      get { return nfSelectedImageSource; }
      set
      {
        nfSelectedImageSource = value;
        NotifyPropertyChanged("NFSelectedImageSource");
      }
    } string nfSelectedImageSource;
    public int NFGenreSelectedIndex
    {
      get { return nfgenreSelectedIndex; }
      set
      {
        nfgenreSelectedIndex = value;
        NotifyPropertyChanged("NFGenreSelectedIndex");
      }
    } int nfgenreSelectedIndex;
    public static int NFMovieSelectedIndex
    {
      get { return nfmovieSelectedIndex; }
      set
      {
        nfmovieSelectedIndex = value;        
      }
    } static int nfmovieSelectedIndex;
    private List<string> genreList;
    #endregion

    public Netflix_ContentVM()
    {
      NFGenreSelectedIndex = 0;
      NFMovieSelectedIndex = 0;
      client = new TMDbClient("ecaa9ae8c8346269b53c80e2a61aa0ea");
      client.GetConfig();
      Messenger.Default.Register<string>(this, "LoadStatus", successfulLoad);
      NetflixUpdater NFU = new NetflixUpdater(); //NetflixUpdater checks for xml file and loads or updates if older than 7 days      
    }

    private void successfulLoad(string status)
    {
      if (status == "Successful")
      {
        Messenger.Default.Register<KeyEventArgs>(this, "SmartHome.Netflix.Views.Netflix_Content", shell_keydown);
        SetGenreList();
        NFMovieCollection = new ObservableCollection<List<CatalogTitleModel>>();
        RefreshFullCatalog();
      }
      if (status == "Updating")
      {
        NFStatus = "Updating Catalog";
      }
    }

    private void shell_keydown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Right:
          NFMovieSelectedIndex++;
          break;
        case Key.Left:
          if (NFMovieSelectedIndex > 0)
            NFMovieSelectedIndex--;
          break;
        case Key.Up:
          if (NFGenreSelectedIndex > 0)
            NFGenreSelectedIndex--;
          break;
        case Key.Down:
          if (NFGenreSelectedIndex < NFMovieCollection.Count() -1)
          NFGenreSelectedIndex++;
          break;
        case Key.Enter:
          Play();
          break;
      }
      Messenger.Default.Send(NFGenreSelectedIndex, "Netflix_Content_SetItemFocus");
      setSelectedImage();
    }

    private void setSelectedImage()
    {
      NFSelectedImageSource = NFMovieCollection[nfgenreSelectedIndex][NFMovieSelectedIndex].HDBoxArt;
      NFSelectedTitle = NFMovieCollection[nfgenreSelectedIndex][NFMovieSelectedIndex].Title;
      NFSelectedDescription = NFMovieCollection[nfgenreSelectedIndex][NFMovieSelectedIndex].Synopsis;
    }

    #region Data Access/Update
    private async void RefreshFullCatalog()
    {
      List<CatalogTitleModel> tempCatalog;
      NetflixAPI NAPI = new NetflixAPI();
      List<CatalogTitleModel> tempList = await NAPI.RefreshNetlixMyList();
      tempList[0].ParentCategory = "My List";
      NFMovieCollection.Insert(0, tempList);
      foreach (string genre in genreList)
      {
        tempCatalog = NAPI.RefreshNetflixData(genre, "genre");
        if (tempCatalog.Count != 0)
        {
          tempCatalog[0].ParentCategory = genre;
          NFMovieCollection.Add(tempCatalog);
        }
      }
      setSelectedImage();
    }

    private void SetGenreList()
    {
      string[] Genres = { "Action & Adventure", "Comedies", "TV Shows", "Children & Family Movies", "Sci-Fi & Fantasy", "Thrillers" };
      genreList = new List<string>(Genres);
    }
    #endregion

    #region Netflix Player
    private void Play()
    {
      //NEED TO REPLACE CONFIG.GETFOLDER WITH THE NFPLAYER DIRECTORY AND AUTOPOPULATE ARGUMENTS VS FIXED ALSO PLACE NFPLAYER IN OUTPUT FOLDER    
      string arguments = Constants.baseMovieUrl + NFMovieCollection[NFGenreSelectedIndex][NFMovieSelectedIndex].Id;
      Process.Start(Environment.CurrentDirectory + "\\NfPlayer.exe", arguments);
    }
    #endregion
  }
}
