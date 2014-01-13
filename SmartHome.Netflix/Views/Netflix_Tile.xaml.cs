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

namespace SmartHome.Netflix.Views
{
  /// <summary>
  /// Interaction logic for Netflix_Tile.xaml
  /// </summary>
  public partial class Netflix_Tile : UserControl
  {
    public Netflix_Tile()
    {
      InitializeComponent();
      SetImageSources();
    }

    private void SetImageSources()
    {
      NetImg.Source = new ImageSourceConverter().ConvertFromString(Directory.GetCurrentDirectory() + @"\Images\netflix.png") as ImageSource; ;
    }
   
  }
}
