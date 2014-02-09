using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using TvDatabase;

namespace SmartHome.TV.Controls
{
  class TV_Control
  {
    #region Variables
    TvClientV2.TimeShiftSession currentSession;
    private System.Timers.Timer delayChannelChangeTimer;
    private bool delayChannelChange;
    private MediaElement mediaElement1;
    private bool timeshiftActive;
    private int currentChannel;
    private Uri TVSource;
    #endregion

    public TV_Control(MediaElement mE)
    {
      Messenger.Default.Register<int>(this, "EPGRequest_ChannelChange", EPG_Request);
      delayChannelChangeTimer = new System.Timers.Timer(1000);
      delayChannelChangeTimer.Elapsed += new ElapsedEventHandler(DelayedChannelChange);
      this.mediaElement1 = mE;
    }

    private void EPG_Request(int EPGchannel)
    {
      int ch = EPGchannel;
      if (ch > 21)
      {
        ch = ch + 1;
      }
      if (ch > 45)
      {
        ch = ch + 1;
      }
      tuneChannel(ch);
    }

    public void Channel_Up()
    {
      ResetChannelChangeTimer();
      currentChannel++;
    }

    public void Channel_Down()
    {
      ResetChannelChangeTimer();
      currentChannel--;
    }

    public void SetChannel(int num)
    {
      ResetChannelChangeTimer();
      if (delayChannelChange)
      {
        delayChannelChange = false;
        currentChannel = currentChannel + 2; //Gets Channel back to orginal to concate but still need to subtract 2 for offset which gets done everytime anyway
        currentChannel = int.Parse(currentChannel.ToString() + (num).ToString());
      }
      else
      {
        delayChannelChange = true;
        currentChannel = num;
      }
      //Need to not do anything for under 2
      //Need to subtract 2 from channelNumber for offset of starting channel being 2 
      //if above 21 need to offset by 3
      if (currentChannel < 21)
      {
        currentChannel = currentChannel - 2;
      }
      else
      {
        currentChannel = currentChannel - 3;
      }
    }

    public void SetChannel(string name)
    {
      ResetChannelChangeTimer();
      Channel channel = Enumerable.Single<Channel>((IEnumerable<Channel>)TvClientV2.Channels, (Func<Channel, bool>)(chan => chan.DisplayName == name));
      currentChannel = channel.SortOrder;
    }

    private async void DelayedChannelChange(object source, ElapsedEventArgs e)
    {
      delayChannelChangeTimer.Stop();
      //Timer is run on a separate thread so you need to invoke a dispatcher thread on the UI to control PlayTo
      Application.Current.Dispatcher.Invoke((Action)delegate()
      {
        tuneChannel(currentChannel);
      });
    }

    private void ResetChannelChangeTimer()
    {
      //Reset the channel change timer by stopping and starting it
      delayChannelChangeTimer.Stop();
      delayChannelChangeTimer.Start();
    }

    private async Task tuneChannel(int idChannel)
    {
      if (!TvClientV2.IsConnected)
        System.Windows.MessageBox.Show("TV NOT CONNECTED");
      Channel channel = Enumerable.Single<Channel>((IEnumerable<Channel>)TvClientV2.Channels, (Func<Channel, bool>)(chan => chan.SortOrder == idChannel));
      //HAVE TO STOP PLAYBACK AND CLOSE MEDIAELEMENT IN ORDER TO FREE LIVE BUFFER FILE FOR UPDATED CHANNEL
      //mediaElement1.Stop();
      //mediaElement1.Close();
      //mediaElement1.Source = null;
      currentSession = await TvClientV2.StartTimeShift(channel);
      //mediaElement1.Play();
      //mediaElement1.Source = TVSource;
      //mediaElement1.Play();               
      timeshiftActive = true;
    }

    public async Task TuneActiveTimeShift()
    {
      string activeFile = TvClientV2.CurrentSession.VirtualCard.TimeShiftFileName.Substring(19);
      string activeStream = @"\\HNHPD5\MP Timeshifting\" + activeFile;
      TVSource = new Uri(activeStream);
      mediaElement1.Source = TVSource;
      mediaElement1.Play();
      currentSession = TvClientV2.CurrentSession;
      timeshiftActive = true;
    }

    public async void ClearPlayback()
    {
      await TvClientV2.StopTimeShift();
      //mediaElement1.Stop();
      //mediaElement1.Close();
      timeshiftActive = false;
    }

    public void StopPlayback()
    {
      //mediaElement1.Stop();  
    }

    public void StartPlayback()
    {
      mediaElement1.Play();
    }
  }
}
