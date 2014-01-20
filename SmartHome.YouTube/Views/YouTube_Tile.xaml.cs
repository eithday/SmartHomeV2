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

namespace SmartHome.YouTube.Views
{
  /// <summary>
  /// Interaction logic for YouTube_Tile.xaml
  /// </summary>
  public partial class YouTube_Tile : UserControl
  {
    public YouTube_Tile()
    {
      InitializeComponent();
      SetImageSources();
    }

    private void SetImageSources()
    {
      YoutubeImg.Source = new ImageSourceConverter().ConvertFromString(Directory.GetCurrentDirectory() + @"\Images\YouTube.png") as ImageSource; ;
    }
  }
}
