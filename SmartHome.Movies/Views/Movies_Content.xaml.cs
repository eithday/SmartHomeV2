using GalaSoft.MvvmLight.Messaging;
using SmartHome.Movies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SmartHome.Movies.ViewModels;

namespace SmartHome.Movies.Views
{
  /// <summary>
  /// Interaction logic for Movies_Content.xaml
  /// </summary>
  public partial class Movies_Content : UserControl
  {
    public Movies_Content()
    {
      InitializeComponent();      
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Messenger.Default.Register<int>(this, "Movies_Content_SetItemFocus", setItemFocus);
      setItemFocus(0);
    }

    private void setItemFocus(int index)
    {
      if (GenreListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
      {
        ListViewItem myListViewItem =
         (ListViewItem)(GenreListView.ItemContainerGenerator.ContainerFromIndex(index));
        if (myListViewItem != null)
        {
          ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListViewItem);
          DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
          ListView myListView = (ListView)myDataTemplate.FindName("MediaListView", myContentPresenter);
          myListView.SelectedIndex = Movies_ContentVM.MovieSelectedIndex;
          myListView.BringIntoView();
          myListView.ScrollIntoView(myListView.SelectedItem);
          //myListView.Focus();
        }
      }      
    }

    

    private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        if (child != null && child is childItem)
          return (childItem)child;
        else
        {
          childItem childOfChild = FindVisualChild<childItem>(child);
          if (childOfChild != null)
            return childOfChild;
        }
      }
      return null;
    }   
  }
}
