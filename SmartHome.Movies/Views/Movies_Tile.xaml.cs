using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartHome.Movies.Views
{
  /// <summary>
  /// Interaction logic for Movies_Tile.xaml
  /// </summary>
  public partial class Movies_Tile : UserControl
  {
    public Movies_Tile()
    {
      InitializeComponent();
      SetImageSources();
    }

    private void SetImageSources()
    {
      MovieImg.Source = new ImageSourceConverter().ConvertFromString(Directory.GetCurrentDirectory() + @"\Images\Movies.gif") as ImageSource; 
    }
  }
}
