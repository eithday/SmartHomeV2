﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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

namespace SmartHome.Control.Views
{
  /// <summary>
  /// Interaction logic for Control_Content.xaml
  /// </summary>
  public partial class Control_Content : UserControl
  {
    public Control_Content()
    {
      InitializeComponent();
    }

    private void PLCBTN_Click(object sender, RoutedEventArgs e)
    {
      Messenger.Default.Send("Set", "Control_Content_HMIBtn");
    }

    private void PLCBTN_Reset_Click(object sender, RoutedEventArgs e)
    {
      Messenger.Default.Send("ReSet", "Control_Content_HMIBtn");
    }

  }
}
