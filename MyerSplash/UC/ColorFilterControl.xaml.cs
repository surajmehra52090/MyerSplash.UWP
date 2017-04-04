﻿using MyerSplash.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.UC
{
    public sealed partial class ColorFilterControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public ColorFilterControl()
        {
            this.InitializeComponent();
        }

        private void ImageGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}