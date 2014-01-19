using GalaSoft.MvvmLight.Messaging;
using Gentle.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TvControl;
using TvDatabase;

namespace SmartHome.TV.Controls
{
  public sealed class TvClientV2
  {
    #region Variables
    private static volatile TvClientV2 instance;
    private static object syncRoot = new Object();
    public static TvClientV2 Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new TvClientV2();
          }
        }
        return instance;
      }
    }
    public static TvClientV2.TimeShiftSession CurrentSession = new TvClientV2.TimeShiftSession();
    private static bool _IsConnected = false;
    private static IUser ControlUser;
    public static IList<Channel> Channels;
    public static IList<Card> Cards;
    private static string server;
    private static VirtualCard usedCard;
    public static bool IsConnected
    {
      get { return _IsConnected; }
    }
    #endregion

    public class TimeShiftSession
    {
      public IUser User;
      public VirtualCard VirtualCard;
    }

    public static bool GetDatabaseConnectionString(out string connStr, out string provider)
    {
      connStr = "";
      provider = "";
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(string.Format("{0}\\gentle.config", (object)System.Windows.Forms.Application.StartupPath));
        XmlNode xmlNode = xmlDocument.SelectSingleNode("/Gentle.Framework/DefaultProvider");
        XmlNode namedItem1 = xmlNode.Attributes.GetNamedItem("connectionString");
        XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("name");
        connStr = namedItem1.InnerText;
        provider = namedItem2.InnerText;
      }
      catch (Exception ex)
      {
        connStr = "";
        provider = "";
        return false;
      }
      return true;
    }

    public async static Task Connect(string tvservername)
    {
      string connStr;
      string provider;
      if (!TvClientV2.GetDatabaseConnectionString(out connStr, out provider))
        return;
      TvClientV2.server = tvservername;
      RemoteControl.HostName = tvservername;
      try
      {
        ProviderFactory.SetDefaultProviderConnectionString(connStr);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.InnerException.ToString());
      }
      TvClientV2.ControlUser = (IUser)new User();
      Channel channel = Channel.Retrieve(336);
      await TvClientV2.StartTimeShift(channel);
      TvClientV2._IsConnected = true;
      Messenger.Default.Send("TV_Connected", "TVLoaded");
    }

    public async static void Disconnect()
    {
      TvClientV2._IsConnected = false;
      await TvClientV2.StopTimeShift();
      RemoteControl.Clear();
    }

    public async static Task<TimeShiftSession> StartTimeShift(Channel channel)
    {
      if (ControlUser.CardId == -1)
      {
        ControlUser = new User(System.Environment.MachineName, true);
        CurrentSession = new TimeShiftSession();
      }
      VirtualCard card;
      TvResult tvResult;
      //if (CurrentSession.VirtualCard != null)   
      //await StopTimeShift(); //HAVE TO STOP TIMESHIFT IN ORDER FOR TVSERVER TO DELETE LIVE BUFFER
      tvResult = RemoteControl.Instance.StartTimeShifting(ref TvClientV2.ControlUser, channel.IdChannel, out card);
      TvClientV2.ControlUser.CardId = card.Id;
      TvClientV2.ControlUser.IdChannel = card.IdChannel;
      TvClientV2.TimeShiftSession timeShiftSession = new TvClientV2.TimeShiftSession()
      {
        User = ControlUser,
        VirtualCard = card,
      };
      CurrentSession = timeShiftSession;
      return timeShiftSession;
    }

    public async static Task StopTimeShift()
    {
      RemoteControl.Instance.StopTimeShifting(ref TvClientV2.ControlUser);
    }

    public async static Task Update()
    {
      TvClientV2.Channels = Channel.ListAll();
      TvClientV2.Cards = Card.ListAll();
      //TvClientV2.Groups = ChannelGroup.ListAll();
      //TvClientV2.RadioGroups = RadioChannelGroup.ListAll();   
      //TvClientV2.GroupMaps = GroupMap.ListAll();         
    }
  }
}