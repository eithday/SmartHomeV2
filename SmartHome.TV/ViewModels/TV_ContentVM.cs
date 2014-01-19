using GalaSoft.MvvmLight.Messaging;
using SmartHome.Common.Utilities;
using SmartHome.TV.Controls;
using SmartHome.TV.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartHome.TV.ViewModels
{
  public class TV_ContentVM : VMBase
  {
    #region Variables
    //TV VARIABLES 
    private TV_Control TV_Ctrl;
    private MediaElement me1;

    //ContentControl VARIABLES
    private ContentControl CC;
    public string ContentControl_Visibility
    {
      get { return contentControl_Visibility; }
      set
      {
        contentControl_Visibility = value;
        NotifyPropertyChanged("ContentControl_Visibility");
      }
    } string contentControl_Visibility;
    #endregion

    public TV_ContentVM()
    {
      ContentControl_Visibility = "Hidden";
      Messenger.Default.Register<MediaElement>(this, "Main", TV_Rendered);
      Messenger.Default.Register<ContentControl>(this, "Content", CC_Rendered);
      Messenger.Default.Register<string>(this, AppClosing);
      Messenger.Default.Register<string>(this, "TVLoaded", StartTV);
      Messenger.Default.Register<KeyEventArgs>(this, "SmartHome.TV.Views.TV_Content", shell_keydown);
      StartupControl();
    }

    private async void shell_keydown(KeyEventArgs e)
    {     
      int num;
      string numKey = e.Key.ToString().Substring(e.Key.ToString().Length - 1);
      if (int.TryParse(numKey, out num))
      {
        TV_Ctrl.SetChannel(num);
      }
      else
      {
        switch (e.Key)
        {
          case Key.Left:
            if (ContentControl_Visibility == "Visible")
            { }
            break;
          case Key.Right:
            if (ContentControl_Visibility == "Visible")
            { }
            break;
          case Key.Up:
            if (ContentControl_Visibility == "Visible")
            { }
            else
            {
              TV_Ctrl.Channel_Up();
            }
            break;
          case Key.Down:
            if (ContentControl_Visibility == "Visible")
            { }
            else
              TV_Ctrl.Channel_Down();
            break;
          case Key.Enter:
            //TV_Ctrl.SetChannel(EPG_Ctrl.Selected_Channel);
            break;
          case Key.Escape:
             TV_Ctrl.StopPlayback();
            break;
          case Key.Tab:
            if (ContentControl_Visibility == "Hidden")
            {
              ContentControl_Visibility = "Visible";
              Messenger.Default.Send("Visible", "TVCC");
            }
            else
            {
              ContentControl_Visibility = "Hidden";
              Messenger.Default.Send("Hidden", "TVCC");
            }
            break;
        }
      }
    }

    #region Startup Control
    private async void StartupControl()
    {
      //Connect to TV server (On Connect a timeshift is started automatically for quicker response: default channel 336 i.e. 2)
      await Task.Run(() => TvClientV2.Connect("HNHPD5"));
    }

    //ONCE TV CLIENT IS CONNECTED AND A CONFIRM TIMESHIFT STARTED MESSAGE FROM TVCLIENT.CONNECT() HAS BEEN RECEIVED
    //THEN TUNE FIRST CHANNEL NEED TO TUNE CHANNEL FROM CALLING THREAD SINCE TASK.RUN() WAS CALLED TO KEEP UI RESPONSIVE AND TO SET
    //VLC CONTROL OTHERWISE TUNE CHANNEL IS CALLED BEFORE TV_Ctrl is set and recieve a object null reference error.
    private async void StartTV(string obj)
    {
      await
      Application.Current.Dispatcher.InvokeAsync((Action)delegate()
      {
        TV_Ctrl.TuneActiveTimeShift();
        //After TV is Started Need to Get the Full listings before changing channel or loading guide 
        TvClientV2.Update();
        EPG EP_UserControl = new EPG();
        CC.Content = EP_UserControl;
      });

    }

    //ONCE MediaElement CONTROL IS RENDERED SEND TO TV_CONTROL FOR CORRECT INSTANCE
    private void TV_Rendered(MediaElement obj)
    {
      TV_Ctrl = new TV_Control(obj);
    }

    private void CC_Rendered(ContentControl obj)
    {
      this.CC = obj;
    }
    #endregion

    #region Application Control
    private void AppClosing(string c)
    {
      TV_Ctrl.ClearPlayback();
      TvClientV2.Disconnect();
    }
    #endregion
  }
}
