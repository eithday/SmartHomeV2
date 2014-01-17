using System;
using Google.GData.YouTube;
using Google.YouTube;
using Google.GData.Client;
using GalaSoft.MvvmLight.Messaging;
using System.Collections;
using System.Windows.Input;
using SmartHome.Common.Utilities;

namespace SmartHome.YouTube.ViewModels
{
  public class YouTube_ContentVM : VMBase
  {
    #region Variables
    private string clientID = "760249692721.apps.googleusercontent.com";
    private string clientSecret = "tqIgMZVjjBpTwppxIA3Jsb4q";
    private string developerKey = "AIzaSyChl9uRbOIBkGvZBjIp5-XKxv2oLasvyhk";
    private string developerKeyV2 = "AI39si4ntqvI4SoTuXEt9rKIBD2LIpKnVc8KAtR-H1gYVs20dnTGoVsM2XA3p5sae59AyGMMWvw9Ymu4GUGXXaR0oKG9xEPgXA";
    Stack _history = new Stack();  
    #endregion

    public YouTube_ContentVM()
    {
      //sample call http://gdata.youtube.com/feeds/api/videos?q=voltron
      //Look under OnNavigatedTo to start Search until adding a button to start search
       youTubeSearch("dog");
    }
 
    #region Youtube Control
    private void youTubeSearch(string searchTerm)
    {
        YouTubeRequestSettings settings =
        new YouTubeRequestSettings("SmartHome", developerKeyV2);
        YouTubeRequest request = new YouTubeRequest(settings);

        YouTubeQuery query = new YouTubeQuery(YouTubeQuery.DefaultVideoUri);

        //For Categories
        //AtomCategory category1 = new AtomCategory("News", YouTubeNameTable.KeywordSchema);
        //query.Categories.Add(new QueryCategory(category1);

        //For Playlist
        // YouTubeQuery query = new YouTubeQuery(YouTubeQuery.BaseUserUri);
        // Feed<Google.YouTube.Playlist> userPlayList = request.GetPlaylistsFeed("chad.hire@gmail.com");
        // foreach (Google.YouTube.Playlist p in userPlayList.Entries)
        // {
        //     Feed<PlayListMember> list = request.GetPlaylist(p);
        //     foreach (Google.YouTube.Video entry in list.Entries)
      //      {
      //          System.Windows.MessageBox.Show(entry.Title);
      //      }
      //  }
            

        //order results by the number of views (most viewed first)
        query.OrderBy = "viewCount";
        query.NumberToRetrieve = 50;

        // search for puppies and include restricted content in the search results
        // query.SafeSearch could also be set to YouTubeQuery.SafeSearchValues.Moderate
        query.Query = searchTerm;
        query.SafeSearch = YouTubeQuery.SafeSearchValues.None;

        Feed<Google.YouTube.Video> videoFeed = request.Get<Google.YouTube.Video>(query);

        foreach (Google.YouTube.Video entry in videoFeed.Entries)
        {
            //System.Windows.MessageBox.Show(entry.Title);
        }
    }
    #endregion
  }
}
