﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace HamburgerMenu
{
    /// <summary>
    /// Interaction logic for NavMenuToggleButton.xaml
    /// </summary>
    public partial class NavMenuToggleButton : UserControl
    {
        public NavMenuToggleButton()
        {
            InitializeComponent();
            MouseClickManager manager = new MouseClickManager(200);
            this.MouseLeftButtonDown += manager.OnMouseLeftButtonDown;
            this.MouseLeftButtonUp += manager.OnMouseLeftButtonUp;
            manager.Click += (object sender, MouseButtonEventArgs e) => { OnClicked(); };
        }

        #region properties

        /// <summary>
        /// Цвет заливки прямоугольников кнопки.
        /// </summary>
        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(NavMenuToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Получает или задает положение прямоугольников кнопки(паралельное или крестом).
        /// </summary>
        public bool IsParallel
        {
            get { return (bool)GetValue(IsParallelProperty); }
            set
            {
                SetValue(IsParallelProperty, value);
            }
        }
        public static readonly DependencyProperty IsParallelProperty =
            DependencyProperty.Register("IsParallel", typeof(bool), typeof(NavMenuToggleButton), new UIPropertyMetadata(true, OnIsParallelChanged));

        #endregion

        #region callbacks

        private static void OnIsParallelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavMenuToggleButton button)
            {
                if (button.IsParallel)
                    button.OnParalleled();
                else
                    button.OnCrossed();
            }
        }

        #endregion

        #region routed events

        /// <summary>
        /// Событие, вызываемое при начале приведения прямоугольников кнопки в крестообразное положение.
        /// </summary>
        public static readonly RoutedEvent CrossedEvent = EventManager.RegisterRoutedEvent(
            "Crossed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavMenuToggleButton));
        public event RoutedEventHandler Crossed
        {
            add { AddHandler(CrossedEvent, value); }
            remove { RemoveHandler(CrossedEvent, value); }
        }
        public void OnCrossed()
        {
            RaiseEvent(new RoutedEventArgs(NavMenuToggleButton.CrossedEvent));
        }

        /// <summary>
        /// Событие, вызываемое при начале приведения прямоугольников кнопки в параллельное положение.
        /// </summary>
        public static readonly RoutedEvent ParalleledEvent = EventManager.RegisterRoutedEvent(
            "Paralleled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavMenuToggleButton));
        public event RoutedEventHandler Paralleled
        {
            add { AddHandler(ParalleledEvent, value); }
            remove { RemoveHandler(ParalleledEvent, value); }
        }
        public void OnParalleled()
        {
            RaiseEvent(new RoutedEventArgs(NavMenuToggleButton.ParalleledEvent));
        }

        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent(
             "Clicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavMenuToggleButton));
        public event RoutedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }
        public void OnClicked()
        {
            RaiseEvent(new RoutedEventArgs(NavMenuToggleButton.ClickedEvent));
        }

        #endregion
    }
}
