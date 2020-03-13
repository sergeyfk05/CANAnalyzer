using HamburgerMenu.Calcs;
using HamburgerMenu.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace HamburgerMenu
{
    /// <summary>
    /// Interaction logic for NavMenu.xaml
    /// </summary>
    public partial class NavMenu : UserControl
    {
        public NavMenu()
        {
            InitializeComponent();
            Root.Loaded += (object sender, RoutedEventArgs e) => { MinCorrectWidth = Root.CalcMinNavMenuCorrectWidth(); };
            Draw(Root);
        }


        private void Draw(StackPanel root)
        {
            if (ItemSource == null)
                return;

            foreach (var el in ItemSource)
            {
                CreateNavMenuItem(el, root);
            }
            
        }

        private void CreateNavMenuItem(NavMenuItemData item, Panel toAdd, int offset = 0)
        {
            #region создание самого элемента

            DockPanel result = new DockPanel()
            {
                Height = ItemHeight,
                Background = new SolidColorBrush(item.IsSelected ? SelectedItemBackground : Background)
            };
        

            Image icon = new Image()
            {
                Height = IconSize,
                Width = IconSize,
                Margin = new Thickness((DropdownIconSectionWidth - IconSize) / 2 + offset, (ItemHeight - IconSize) / 2, (DropdownIconSectionWidth - IconSize) / 2, (ItemHeight - IconSize) / 2),
                Source = new BitmapImage(item.ImageSource)
            };
            DockPanel.SetDock(icon, Dock.Left);
            result.Children.Add(icon);


            TextBlock text = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = item.Text,
                Foreground = item.IsSelected ? new SolidColorBrush(SelectedItemTextColor) : new SolidColorBrush(ItemTextColor),
                FontFamily = ItemTextFontFamily,
                FontSize = ItemTextFontSize,
                FontWeight = ItemTextFontWeight,
                Margin = new Thickness(0, 0, (ItemHeight - ItemTextFontSize) / 2, 0)
            };
            DockPanel.SetDock(text, Dock.Left);
            result.Children.Add(text);



            ColorAnimation mouseEnterAnimation = new ColorAnimation()
            {
                From = item.IsSelected ? SelectedItemBackground : Background,
                To = MouseInItemBackground,
                Duration = MouseInOverAnimationDuration
            };
            ColorAnimation mouseEnterTextAnimation = new ColorAnimation()
            {
                From = item.IsSelected ? SelectedItemTextColor : ItemTextColor,
                To = MouseInItemTextColor,
                Duration = MouseInOverAnimationDuration
            };
            result.MouseEnter += (object sender, MouseEventArgs e) => 
            {
                if (!this.IsEnabled)
                    return;

                result.Background.BeginAnimation(SolidColorBrush.ColorProperty, mouseEnterAnimation);
                text.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, mouseEnterTextAnimation);
            };

            ColorAnimation mouseLeaveAnimation = new ColorAnimation()
            {
                From = MouseInItemBackground,
                To = item.IsSelected ? SelectedItemBackground : Background,
                Duration = MouseInOverAnimationDuration
            };
            ColorAnimation mouseLeaveTextAnimation = new ColorAnimation()
            {
                To = item.IsSelected ? SelectedItemTextColor : ItemTextColor,
                From = MouseInItemTextColor,
                Duration = MouseInOverAnimationDuration
            };
            result.MouseLeave += (object sender, MouseEventArgs e) => 
            {
                if (!this.IsEnabled)
                    return;

                result.Background.BeginAnimation(SolidColorBrush.ColorProperty, mouseLeaveAnimation);
                text.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, mouseLeaveTextAnimation);
            };

            MouseClickManager clickManager = new MouseClickManager(200, () => { return this.IsEnabled; });
            result.MouseLeftButtonDown += clickManager.OnMouseLeftButtonDown;
            result.MouseLeftButtonUp += clickManager.OnMouseLeftButtonUp;

            //обработчик кликов во вне
            clickManager.Click += (object sender, MouseButtonEventArgs e) => { RaiseClickedEvent(item); };


            toAdd.Children.Add(result);

            #endregion

            #region создание подменю

            bool isAnimatedNow = false;
            if (item.IsDropdownItem && item.DropdownItems != null)
            {
                Image dropdownIcon = new Image()
                {
                    Height = DropdownIconSize,
                    Width = DropdownIconSize,
                    Margin = 
                    new Thickness((ItemHeight - DropdownIconSize) / 2 > DropdownIconMinLeftOffset ? (ItemHeight - DropdownIconSize) / 2 : DropdownIconMinLeftOffset, 
                    (ItemHeight - DropdownIconSize) / 2, 
                    (ItemHeight - DropdownIconSize) / 2, 
                    (ItemHeight - DropdownIconSize) / 2),

                    HorizontalAlignment = HorizontalAlignment.Right,
                    Source = new BitmapImage(DropdownIconSource),
                    RenderTransform = new RotateTransform(0, DropdownIconSize / 2, DropdownIconSize / 2),
                    Name = "dropdownIcon"
                };
                DockPanel.SetDock(dropdownIcon, Dock.Right);
                result.Children.Add(dropdownIcon);

                DoubleAnimation rotateAnimation = new DoubleAnimation()
                {
                    Duration = DropdownMenuAnimationDuration,
                    EasingFunction = DropdownMenuFunction,
                };
                
                StackPanel dropdownMenu = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    MaxHeight = 0
                };



                foreach (var el in item.DropdownItems)
                {
                    CreateNavMenuItem(el, dropdownMenu, offset + DropdownMenuLeftOffset);
                }

                
                DoubleAnimation dropupAnimation = new DoubleAnimation()
                {
                    Duration = DropdownMenuAnimationDuration,
                    EasingFunction = DropdownMenuFunction
                };

                DoubleAnimationUsingKeyFrames dropdownAnimation = new DoubleAnimationUsingKeyFrames();
                dropdownAnimation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = 0,  EasingFunction = DropdownMenuFunction});
                dropdownAnimation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(DropdownMenuAnimationDuration.Milliseconds - 1)), EasingFunction = DropdownMenuFunction });
                dropdownAnimation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(DropdownMenuAnimationDuration), Value = 100000 });

                this.IsEnabledChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
                {
                    if (!(bool)e.NewValue && dropdownMenu.MaxHeight > 0)
                    {
                        dropupAnimation.From = dropdownMenu.RenderSize.Height;
                        dropupAnimation.To = 0;
                        dropdownMenu.BeginAnimation(Panel.MaxHeightProperty, dropupAnimation);
                    }
                };


                #region обработчик кликов для скрытия/раскрытия меню

                rotateAnimation.Completed += (object sender, EventArgs e) => { isAnimatedNow = false; };

                clickManager.Click += (object sender, MouseButtonEventArgs e) =>
                {

                    if (!isAnimatedNow && (sender is Panel senderPanel))
                    {
                        if (senderPanel.Parent is Panel parentPanel)
                        {
                            var submenu = parentPanel.Children[parentPanel.Children.IndexOf(senderPanel) + 1];

                            if (submenu is Panel submenuPanel)
                            {
                                isAnimatedNow = true;
                                if (submenuPanel.MaxHeight == 0)
                                {
                                    rotateAnimation.From = 0;
                                    rotateAnimation.To = 180;
                                    dropdownAnimation.KeyFrames[1].Value = submenuPanel.RenderSize.Height;
                                    submenuPanel.BeginAnimation(Panel.MaxHeightProperty, dropdownAnimation);
                                }
                                else
                                {
                                    rotateAnimation.From = 180;
                                    rotateAnimation.To = 360;
                                    dropupAnimation.From = submenuPanel.RenderSize.Height;
                                    dropupAnimation.To = 0;
                                    submenuPanel.BeginAnimation(Panel.MaxHeightProperty, dropupAnimation);
                                }

                                
                                if((LogicalTreeHelper.FindLogicalNode(senderPanel, "dropdownIcon") is Image img) && (img.RenderTransform is RotateTransform transform))
                                {
                                   transform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
                                }
                            }
                        }
                    }

                };
                #endregion

                toAdd.Children.Add(dropdownMenu);
            }

            #endregion

        }

        public void ReDraw()
        {
            Root.Children.Clear();
            Draw(Root);
        }


        public static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked", RoutingStrategy.Bubble, typeof(ClickedEventHandler), typeof(NavMenu));               
        public event ClickedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }
        private void RaiseClickedEvent(NavMenuItemData item)
        {
            RaiseEvent(new ClickedEventArgs(ClickedEvent, item));
        }



        public List<NavMenuItemData> ItemSource
        {
            get { return (List<NavMenuItemData>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(List<NavMenuItemData>), typeof(NavMenu), new UIPropertyMetadata(null, OnItemSourceChanged));

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavMenu nm)
            {
                nm.ReDraw();
            }
        }


        private static void OnRedraw(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavMenu nm)
            {
                nm.ReDraw();
            }
        }

        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(int), typeof(NavMenu), new UIPropertyMetadata(30, OnRedraw));



        public int IconSize
        {
            get { return (int)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(NavMenu), new UIPropertyMetadata(20, OnRedraw));



        public Color Background
        {
            get { return (Color)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(0, 255, 0), OnRedraw));



        public Color MouseInItemBackground
        {
            get { return (Color)GetValue(MouseInItemBackgroundProperty); }
            set { SetValue(MouseInItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MouseInItemBackgroundProperty =
            DependencyProperty.Register("MouseInItemBackground", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(155, 155, 155), OnRedraw));



        public Duration MouseInOverAnimationDuration
        {
            get { return (Duration)GetValue(MouseInOverAnimationDurationProperty); }
            set { SetValue(MouseInOverAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty MouseInOverAnimationDurationProperty =
            DependencyProperty.Register("MouseInOverAnimationDuration", typeof(Duration), typeof(NavMenu), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(150))));



        public TimeSpan DropdownMenuAnimationDuration
        {
            get { return (TimeSpan)GetValue(DropdownMenuAnimationDurationProperty); }
            set { SetValue(DropdownMenuAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty DropdownMenuAnimationDurationProperty =
            DependencyProperty.Register("DropdownMenuAnimationDuration", typeof(TimeSpan), typeof(NavMenu), new PropertyMetadata(TimeSpan.FromMilliseconds(350)));



        public Color SelectedItemBackground
        {
            get { return (Color)GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register("SelectedItemBackground", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(100, 100, 100), OnRedraw));




        public int DropdownMenuLeftOffset
        {
            get { return (int)GetValue(DropdownMenuLeftOffsetProperty); }
            set { SetValue(DropdownMenuLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownMenuLeftOffsetProperty =
            DependencyProperty.Register("DropdownMenuLeftOffset", typeof(int), typeof(NavMenu), new UIPropertyMetadata(20, OnRedraw));



        public IEasingFunction DropdownMenuFunction
        {
            get { return (IEasingFunction)GetValue(DropdownMenuFunctionProperty); }
            set { SetValue(DropdownMenuFunctionProperty, value); }
        }
        public static readonly DependencyProperty DropdownMenuFunctionProperty =
            DependencyProperty.Register("DropdownMenuFunction", typeof(IEasingFunction), typeof(NavMenu), new UIPropertyMetadata(new CircleEase(), OnRedraw));




        public int DropdownIconSize
        {
            get { return (int)GetValue(DropdownIconSizeProperty); }
            set { SetValue(DropdownIconSizeProperty, value); }
        }   
        public static readonly DependencyProperty DropdownIconSizeProperty =
            DependencyProperty.Register("DropdownIconSize", typeof(int), typeof(NavMenu), new UIPropertyMetadata(10, OnRedraw));



        public Uri DropdownIconSource
        {
            get { return (Uri)GetValue(DropdownIconSourceProperty); }
            set { SetValue(DropdownIconSourceProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSourceProperty =
            DependencyProperty.Register("DropdownIconSource", typeof(Uri), typeof(NavMenu), new UIPropertyMetadata(new Uri(new Uri(Assembly.GetExecutingAssembly().Location), "1.png"), OnRedraw));




        public Color ItemTextColor
        {
            get { return (Color)GetValue(ItemTextColorProperty); }
            set { SetValue(ItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty ItemTextColorProperty =
            DependencyProperty.Register("ItemTextColor", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(0, 0, 0), OnRedraw));



        public Color MouseInItemTextColor
        {
            get { return (Color)GetValue(MouseInItemTextColorProperty); }
            set { SetValue(MouseInItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty MouseInItemTextColorProperty =
            DependencyProperty.Register("MouseInItemTextColor", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(0, 255, 0), OnRedraw));




        public Color SelectedItemTextColor
        {
            get { return (Color)GetValue(SelectedItemTextColorProperty); }
            set { SetValue(SelectedItemTextColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemTextColorProperty =
            DependencyProperty.Register("SelectedItemTextColor", typeof(Color), typeof(NavMenu), new UIPropertyMetadata(Color.FromRgb(0, 0, 255), OnRedraw));




        public FontFamily ItemTextFontFamily
        {
            get { return (FontFamily)GetValue(ItemTextFontFamilyProperty); }
            set { SetValue(ItemTextFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty ItemTextFontFamilyProperty =
            DependencyProperty.Register("ItemTextFontFamily", typeof(FontFamily), typeof(NavMenu), new UIPropertyMetadata(new FontFamily("Arial"), OnRedraw));




        public FontWeight ItemTextFontWeight
        {
            get { return (FontWeight)GetValue(ItemTextFontWeightProperty); }
            set { SetValue(ItemTextFontWeightProperty, value); }
        }
        public static readonly DependencyProperty ItemTextFontWeightProperty =
            DependencyProperty.Register("ItemTextFontWeight", typeof(FontWeight), typeof(NavMenu), new UIPropertyMetadata(FontWeights.Bold, OnRedraw));



        public double ItemTextFontSize
        {
            get { return (double)GetValue(ItemTextFontSizeProperty); }
            set { SetValue(ItemTextFontSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemTextFontSizeProperty =
            DependencyProperty.Register("ItemTextFontSize", typeof(double), typeof(NavMenu), new UIPropertyMetadata(12.0, OnRedraw));





        public double MinCorrectWidth
        {
            get { return (double)GetValue(MinCorrectWidthProperty); }
            set { SetValue(MinCorrectWidthProperty, value); }
        }
        public static readonly DependencyProperty MinCorrectWidthProperty =
            DependencyProperty.Register("MinCorrectWidth", typeof(double), typeof(NavMenu), new PropertyMetadata(0.0));


        public double DropdownIconSectionWidth
        {
            get { return (double)GetValue(DropdownIconSectionWidthProperty); }
            set { SetValue(DropdownIconSectionWidthProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSectionWidthProperty =
            DependencyProperty.Register("DropdownIconSectionWidth", typeof(double), typeof(NavMenu), new UIPropertyMetadata(100.0, OnRedraw));



        public double DropdownIconMinLeftOffset
        {
            get { return (double)GetValue(DropdownIconMinLeftOffsetProperty); }
            set { SetValue(DropdownIconMinLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconMinLeftOffsetProperty =
            DependencyProperty.Register("DropdownIconMinLeftOffset", typeof(double), typeof(NavMenu), new UIPropertyMetadata(30.0, OnRedraw));



    }
}
