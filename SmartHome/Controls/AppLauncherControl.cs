using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartHome.Controls
{
  public sealed class AppLauncherControl
  {
    #region Variables
    private static volatile AppLauncherControl instance;
    private static bool init;
    private static object syncRoot = new Object();
    public static AppLauncherControl Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new AppLauncherControl();
          }
        }
        return instance;
      }
    }
    public static ObservableCollection<UserControl> AppTiles
    {
      get { return appTiles; }
    } static ObservableCollection<UserControl> appTiles;
    public static ObservableCollection<UserControl> AppContent
    {
      get { return appContent; } 
    } static ObservableCollection<UserControl> appContent;

    private void Initialize()
    {
      appTiles = new ObservableCollection<UserControl>();
      appContent = new ObservableCollection<UserControl>();
      init = true;
    }

    public void AddApplication(UserControl tile, UserControl content)
    {
      if (!init)
        Initialize();
      
      appTiles.Add(tile);
      appContent.Add(content);      
    }

    #endregion
  }
}
