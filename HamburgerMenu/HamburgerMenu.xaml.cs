/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using HamburgerMenu.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HamburgerMenu
{
    /// <summary>
    /// Interaction logic for HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        public HamburgerMenu()
        {
            InitializeComponent();
            this.Loaded += OnHamburgerMenu_Loaded;
            
        }

        private void OnHamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is HamburgerMenu hm)
            {
                Width = CollapsedWidth;

                if (hm.Template.FindName("ToggleButton", hm) is NavMenuToggleButton button)
                {
                    button.Clicked += (object senderL, RoutedEventArgs eL) =>
                    {
                        IsCollapsed = !IsCollapsed;
                    };
                }
            }
        }



        public Brush ToggleButtonBackground
        {
            get { return (Brush)GetValue(ToggleButtonBackgroundProperty); }
            set { SetValue(ToggleButtonBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ToggleButtonBackgroundProperty =
            DependencyProperty.Register("ToggleButtonBackground", typeof(Brush), typeof(HamburgerMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));



        public Brush ToggleButtonColor
        {
            get { return (Brush)GetValue(ToggleButtonColorProperty); }
            set { SetValue(ToggleButtonColorProperty, value); }
        }
        public static readonly DependencyProperty ToggleButtonColorProperty =
            DependencyProperty.Register("ToggleButtonColor", typeof(Brush), typeof(HamburgerMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 255, 0))));



        public bool NavMenusIsEnabled
        {
            get { return (bool)GetValue(NavMenusIsEnabledProperty); }
            set { SetValue(NavMenusIsEnabledProperty, value); }
        }
        public static readonly DependencyProperty NavMenusIsEnabledProperty =
            DependencyProperty.Register("NavMenusIsEnabled", typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false));



        public double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }
        public static readonly DependencyProperty CollapsedWidthProperty =
            DependencyProperty.Register("CollapsedWidth", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(50.0));


        public double ToggleButtonBlockHeight
        {
            get { return (double)GetValue(ToggleButtonBlockHeightProperty); }
            set { SetValue(ToggleButtonBlockHeightProperty, value); }
        }
        public static readonly DependencyProperty ToggleButtonBlockHeightProperty =
            DependencyProperty.Register("ToggleButtonBlockHeight", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(50.0));




        public List<NavMenuItemData> TopNavMenuItemSource
        {
            get { return (List<NavMenuItemData>)GetValue(TopNavMenuItemSourceProperty); }
            set { SetValue(TopNavMenuItemSourceProperty, value); }
        }
        public static readonly DependencyProperty TopNavMenuItemSourceProperty =
            DependencyProperty.Register("TopNavMenuItemSource", typeof(List<NavMenuItemData>), typeof(HamburgerMenu), new PropertyMetadata(null));



        public List<NavMenuItemData> BottomNavMenuItemSource
        {
            get { return (List<NavMenuItemData>)GetValue(BottomNavMenuItemSourceProperty); }
            set { SetValue(BottomNavMenuItemSourceProperty, value); }
        }
        public static readonly DependencyProperty BottomNavMenuItemSourceProperty =
            DependencyProperty.Register("BottomNavMenuItemSource", typeof(List<NavMenuItemData>), typeof(HamburgerMenu), new PropertyMetadata(null));




        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(HamburgerMenu), new UIPropertyMetadata(true, OnIsCollapsedChanged));

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HamburgerMenu hm && hm.IsLoaded)
            {
                if (hm.IsCollapsed)
                {
                    hm.Width = hm.CollapsedWidth;
                }
                else
                {
                    
                    double newWidth = hm.CollapsedWidth;
                    if (hm.Template.FindName("TopNavMenu", hm) is NavMenu topNavMenu && newWidth < topNavMenu.UpdateMinCorrectWidth())
                    {
                        newWidth = topNavMenu.MinCorrectWidth;
                    }


                    if (hm.Template.FindName("BottomNavMenu", hm) is NavMenu bottomNavMenu && newWidth < bottomNavMenu.UpdateMinCorrectWidth())
                    {
                        newWidth = bottomNavMenu.MinCorrectWidth;
                    }

                    hm.Width = newWidth;

                }

                hm.NavMenusIsEnabled = !hm.IsCollapsed;
            }
        }


        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(HamburgerMenu), new UIPropertyMetadata(0.0, OnWidthChanged));

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HamburgerMenu sender && sender.IsLoaded)
            {
                if (sender.Template.FindName("Root", sender) is DockPanel panel)
                {
                    DoubleAnimation widthAnimation = new DoubleAnimation()
                    {
                        From = (double)e.OldValue,
                        To = (double)e.NewValue,
                        Duration = sender.CollapseHamburgerAnimationDuration,
                        EasingFunction = sender.AnimationFunction,
                    };

                    panel.BeginAnimation(Panel.WidthProperty, widthAnimation);
                }
            }
        }




        public TimeSpan CollapseHamburgerAnimationDuration
        {
            get { return (TimeSpan)GetValue(CollapseHamburgerAnimationDurationProperty); }
            set { SetValue(CollapseHamburgerAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty CollapseHamburgerAnimationDurationProperty =
            DependencyProperty.Register("CollapseHamburgerAnimationDuration", typeof(TimeSpan), typeof(HamburgerMenu), new PropertyMetadata(TimeSpan.FromMilliseconds(350)));



        public TimeSpan DropdownNavMenuAnimationDuration
        {
            get { return (TimeSpan)GetValue(DropdownNavMenuAnimationDurationProperty); }
            set { SetValue(DropdownNavMenuAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty DropdownNavMenuAnimationDurationProperty =
            DependencyProperty.Register("DropdownNavMenuAnimationDuration", typeof(TimeSpan), typeof(HamburgerMenu), new PropertyMetadata(TimeSpan.FromMilliseconds(350)));



        public Duration NavMenuItemMouseInOverAnimationDuration
        {
            get { return (Duration)GetValue(NavMenuItemMouseInOverAnimationDurationProperty); }
            set { SetValue(NavMenuItemMouseInOverAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty NavMenuItemMouseInOverAnimationDurationProperty =
            DependencyProperty.Register("NavMenuItemMouseInOverAnimationDuration", typeof(Duration), typeof(HamburgerMenu), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));



        public double TopNavMenuItemHeight
        {
            get { return (double)GetValue(TopNavMenuItemHeightProperty); }
            set { SetValue(TopNavMenuItemHeightProperty, value); }
        }
        public static readonly DependencyProperty TopNavMenuItemHeightProperty =
            DependencyProperty.Register("TopNavMenuItemHeight", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(30.0));



        public double BottomNavMenuItemHeight
        {
            get { return (double)GetValue(BottomNavMenuItemHeightProperty); }
            set { SetValue(BottomNavMenuItemHeightProperty, value); }
        }
        public static readonly DependencyProperty BottomNavMenuItemHeightProperty =
            DependencyProperty.Register("BottomNavMenuItemHeight", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(30.0));



        public double TopNavMenuItemIconSize
        {
            get { return (double)GetValue(TopNavMenuItemIconSizeProperty); }
            set { SetValue(TopNavMenuItemIconSizeProperty, value); }
        }
        public static readonly DependencyProperty TopNavMenuItemIconSizeProperty =
            DependencyProperty.Register("TopNavMenuItemIconSize", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(20.0));




        public double BottomNavMenuItemIconSize
        {
            get { return (double)GetValue(BottomNavMenuItemIconSizeProperty); }
            set { SetValue(BottomNavMenuItemIconSizeProperty, value); }
        }
        public static readonly DependencyProperty BottomNavMenuItemIconSizeProperty =
            DependencyProperty.Register("BottomNavMenuItemIconSize", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(20.0));



        public Color NavMenuBackground
        {
            get { return (Color)GetValue(NavMenuBackgroundProperty); }
            set { SetValue(NavMenuBackgroundProperty, value); }
        }
        public static readonly DependencyProperty NavMenuBackgroundProperty =
            DependencyProperty.Register("NavMenuBackground", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(0, 0, 0)));




        public Color NavMenuMouseInItemBackground
        {
            get { return (Color)GetValue(NavMenuMouseInItemBackgroundProperty); }
            set { SetValue(NavMenuMouseInItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty NavMenuMouseInItemBackgroundProperty =
            DependencyProperty.Register("NavMenuMouseInItemBackground", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(125, 0, 0)));



        public Color NavMenuSelectedItemBackground
        {
            get { return (Color)GetValue(NavMenuSelectedItemBackgroundProperty); }
            set { SetValue(NavMenuSelectedItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty NavMenuSelectedItemBackgroundProperty =
            DependencyProperty.Register("NavMenuSelectedItemBackground", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(125, 125, 0)));


        public double NavMenuDropdownIconLeftOffset
        {
            get { return (double)GetValue(NavMenuDropdownIconLeftOffsetProperty); }
            set { SetValue(NavMenuDropdownIconLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty NavMenuDropdownIconLeftOffsetProperty =
            DependencyProperty.Register("NavMenuDropdownIconLeftOffset", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(15.0));



        public double NavMenuDropdownIconSize
        {
            get { return (double)GetValue(NavMenuDropdownIconSizeProperty); }
            set { SetValue(NavMenuDropdownIconSizeProperty, value); }
        }
        public static readonly DependencyProperty NavMenuDropdownIconSizeProperty =
            DependencyProperty.Register("NavMenuDropdownIconSize", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(10.0));




        public Uri NavMenuDropdownIconSource
        {
            get { return (Uri)GetValue(NavMenuDropdownIconSourceProperty); }
            set { SetValue(NavMenuDropdownIconSourceProperty, value); }
        }
        public static readonly DependencyProperty NavMenuDropdownIconSourceProperty =
            DependencyProperty.Register("NavMenuDropdownIconSource", typeof(Uri), typeof(HamburgerMenu), new PropertyMetadata(new Uri(new Uri(Assembly.GetExecutingAssembly().Location), "1.png")));



        public Color NavMenuItemTextColor
        {
            get { return (Color)GetValue(NavMenuItemTextColorProperty); }
            set { SetValue(NavMenuItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty NavMenuItemTextColorProperty =
            DependencyProperty.Register("NavMenuItemTextColor", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(0, 0, 255)));



        public Color NavMenuMouseInItemTextColor
        {
            get { return (Color)GetValue(NavMenuMouseInItemTextColorProperty); }
            set { SetValue(NavMenuMouseInItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty NavMenuMouseInItemTextColorProperty =
            DependencyProperty.Register("NavMenuMouseInItemTextColor", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(0, 0, 255)));



        public Color NavMenuSelectedItemTextColor
        {
            get { return (Color)GetValue(NavMenuSelectedItemTextColorProperty); }
            set { SetValue(NavMenuSelectedItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty NavMenuSelectedItemTextColorProperty =
            DependencyProperty.Register("NavMenuSelectedItemTextColor", typeof(Color), typeof(HamburgerMenu), new PropertyMetadata(Color.FromRgb(0, 0, 255)));




        public FontWeight NavMenuItemTextFontWeight
        {
            get { return (FontWeight)GetValue(NavMenuItemTextFontWeightProperty); }
            set { SetValue(NavMenuItemTextFontWeightProperty, value); }
        }
        public static readonly DependencyProperty NavMenuItemTextFontWeightProperty =
            DependencyProperty.Register("NavMenuItemTextFontWeight", typeof(FontWeight), typeof(HamburgerMenu), new PropertyMetadata(FontWeights.Bold));




        public FontFamily NavMenuItemTextFontFamily
        {
            get { return (FontFamily)GetValue(NavMenuItemTextFontFamilyProperty); }
            set { SetValue(NavMenuItemTextFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty NavMenuItemTextFontFamilyProperty =
            DependencyProperty.Register("NavMenuItemTextFontFamily", typeof(FontFamily), typeof(HamburgerMenu), new PropertyMetadata(new FontFamily("Arial")));




        public double NavMenuItemTextFontSize
        {
            get { return (double)GetValue(NavMenuItemTextFontSizeProperty); }
            set { SetValue(NavMenuItemTextFontSizeProperty, value); }
        }
        public static readonly DependencyProperty NavMenuItemTextFontSizeProperty =
            DependencyProperty.Register("NavMenuItemTextFontSize", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(10.0));




        public double NavMenuDropdownIconMinLeftOffset
        {
            get { return (double)GetValue(NavMenuDropdownIconMinLeftOffsetProperty); }
            set { SetValue(NavMenuDropdownIconMinLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty NavMenuDropdownIconMinLeftOffsetProperty =
            DependencyProperty.Register("NavMenuDropdownIconMinLeftOffset", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(15.0));




        public IEasingFunction AnimationFunction
        {
            get { return (IEasingFunction)GetValue(AnimationFunctionProperty); }
            set { SetValue(AnimationFunctionProperty, value); }
        }
        public static readonly DependencyProperty AnimationFunctionProperty =
            DependencyProperty.Register("AnimationFunction", typeof(IEasingFunction), typeof(HamburgerMenu), new PropertyMetadata(new CircleEase()));



        public NavMenuItemData LastClickedItem
        {
            get { return (NavMenuItemData)GetValue(LastClickedItemProperty); }
            set { SetValue(LastClickedItemProperty, value); }
        }
        public static readonly DependencyProperty LastClickedItemProperty =
            DependencyProperty.Register("LastClickedItem", typeof(NavMenuItemData), typeof(HamburgerMenu), new PropertyMetadata(null));




        private void NavMenu_Click(object sender, ClickedEventArgs e)
        {
            LastClickedItem = e.ClickedItem;
            RaiseClickedEvent(e.ClickedItem);
        }

        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked", RoutingStrategy.Bubble, typeof(ClickedEventHandler), typeof(HamburgerMenu));
        public event ClickedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }
        private void RaiseClickedEvent(NavMenuItemData item)
        {
            RaiseEvent(new ClickedEventArgs(ClickedEvent, item));
        }
    }
}
