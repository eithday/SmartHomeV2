using GalaSoft.MvvmLight.Messaging;
using SmartHome.Common.Utilities;
using SmartHome.TV.Controls;
using SmartHome.TV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace SmartHome.TV.ViewModels
{
  public class EPG_VM : VMBase
  {
    #region Variables
    private EPG_Control_V4 EPG_Ctrl;
    private bool guideLoadedSuccessfuly;
    private string sourceDirectory;
    private bool updatingSelectedImage;
    private System.Timers.Timer delayItemUpdateTimer;
    private bool delayItemUpdate;
    public EPG_SelectedItem EPG_Item
    {
      get { return epg_Item; }
      set
      {
        epg_Item = value;
        NotifyPropertyChanged("EPG_Item");
      }
    } EPG_SelectedItem epg_Item;
    public string EPG_ItemImageSource
    {
      get { return epg_ItemImageSource; }
      set
      {
        epg_ItemImageSource = value;
        NotifyPropertyChanged("EPG_ItemImageSource");
      }
    } string epg_ItemImageSource;
    public EPG_Time EPGTimeLine
    {
      get { return epgTimeLine; }
      set
      {
        epgTimeLine = value;
        NotifyPropertyChanged("EPGTimeLine");
      }
    } EPG_Time epgTimeLine;
    public EPG_Program_Channel EPG_Data
    {
      get { return epg_Data; }
      set
      {
        epg_Data = value;
        NotifyPropertyChanged("EPG_Data");
      }
    }  EPG_Program_Channel epg_Data;
    #endregion

    #region Startup Control
    public EPG_VM()
    {
      Startup();
    }
    public async void Startup()
    {
      //REGISTER FOR ANY MESSAGES AND INITIALIZE ANY ITEMS      
      Messenger.Default.Register<string>(this, "TVCC", setActiveStatus);
      guideLoadedSuccessfuly = false;
      delayItemUpdateTimer = new System.Timers.Timer(500);
      delayItemUpdateTimer.Elapsed += new ElapsedEventHandler(DelayedItemUpdate);
      //Load Default Directory
      string sourceApplication = System.IO.Directory.GetCurrentDirectory();
      int removeIndex = sourceApplication.IndexOf("SmartHome\\bin\\Debug");
      int removeLength = sourceApplication.Length - removeIndex;
      sourceDirectory = sourceApplication.Substring(0, removeIndex);
      EPG_Ctrl = new EPG_Control_V4(sourceDirectory);
      EPG_Data = await Task.Run(() => EPG_Ctrl.LoadGuide());
      EPGTimeLine = await Task.Run(() => EPG_Ctrl.setEPG_Times());
      EPG_Item = await Task.Run(() => EPG_Ctrl.UpdateSelectedItem());
      EPG_ItemImageSource = await Task.Run(() => EPG_Ctrl.UpdateSelectedItemImage());
      guideLoadedSuccessfuly = true;
    }
    #endregion

    #region EPG Navigation
    private void setActiveStatus(string status)
    {
      if (status == "Visible")
        Messenger.Default.Register<KeyEventArgs>(this, "SmartHome.TV.Views.TV_Content", shell_keydown);
      else
        Messenger.Default.Unregister<KeyEventArgs>(this, "SmartHome.TV.Views.TV_Content", shell_keydown);
    }

    private async void shell_keydown(KeyEventArgs e)
    {
      int num;
      string numKey = e.Key.ToString().Substring(e.Key.ToString().Length - 1);
      if (int.TryParse(numKey, out num))
      {
        //Start delay and then navigate EPG to selected channel.
      }
      else
      {
        switch (e.Key)
        {
          case Key.Left:
            EPG_Data = await Task.Run(() => EPG_Ctrl.Navigate_Item("Left"));
            EPGTimeLine = await Task.Run(() => EPG_Ctrl.updateEPG_Times());
            EPG_Item = await Task.Run(() => EPG_Ctrl.UpdateSelectedItem());
            ResetItemUpdateTimer(); //THIS WILL UPDATE SELECTED ITEM IMAGE AFTER DELAY SET ON STARTUP            
            break;
          case Key.Right:
            EPG_Data = await Task.Run(() => EPG_Ctrl.Navigate_Item("Right"));
            EPGTimeLine = await Task.Run(() => EPG_Ctrl.updateEPG_Times());
            EPG_Item = await Task.Run(() => EPG_Ctrl.UpdateSelectedItem());
            ResetItemUpdateTimer(); //THIS WILL UPDATE SELECTED ITEM IMAGE AFTER DELAY SET ON STARTUP            
            break;
          case Key.Up:
            EPG_Data = await Task.Run(() => EPG_Ctrl.Navigate_Item("Up"));
            EPG_Item = await Task.Run(() => EPG_Ctrl.UpdateSelectedItem());
            ResetItemUpdateTimer(); //THIS WILL UPDATE SELECTED ITEM IMAGE AFTER DELAY SET ON STARTUP   
            break;
          case Key.Down:
            EPG_Data = await Task.Run(() => EPG_Ctrl.Navigate_Item("Down"));
            EPG_Item = await Task.Run(() => EPG_Ctrl.UpdateSelectedItem());
            ResetItemUpdateTimer(); //THIS WILL UPDATE SELECTED ITEM IMAGE AFTER DELAY SET ON STARTUP            
            break;
          case Key.Enter:
            Messenger.Default.Send(EPG_Ctrl.Selected_Channel, "EPGRequest_ChannelChange");
            break;
        }
      }
    }
    #endregion

    #region EPG Updates
    private async void DelayedItemUpdate(object sender, ElapsedEventArgs e)
    {
      delayItemUpdateTimer.Stop();
      await
      Application.Current.Dispatcher.InvokeAsync((Action)delegate()
      {
        if (!updatingSelectedImage)
        {
          updatingSelectedImage = true;
          UpdateItem();
        }
      });
    }
    private async void UpdateItem()
    {
      EPG_ItemImageSource = await Task.Run(() => EPG_Ctrl.UpdateSelectedItemImage());
      updatingSelectedImage = false;
    }
    private void ResetItemUpdateTimer()
    {
      //Reset the channel change timer by stopping and starting it
      delayItemUpdateTimer.Stop();
      delayItemUpdateTimer.Start();
    }
    #endregion
  }
}