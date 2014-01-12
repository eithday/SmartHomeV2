using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartHome.Controls
{
  public sealed class LoadedContentControl
  {
    #region Variables
    private static volatile LoadedContentControl instance;
    private static object syncRoot = new Object();
    public static LoadedContentControl Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new LoadedContentControl();
          }
        }
        return instance;
      }
    }
    public static UserControl CurrentLoadedApplication
    {
      get { return currentLoadedApplication; }
    } static UserControl currentLoadedApplication;
    #endregion

    public UserControl LoadApplication(UserControl requestedApplication)
    {
      int startIndexOfFullName = requestedApplication.ToString().LastIndexOf('.') + 1;
      int lastIndexOfFullName = requestedApplication.ToString().IndexOf('_');
      string appTileName = requestedApplication.ToString().Substring(startIndexOfFullName, lastIndexOfFullName - startIndexOfFullName);
      foreach (UserControl Apps in AppLauncherControl.AppContent)
      {
        startIndexOfFullName = Apps.ToString().LastIndexOf('.') + 1;
        lastIndexOfFullName = Apps.ToString().IndexOf('_');
        string appContentName = Apps.ToString().Substring(startIndexOfFullName, lastIndexOfFullName - startIndexOfFullName);

        if (appTileName == appContentName)
        {
          currentLoadedApplication = Apps;
          break;
        }
      }      
      return currentLoadedApplication; 
    }
  }
}
