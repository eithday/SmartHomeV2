using System.Windows;
using System.Windows.Input;
using SmartHome.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Reflection;

namespace SmartHome
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    #region Variables
    public string NotificationCenter_Visibility
    {
      get { return notificationCenter_Visibility; }
      set
      {
        notificationCenter_Visibility = value;
        NotifyPropertyChanged("NotificationCenter_Visibility");
      }
    } string notificationCenter_Visibility;
    public string AppLauncher_Visibility
    {
      get { return appLauncher_Visibility; }
      set
      {
        appLauncher_Visibility = value;
        NotifyPropertyChanged("AppLauncher_Visibility");
        if (appLauncher_Visibility == "Visible")
          AppLauncher.Focus();
      }
    } string appLauncher_Visibility;
    public int AppSelectedIndex
    {
      get { return appSelectedIndex; }
      set
      {
        appSelectedIndex = value;
        NotifyPropertyChanged("AppSelectedIndex");
      }
    } int appSelectedIndex;

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }

    protected virtual void NotifyPropertyChangedAll(object inOjbect)
    {
      foreach (PropertyInfo pi in inOjbect.GetType().GetProperties())
      {
        NotifyPropertyChanged(pi.Name);
      }
    }
    public virtual void Refresh()
    {
      NotifyPropertyChangedAll(this);
    }
    #endregion // INotifyPropertyChanged Members

    public MainWindow()
    {
      InitializeComponent();
      StartupControl();     
    }

    private void StartupControl()
    {
      //Procedure:       
      //1) Initialize or set any Variables
      //2) Add each project as an applicatioin into AppLauncher with associated Tile and Content
      //3) Load TV as startup Application

      Application.Current.MainWindow.PreviewKeyDown += shell_KeyDown; 
      NotificationCenter_Visibility = "Hidden";
      AppLauncher_Visibility = "Hidden";
      AppLauncher.SelectedIndex = -1;

      AppLauncherControl.Instance.AddApplication(new SmartHome.SmartHome_Tile());
      AppLauncherControl.Instance.AddApplication(new SmartHome.TV.Views.TV_Tile(), new SmartHome.TV.Views.TV_Content());
      AppLauncherControl.Instance.AddApplication(new SmartHome.Control.Views.Control_Tile(), new SmartHome.Control.Views.Control_Content());
      AppLauncherControl.Instance.AddApplication(new SmartHome.Movies.Views.Movies_Tile(), new SmartHome.Movies.Views.Movies_Content());
      AppLauncherControl.Instance.AddApplication(new SmartHome.Netflix.Views.Netflix_Tile(), new SmartHome.Netflix.Views.Netflix_Content());
      AppLauncherControl.Instance.AddApplication(new SmartHome.Weather.Views.Weather_Tile(), new SmartHome.Weather.Views.Weather_Content());

      LoadedContent.Content = LoadedContentControl.Instance.LoadApplication(AppLauncherControl.AppContent[0]);
      AppLauncher.ItemsSource = AppLauncherControl.AppTiles;     
    }

    private void loadSelectedApp()
    {
      if (AppLauncher.SelectedIndex > 0)
      LoadedContent.Content = LoadedContentControl.Instance.LoadApplication(AppLauncherControl.AppContent[AppLauncher.SelectedIndex-1]);
    }

    private void shell_KeyDown(object sender, KeyEventArgs e)
    {
      Messenger.Default.Send(e, "globalKeyPress");
      switch (e.Key)
      {
        case Key.Right:
          if (AppLauncher_Visibility == "Visible")
          {
            if (AppSelectedIndex < AppLauncherControl.AppContent.Count)
              AppSelectedIndex++;
            if (AppSelectedIndex <= 0)

              AppSelectedIndex = 1;
          }
          break;
        case Key.Left:
          if (AppLauncher_Visibility == "Visible")
          {
            if (AppSelectedIndex > 1)
              AppSelectedIndex--;
          }
          break;
        case Key.Space:
          if (AppLauncher_Visibility == "Visible")
            AppLauncher_Visibility = "Hidden";
          else
            AppLauncher_Visibility = "Visible";
          break;
        case Key.Enter:
          if (AppLauncher_Visibility == "Visible")
            loadSelectedApp();
          break;
      }      
    }
  }
}
