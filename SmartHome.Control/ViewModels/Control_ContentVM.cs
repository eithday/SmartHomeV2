using AdvancedHMIDrivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using SmartHome.Common.Utilities;
using GalaSoft.MvvmLight.Messaging;

namespace SmartHome.Control.ViewModels
{
  public class Control_ContentVM : VMBase
  {
    EthernetIPforCLXComm MyCLX = new EthernetIPforCLXComm();
    DispatcherTimer timerReadPLC = new DispatcherTimer();
   
    public string Status
    {
      get { return status; }
      set
      {
        status = value;
        NotifyPropertyChanged("Status");
      }
    } string status;

    public Control_ContentVM()
    {
      Messenger.Default.Register<string>(this, "Control_Content_HMIBtn", HMI_ButtonHandler);
      MyCLX.IPAddress = "10.75.13.200"; // This should be hardwired to ENBT card so no need to worry about it changing.

      timerReadPLC.Interval = TimeSpan.FromMilliseconds(500);
      timerReadPLC.Tick += new EventHandler(timerReadPLC_Tick);
      timerReadPLC.Start();
    }

    private void HMI_ButtonHandler(string btn)
    {
      if (btn == "Set")
        MyCLX.WriteData("HMI_BTN", "1");
      else
      {
        // MessageBox.Show("clicked ReSet");
        MyCLX.WriteData("HMI_BTN", "0");
        MyCLX.WriteData("HMI_RES_BTN", "1");
        MyCLX.WriteData("HMI_RES_BTN", "0");
      }
    }

    #region Page Navigation
    private void shell_KeyDown(KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Left:
          break;
        case Key.Right:
          break;
        case Key.Up:
          break;
        case Key.Down:
          break;
        case Key.Enter:
          break;
      }
    }
    #endregion

    void timerReadPLC_Tick(object sender, EventArgs e)
    {
      Status = MyCLX.ReadAny("HMI_BTN_OUT");
    }
  }    
}
