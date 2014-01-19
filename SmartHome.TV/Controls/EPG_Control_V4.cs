using SmartHome.TV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TvDatabase;
using TvdbLib;
using TvdbLib.Cache;

namespace SmartHome.TV.Controls
{
  class EPG_Control_V4
  {
    #region Variables
    private string sourceDirectory;
    private DateTime currentTime;
    private System.TimeSpan incrementalTime;
    private DateTime lastTime;
    private DateTime currentSelectedTime;
    private int currentChannel;
    private int lastChannel;
    private int selected_Program;
    private bool allowEPGDataUpdate;
    private EPG_Program_Channel epg_Data;
    private IList<Program> listdailyProgams;
    private List<ExtendedProgram> listExtendDailyPrograms;
    private EPG_Time epg_Times;
    private EPG_SelectedItem epg_SelectedItem;
    private string epg_SelectedItemImage;
    private string epg_Direction;
    private TvdbHandler tvdbHandler;
    private TMDbClient client;

    public int Selected_Channel
    {
      get { return selected_Channel; }
      set { selected_Channel = value; }
    } int selected_Channel;

    public int Channel_Count
    {
      get { return channel_Count; }
      set { channel_Count = value; }
    } int channel_Count;
    #endregion

    public EPG_Control_V4(string SourceDirectory)
    {
      this.sourceDirectory = SourceDirectory; ;
      if (DateTime.Now.Minute < 30)
        currentTime = DateTime.Now.AddMinutes(-DateTime.Now.Minute);
      else
        currentTime = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute);
      currentSelectedTime = currentTime;
      lastTime = currentTime.AddHours(2);
      currentChannel = 0;
      lastChannel = 2;
      incrementalTime = new System.TimeSpan(0, 30, 0);
      epg_Data = new EPG_Program_Channel();
      epg_Times = new EPG_Time();
      epg_SelectedItem = new EPG_SelectedItem();
      tvdbHandler = new TvdbHandler(new XmlCacheProvider("C:\\test"), "572328F18DC4374A");
      tvdbHandler.InitCache();
      client = new TMDbClient("ecaa9ae8c8346269b53c80e2a61aa0ea");
      client.GetConfig();
    }

    public async Task<EPG_Program_Channel> Navigate_Item(string Direction)
    {
      epg_Direction = Direction;
      switch (Direction)
      {
        case "Up":
          if (selected_Channel > 0)
          {
            selected_Channel--;
          }
          else
            selected_Channel = listExtendDailyPrograms.Count() - 3;
          break;
        case "Down":
          if (selected_Channel < listExtendDailyPrograms.Count() - 3)
          {
            selected_Channel++;
          }
          else
            selected_Channel = 0;
          break;
        case "Left":
          if (selected_Program > 0)
            selected_Program--;
          break;
        case "Right":
          if (selected_Program < listExtendDailyPrograms[selected_Channel].Programs.Count() - 4)
            selected_Program++;
          break;
      }
      checkForEPGUpdate();
      highlightSelectedItem();
      return epg_Data;
    }

    private void checkForEPGUpdate()
    {
      allowEPGDataUpdate = false;
      switch (epg_Direction)
      {
        case "Up":
          if (selected_Channel < currentChannel)
          {
            currentChannel--;
            lastChannel--;
            selected_Channel = currentChannel;
            allowEPGDataUpdate = true;
          }
          if (selected_Channel == listExtendDailyPrograms.Count() - 3)
          {
            currentChannel = selected_Channel;
            lastChannel = currentChannel + 2;
            allowEPGDataUpdate = true;
          }
          //setSelectedProgramToPreviousChannelTimeSlot();
          break;
        case "Down":
          if (selected_Channel > lastChannel)
          {
            currentChannel++;
            lastChannel++;
            selected_Channel = lastChannel;
            allowEPGDataUpdate = true;
          }
          if (selected_Channel == 0)
          {
            currentChannel = selected_Channel;
            lastChannel = currentChannel + 2;
            allowEPGDataUpdate = true;
          }
          //setSelectedProgramToPreviousChannelTimeSlot();
          break;
        case "Left":
          if (currentSelectedTime > DateTime.Now)
            currentSelectedTime = currentSelectedTime - incrementalTime;
          if (listExtendDailyPrograms[selected_Channel].Programs[selected_Program].StartTime < currentTime)
          {
            currentTime = currentTime - incrementalTime;
            lastTime = lastTime - incrementalTime;
            if (currentTime > DateTime.Now)
            {
              allowEPGDataUpdate = true;
            }
            else
            {
              if (DateTime.Now.Minute < 30)
                currentTime = DateTime.Now.AddMinutes(-DateTime.Now.Minute);
              else
                currentTime = DateTime.Now.AddMinutes(30 - DateTime.Now.Minute);
              lastTime = currentTime.AddHours(2);
            }
            currentSelectedTime = currentTime;
            updateTimes();
          }
          break;
        case "Right":
          currentSelectedTime = currentSelectedTime + incrementalTime;
          if (listExtendDailyPrograms[selected_Channel].Programs[selected_Program].EndTime > lastTime)
          {
            currentTime = currentTime + incrementalTime;
            lastTime = lastTime + incrementalTime;
            currentSelectedTime = lastTime;
            //selected_Program--;            
            updateTimes();
            allowEPGDataUpdate = true;
          }
          break;
      }
      setSelectedProgramWithinRange();
      if (allowEPGDataUpdate)
        updateEPGData();
    }

    private void setSelectedProgramWithinRange()
    {
      //CHECKS TO MAKE SURE PROGRAM IS INSIDE CURRENT TIME SLOT AND ADJUSTS IF NECESSARY
      while (listExtendDailyPrograms[selected_Channel].Programs[selected_Program].StartTime > lastTime - incrementalTime)
        selected_Program--;
      while (listExtendDailyPrograms[selected_Channel].Programs[selected_Program].EndTime < currentTime)
        selected_Program++;
    }

    private void setSelectedProgramToPreviousChannelTimeSlot()
    {
      int attempts = 6;
      int currentAttempts = 0;
      //TO DO:  NEED TO STORE THE CURRENT SELECTED TIME SLOT AND GO THROUGH AND SHIFT SELECTED PROGRAM START TIME   
      // ON CHANNEL UP AND DOWN TO MAKE SURE THE SELECTED PROGRAM IS ALWAYS ON THE RIGHT TIME INSTEAD OF THE SELECTED PROGRAM INDEX 
      while (!((currentSelectedTime > listExtendDailyPrograms[selected_Channel].Programs[selected_Program].StartTime) && (currentSelectedTime < listExtendDailyPrograms[selected_Channel].Programs[selected_Program].EndTime)))
      {
        currentAttempts++;
        if (currentAttempts < attempts)
          selected_Program--;
        else
          selected_Program++;
      }
    }

    private void updateEPGData()
    {
      //Channel offset is used to set the next program to account for the colspan
      int channel1_ColSpan = 0;
      int channel2_ColSpan = 0;
      int channel3_ColSpan = 0;
      clearEPG_Programs();
      //SET CHANNEL INFORMATION
      epg_Data.Channel1_Name = listExtendDailyPrograms[currentChannel].ChannelName;
      epg_Data.Channel1_Image = listExtendDailyPrograms[currentChannel].ChannelImage;
      epg_Data.Channel2_Name = listExtendDailyPrograms[currentChannel + 1].ChannelName;
      epg_Data.Channel2_Image = listExtendDailyPrograms[currentChannel + 1].ChannelImage;
      epg_Data.Channel3_Name = listExtendDailyPrograms[currentChannel + 2].ChannelName;
      epg_Data.Channel3_Image = listExtendDailyPrograms[currentChannel + 2].ChannelImage;
      //SET PROGRAM INFORMATION FIRST CHANNEL 
      foreach (Program p in listExtendDailyPrograms[currentChannel].Programs)
      {
        if ((currentTime > p.StartTime) && (currentTime < p.EndTime))
        {
          epg_Data.Program1_ID = p.IdProgram;
          epg_Data.Program1_Title = p.Title;
          TimeSpan programSpan = p.EndTime - currentTime;
          epg_Data.Program1_ColSpan = SetProgramLength(programSpan);
        }
        channel1_ColSpan = epg_Data.Program1_ColSpan;
        if (channel1_ColSpan < 2)
        {
          if ((currentTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program2_ID = p.IdProgram;
            epg_Data.Program2_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime);
            epg_Data.Program2_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel1_ColSpan = channel1_ColSpan + epg_Data.Program2_ColSpan;
        if (channel1_ColSpan < 3)
        {
          if ((currentTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program3_ID = p.IdProgram;
            epg_Data.Program3_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime + incrementalTime);
            epg_Data.Program3_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel1_ColSpan = channel1_ColSpan + epg_Data.Program3_ColSpan;
        if (channel1_ColSpan < 4)
        {
          if ((currentTime + incrementalTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program4_ID = p.IdProgram;
            epg_Data.Program4_Title = p.Title;
            epg_Data.Program4_ColSpan = 1;
          }
        }
      }
      //SET PROGRAM INFORMATION SECOND CHANNEL 
      foreach (Program p in listExtendDailyPrograms[currentChannel + 1].Programs)
      {
        if ((currentTime > p.StartTime) && (currentTime < p.EndTime))
        {
          epg_Data.Program5_ID = p.IdProgram;
          epg_Data.Program5_Title = p.Title;
          TimeSpan programSpan = p.EndTime - currentTime;
          epg_Data.Program5_ColSpan = SetProgramLength(programSpan);
        }
        channel2_ColSpan = epg_Data.Program5_ColSpan;
        if (channel2_ColSpan < 2)
        {
          if ((currentTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program6_ID = p.IdProgram;
            epg_Data.Program6_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime);
            epg_Data.Program6_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel2_ColSpan = channel2_ColSpan + epg_Data.Program6_ColSpan;
        if (channel2_ColSpan < 3)
        {
          if ((currentTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program7_ID = p.IdProgram;
            epg_Data.Program7_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime + incrementalTime);
            epg_Data.Program7_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel2_ColSpan = channel2_ColSpan + epg_Data.Program7_ColSpan;
        if (channel2_ColSpan < 4)
        {
          if ((currentTime + incrementalTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program8_ID = p.IdProgram;
            epg_Data.Program8_Title = p.Title;
            epg_Data.Program8_ColSpan = 1;
          }
        }
      }
      //SET PROGRAM INFORMATION SECOND CHANNEL 
      foreach (Program p in listExtendDailyPrograms[currentChannel + 2].Programs)
      {
        if ((currentTime > p.StartTime) && (currentTime < p.EndTime))
        {
          epg_Data.Program9_ID = p.IdProgram;
          epg_Data.Program9_Title = p.Title;
          TimeSpan programSpan = p.EndTime - currentTime;
          epg_Data.Program9_ColSpan = SetProgramLength(programSpan);
        }
        channel3_ColSpan = epg_Data.Program9_ColSpan;
        if (channel3_ColSpan < 2)
        {
          if ((currentTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program10_ID = p.IdProgram;
            epg_Data.Program10_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime);
            epg_Data.Program10_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel3_ColSpan = channel3_ColSpan + epg_Data.Program10_ColSpan;
        if (channel3_ColSpan < 3)
        {
          if ((currentTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program11_ID = p.IdProgram;
            epg_Data.Program11_Title = p.Title;
            TimeSpan programSpan = p.EndTime - (currentTime + incrementalTime + incrementalTime);
            epg_Data.Program11_ColSpan = SetProgramLength(programSpan);
          }
        }
        channel3_ColSpan = channel3_ColSpan + epg_Data.Program11_ColSpan;
        if (channel3_ColSpan < 4)
        {
          if ((currentTime + incrementalTime + incrementalTime + incrementalTime > p.StartTime) && (currentTime + incrementalTime + incrementalTime + incrementalTime < p.EndTime))
          {
            epg_Data.Program12_ID = p.IdProgram;
            epg_Data.Program12_Title = p.Title;
            epg_Data.Program12_ColSpan = 1;
          }
        }
      }

    }

    private void highlightSelectedItem()
    {
      clearEPG_Selection();
      if (epg_Data.Program1_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program1_Border = "2";
      else if (epg_Data.Program2_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program2_Border = "2";
      else if (epg_Data.Program3_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program3_Border = "2";
      else if (epg_Data.Program4_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program4_Border = "2";
      else if (epg_Data.Program5_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program5_Border = "2";
      else if (epg_Data.Program6_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program6_Border = "2";
      else if (epg_Data.Program7_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program7_Border = "2";
      else if (epg_Data.Program8_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program8_Border = "2";
      else if (epg_Data.Program9_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program9_Border = "2";
      else if (epg_Data.Program10_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program10_Border = "2";
      else if (epg_Data.Program11_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program11_Border = "2";
      else if (epg_Data.Program12_ID == listExtendDailyPrograms[selected_Channel].Programs[selected_Program].IdProgram)
        epg_Data.Program12_Border = "2";
    }

    private void clearEPG_Programs()
    {
      epg_Data.Program1_Title = "";
      epg_Data.Program1_ColSpan = 0;
      epg_Data.Program1_ID = 0;
      epg_Data.Program2_Title = "";
      epg_Data.Program2_ColSpan = 0;
      epg_Data.Program2_ID = 0;
      epg_Data.Program3_Title = "";
      epg_Data.Program3_ColSpan = 0;
      epg_Data.Program3_ID = 0;
      epg_Data.Program4_Title = "";
      epg_Data.Program4_ColSpan = 0;
      epg_Data.Program4_ID = 0;
      epg_Data.Program5_Title = "";
      epg_Data.Program5_ColSpan = 0;
      epg_Data.Program5_ID = 0;
      epg_Data.Program6_Title = "";
      epg_Data.Program6_ColSpan = 0;
      epg_Data.Program6_ID = 0;
      epg_Data.Program7_Title = "";
      epg_Data.Program7_ColSpan = 0;
      epg_Data.Program7_ID = 0;
      epg_Data.Program8_Title = "";
      epg_Data.Program8_ColSpan = 0;
      epg_Data.Program8_ID = 0;
      epg_Data.Program9_Title = "";
      epg_Data.Program9_ColSpan = 0;
      epg_Data.Program9_ID = 0;
      epg_Data.Program10_Title = "";
      epg_Data.Program10_ColSpan = 0;
      epg_Data.Program10_ID = 0;
      epg_Data.Program11_Title = "";
      epg_Data.Program11_ColSpan = 0;
      epg_Data.Program11_ID = 0;
      epg_Data.Program12_Title = "";
      epg_Data.Program12_ColSpan = 0;
      epg_Data.Program12_ID = 0;
    }

    private void clearEPG_Selection()
    {
      epg_Data.Program1_Border = "0";
      epg_Data.Program1_Margin = "1";
      epg_Data.Program2_Border = "0";
      epg_Data.Program2_Margin = "1";
      epg_Data.Program3_Border = "0";
      epg_Data.Program3_Margin = "1";
      epg_Data.Program4_Border = "0";
      epg_Data.Program4_Margin = "1";
      epg_Data.Program5_Border = "0";
      epg_Data.Program5_Margin = "1";
      epg_Data.Program6_Border = "0";
      epg_Data.Program6_Margin = "1";
      epg_Data.Program7_Border = "0";
      epg_Data.Program7_Margin = "1";
      epg_Data.Program8_Border = "0";
      epg_Data.Program8_Margin = "1";
      epg_Data.Program9_Border = "0";
      epg_Data.Program9_Margin = "1";
      epg_Data.Program10_Border = "0";
      epg_Data.Program10_Margin = "1";
      epg_Data.Program11_Border = "0";
      epg_Data.Program11_Margin = "1";
      epg_Data.Program12_Border = "0";
      epg_Data.Program12_Margin = "1";
    }

    private int SetProgramLength(TimeSpan t)
    {
      int tempMinutes = 0;
      decimal totalMinutes = 0;
      int sections = 0;
      if (t.Hours > 0)
      {
        tempMinutes = t.Hours * 60;
      }
      totalMinutes = tempMinutes + t.Minutes;
      sections = Convert.ToInt16(Math.Round(totalMinutes / 30));
      return sections;
    }

    public async Task<EPG_SelectedItem> UpdateSelectedItem()
    {
      Program p = listExtendDailyPrograms[selected_Channel].Programs[selected_Program];
      epg_SelectedItem.Title = p.Title;
      epg_SelectedItem.Description = p.Description;
      epg_SelectedItem.Times = p.StartTime.ToShortTimeString() + p.EndTime.ToShortTimeString();
      return epg_SelectedItem;
    }

    public async Task<string> UpdateSelectedItemImage()
    {
      Program p = listExtendDailyPrograms[selected_Channel].Programs[selected_Program];
      var results = tvdbHandler.SearchSeries(p.Title);
      if (results.Count > 0)
      {
        StringBuilder sb = new StringBuilder("http://www.thetvdb.com/banners/_cache");
        if (results[0].Banner.BannerType.ToString() != "none")
        {
          sb.Append("/" + results[0].Banner.BannerType);
        }
        sb.Append("/" + results[0].Banner.BannerPath);

        epg_SelectedItemImage = sb.ToString();
      }
      else
      {
        SearchContainer<SearchMovie> movResults = client.SearchMovie(p.Title, 1);
        if (movResults.Results.Count > 0)
        {
          StringBuilder sb = new StringBuilder("https://image.tmdb.org/t/p/w185");
          sb.Append(movResults.Results[0].PosterPath);
          epg_SelectedItemImage = sb.ToString();
        }
        else
        {
          epg_SelectedItemImage = sourceDirectory + "SmartHome.Modules.LiveTV\\ChannelLogos\\default.png";
        }
      }

      tvdbHandler.CloseCache();
      return epg_SelectedItemImage;
    }

    public EPG_Time setEPG_Times()
    {
      int tempMinutes = 0;
      epg_Times.Time1 = DateTime.Now.DayOfWeek.ToString() + DateTime.Now.ToShortDateString();
      epg_Times.Time2 = DateTime.Now.ToShortTimeString();

      if (DateTime.Now.Minute < 30)
        tempMinutes = 30 - DateTime.Now.Minute;
      else
        tempMinutes = 60 - DateTime.Now.Minute;

      epg_Times.Time3 = DateTime.Now.AddMinutes(tempMinutes).ToShortTimeString();
      epg_Times.Time4 = DateTime.Now.AddMinutes(tempMinutes + 30).ToShortTimeString();
      epg_Times.Time5 = DateTime.Now.AddMinutes(tempMinutes + 60).ToShortTimeString();

      return epg_Times;
    }

    public async Task<EPG_Time> updateEPG_Times()
    {
      return epg_Times;
    }

    private void updateTimes()
    {
      int tempMinutes = 0;
      DateTime currentSelectedStartTime = currentTime;
      if (currentSelectedStartTime.Minute < 30)
        tempMinutes = 30 - currentSelectedStartTime.Minute;
      else
        tempMinutes = 60 - currentSelectedStartTime.Minute;
      epg_Times.Time1 = currentSelectedStartTime.DayOfWeek.ToString() + currentSelectedStartTime.ToShortDateString();
      if (currentTime <= DateTime.Now)
        epg_Times.Time2 = DateTime.Now.ToShortTimeString();
      else
        epg_Times.Time2 = currentSelectedStartTime.ToShortTimeString();
      epg_Times.Time3 = currentSelectedStartTime.AddMinutes(tempMinutes).ToShortTimeString();
      epg_Times.Time4 = currentSelectedStartTime.AddMinutes(tempMinutes + 30).ToShortTimeString();
      epg_Times.Time5 = currentSelectedStartTime.AddMinutes(tempMinutes + 60).ToShortTimeString();
    }

    public async Task<EPG_Program_Channel> LoadGuide()
    {
      listExtendDailyPrograms = new List<ExtendedProgram>();
      int lastChannelIndex = TvClientV2.Channels.Count();
      for (int i = lastChannelIndex - 1; i > 0; i--)
      {
        Channel c = TvClientV2.Channels[i];
        listdailyProgams = Program.RetrieveDaily(DateTime.Now, DateTime.Now.AddHours(24), c.IdChannel);
        if (listdailyProgams.Count > 0)
        {
          ExtendedProgram EP = new ExtendedProgram(0, 0, DateTime.Now, DateTime.Now, "", "", "", Program.ProgramState.None, DateTime.Now, "", "", "", "", 0, "", 0);
          if (File.Exists(sourceDirectory + "SmartHome.Modules.LiveTV\\ChannelLogos\\" + c.DisplayName + ".png"))
          {
            EP.ChannelImage = sourceDirectory + "SmartHome.Modules.LiveTV\\ChannelLogos\\" + c.DisplayName + ".png";
          }
          else
            EP.ChannelImage = sourceDirectory + "SmartHome.Modules.LiveTV\\ChannelLogos\\default.png";
          EP.Programs = new List<Program>();
          foreach (Program p in listdailyProgams)
          {
            TimeSpan pLength = p.EndTime - p.StartTime;
            p.StarRating = SetProgramLength(pLength); //Repurpose StarRating for Program Length to set control width
            EP.ChannelName = c.DisplayName;
            EP.Programs.Add(p);
          }
          listExtendDailyPrograms.Add(EP);
        }
      }
      channel_Count = listExtendDailyPrograms.Count();
      updateEPGData();
      highlightSelectedItem();
      return epg_Data;
    }
  }
}
