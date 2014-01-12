using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Controls
{
  public sealed class NotficationCenterControl
  {
    #region Variables
    private static volatile NotficationCenterControl instance;
    private static object syncRoot = new Object();
    public static NotficationCenterControl Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new NotficationCenterControl();
          }
        }
        return instance;
      }
    }
    public static string CurrentLoadedNotification
    {
      get { return currentLoadedNotification; }
    } static string currentLoadedNotification;
    #endregion

  }
}
