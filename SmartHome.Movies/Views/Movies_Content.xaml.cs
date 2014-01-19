using GalaSoft.MvvmLight.Messaging;
using SmartHome.Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHome.Movies.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;
using System.IO;

namespace SmartHome.Movies.Views
{
  /// <summary>
  /// Interaction logic for Movies_Content.xaml
  /// </summary>
  public partial class Movies_Content : UserControl
  {
    public Movies_Content()
    {
      initVLCPlayer();
      InitializeComponent();      
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Messenger.Default.Register<int>(this, "Movies_Content_SetItemFocus", setItemFocus);
      Messenger.Default.Register<string>(this, "Movies_Content_mediaControl", mediaControl);
      setItemFocus(0);
    }

    private void setItemFocus(int index)
    {
      if (GenreListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
      {
        ListViewItem myListViewItem =
         (ListViewItem)(GenreListView.ItemContainerGenerator.ContainerFromIndex(index));
        if (myListViewItem != null)
        {
          ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListViewItem);
          DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
          ListView myListView = (ListView)myDataTemplate.FindName("MediaListView", myContentPresenter);          
          myListView.SelectedIndex = Movies_ContentVM.MovieSelectedIndex;
          myListView.BringIntoView();
          myListView.ScrollIntoView(myListView.SelectedItem);
        }
      }      
    }

    private void mediaControl(string request)
    {
      switch (request)
      {
        case "Play":
          if (!localVlcControl.IsPlaying)
          {
            localVlcControl.Media = new PathMedia(Movies_ContentVM.MSource);
            VLC_Container.Visibility = System.Windows.Visibility.Visible;
            localVlcControl.Play();
          }
          break;
        case "Stop":
          localVlcControl.Stop();
          VLC_Container.Visibility = System.Windows.Visibility.Hidden;
          break;
        case "Pause":
          localVlcControl.Pause();
          break;
        case "Next":
          localVlcControl.Next();
          break;
      }
    }

    private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        if (child != null && child is childItem)
          return (childItem)child;
        else
        {
          childItem childOfChild = FindVisualChild<childItem>(child);
          if (childOfChild != null)
            return childOfChild;
        }
      }
      return null;
    }

    private void initVLCPlayer()
    {
      if (Directory.Exists("C:\\Program Files\\VideoLAN\\VLC"))
      {
        // Set libvlc.dll and libvlccore.dll directory path
        VlcContext.LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
        // Set the vlc plugins directory path
        VlcContext.LibVlcPluginsPath = @"C:\Program Files\VideoLAN\VLC\plugins";
      }
      else
      {
        // Set libvlc.dll and libvlccore.dll directory path
        VlcContext.LibVlcDllsPath = @"C:\Program Files (x86)\VideoLAN\VLC";
        // Set the vlc plugins directory path
        VlcContext.LibVlcPluginsPath = @"C:\Program Files (x86)\VideoLAN\VLC\plugins";
      }

      /* Setting up the configuration of the VLC instance.
       * You can use any available command-line option using the AddOption function (see last two options). 
       * A list of options is available at 
       *     http://wiki.videolan.org/VLC_command-line_help
       * for example. */

      // Ignore the VLC configuration file
      VlcContext.StartupOptions.IgnoreConfig = true;
      // Enable file based logging
      VlcContext.StartupOptions.LogOptions.LogInFile = false;
      // Shows the VLC log console (in addition to the applications window)
      VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
      // Set the log level for the VLC instance
      VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

      // Disable showing the movie file name as an overlay
      VlcContext.StartupOptions.AddOption("--no-video-title-show");     
      VlcContext.StartupOptions.AddOption("--no-snapshot-preview");
      VlcContext.StartupOptions.AddOption("--no-overlay");
      VlcContext.StartupOptions.AddOption("--no-video-title");
      VlcContext.StartupOptions.AddOption("--no-osd");
      VlcContext.StartupOptions.AddOption("--no-dvdnav-menu");
      if (!VlcContext.IsInitialized)
      {
        // Initialize the VlcContext
        VlcContext.Initialize();
      }
    }
  }
}
