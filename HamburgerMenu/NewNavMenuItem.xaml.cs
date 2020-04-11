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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HamburgerMenu
{
    /// <summary>
    /// Interaction logic for NewNavMenuItem.xaml
    /// </summary>
    public partial class NewNavMenuItem : UserControl
    {
        public NewNavMenuItem()
        {
            InitializeComponent();

            this.Loaded += NewNavMenuItem_Loaded;
        }

        private void Manager_Click(object sender, MouseButtonEventArgs e)
        {
            IsDropdowned = !IsDropdowned;
        }

        private void NewNavMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template.FindName("dock", this) is DockPanel d)
                dockPanel = d;


            if (this.Template.FindName("text", this) is TextBlock t)
                textBlock = t;

            if (this.Template.FindName("dropdownIcon", this) is Image i)
                dropdownIcon = i;

            if (this.Template.FindName("dropdownMenu", this) is ItemsControl ic)
                dropdownMenu = ic;


            MouseClickManager manager = new MouseClickManager(200);
            dockPanel.MouseLeftButtonDown += manager.OnMouseLeftButtonDown;
            dockPanel.MouseLeftButtonUp += manager.OnMouseLeftButtonUp;
            manager.Click += Manager_Click;

            OnOffsetsChanged(this, new DependencyPropertyChangedEventArgs());
            ResetColors();
            GenerateBackgroundAnimations();
            GenerateTextAnimations();
            GenerateDropdownAnimations();

            dockPanel.MouseEnter += (object s, MouseEventArgs a) => { mouseEnterBackgroundStoryboard.Begin(); };
            dockPanel.MouseLeave += (object s, MouseEventArgs a) => { mouseLeaveBackgroundStoryboard.Begin(); };

            dockPanel.MouseEnter += (object s, MouseEventArgs a) => { mouseEnterTextStoryboard.Begin(); };
            dockPanel.MouseLeave += (object s, MouseEventArgs a) => { mouseLeaveTextStoryboard.Begin(); };
        }

        public DockPanel dockPanel;
        public TextBlock textBlock;
        public Image dropdownIcon;
        public ItemsControl dropdownMenu;


        public void GenerateBackgroundAnimations()
        {
            mouseEnterBackgroundStoryboard.Children.Clear();

            BrushAnimation mouseEnterBackgroundAnimation = new BrushAnimation
            {
                From = IsSelected ? SelectedItemBackground : ItemBackground,
                To = ItemMouseEnterBackground,
                Duration = MouseEnterOverAnimationDuration
            };
            Storyboard.SetTarget(mouseEnterBackgroundAnimation, dockPanel);
            Storyboard.SetTargetProperty(mouseEnterBackgroundAnimation, new PropertyPath("Background"));

            mouseEnterBackgroundStoryboard.Children.Add(mouseEnterBackgroundAnimation);


            mouseLeaveBackgroundStoryboard.Children.Clear();

            BrushAnimation mouseLeaveBackgroundAnimation = new BrushAnimation
            {
                From = ItemMouseEnterBackground,
                To = IsSelected ? SelectedItemBackground : ItemBackground,
                Duration = MouseEnterOverAnimationDuration
            };
            Storyboard.SetTarget(mouseLeaveBackgroundAnimation, dockPanel);
            Storyboard.SetTargetProperty(mouseLeaveBackgroundAnimation, new PropertyPath("Background"));

            mouseLeaveBackgroundStoryboard.Children.Add(mouseLeaveBackgroundAnimation);
        }
        private Storyboard mouseEnterBackgroundStoryboard = new Storyboard();
        private Storyboard mouseLeaveBackgroundStoryboard = new Storyboard();


        private void GenerateTextAnimations()
        {
            mouseEnterTextStoryboard.Children.Clear();

            BrushAnimation mouseEnterTextAnimation = new BrushAnimation
            {
                From = IsSelected ? SelectedTextBrush : TextBrush,
                To = TextMouseEnterBrush,
                Duration = MouseEnterOverAnimationDuration
            };
            Storyboard.SetTarget(mouseEnterTextAnimation, textBlock);
            Storyboard.SetTargetProperty(mouseEnterTextAnimation, new PropertyPath("Foreground"));

            mouseEnterTextStoryboard.Children.Add(mouseEnterTextAnimation);


            mouseLeaveTextStoryboard.Children.Clear();

            BrushAnimation mouseLeaveTextAnimation = new BrushAnimation
            {
                From = TextMouseEnterBrush,
                To = IsSelected ? SelectedTextBrush : TextBrush,
                Duration = MouseEnterOverAnimationDuration
            };
            Storyboard.SetTarget(mouseLeaveTextAnimation, textBlock);
            Storyboard.SetTargetProperty(mouseLeaveTextAnimation, new PropertyPath("Foreground"));

            mouseLeaveTextStoryboard.Children.Add(mouseLeaveTextAnimation);
        }
        private Storyboard mouseEnterTextStoryboard = new Storyboard();
        private Storyboard mouseLeaveTextStoryboard = new Storyboard();


        private void GenerateDropdownAnimations()
        {
            DropdownIconDropdownedStoryboard.Children.Clear();

            DoubleAnimation dropdownIconAnimation = new DoubleAnimation()
            {
                Duration = DropdownAnimationDuration,
                EasingFunction = DropdownAnimationFunction,
                From = 0,
                To = 180
            };
            Storyboard.SetTarget(dropdownIconAnimation, dropdownIcon);
            Storyboard.SetTargetProperty(dropdownIconAnimation, new PropertyPath("(Image.RenderTransform).(RotateTransform.Angle)"));

            DropdownIconDropdownedStoryboard.Children.Add(dropdownIconAnimation);


            DropdownIconCollapsedStoryboard.Children.Clear();

            DoubleAnimation collapseIconAnimation = new DoubleAnimation()
            {
                Duration = DropdownAnimationDuration,
                EasingFunction = DropdownAnimationFunction,
                From = 180,
                To = 360
            };
            Storyboard.SetTarget(collapseIconAnimation, dropdownIcon);
            Storyboard.SetTargetProperty(collapseIconAnimation, new PropertyPath("(Image.RenderTransform).(RotateTransform.Angle)"));

            DropdownIconCollapsedStoryboard.Children.Add(collapseIconAnimation);


        }
        public Storyboard DropdownIconDropdownedStoryboard = new Storyboard();
        public Storyboard DropdownIconCollapsedStoryboard = new Storyboard();


        public void ResetColors()
        {
            //don't work
            ShowedItemBackground = IsSelected ? SelectedItemBackground : ItemBackground;
            ShowedTextBrush = IsSelected ? SelectedTextBrush : TextBrush;
        }


        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(30.0, OnItemHeightChanged));



        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(20.0, OnIconPropertiesChanged));



        public double IconSectionWidth
        {
            get { return (double)GetValue(DropdownIconSectionWidthProperty); }
            set { SetValue(DropdownIconSectionWidthProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSectionWidthProperty =
            DependencyProperty.Register("IconSectionWidth", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(100.0, OnIconPropertiesChanged));



        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(NewNavMenuItem), new PropertyMetadata(null));


        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(NewNavMenuItem), new PropertyMetadata(new Thickness()));



        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(0.0, OnOffsetsChanged));



        public double DropdownIncrementOffset
        {
            get { return (double)GetValue(DropdownIncrementOffsetProperty); }
            set { SetValue(DropdownIncrementOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownIncrementOffsetProperty =
            DependencyProperty.Register("DropdownIncrementOffset", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(20.0, OnOffsetsChanged));




        public double DropdownMenuOffset
        {
            get { return (double)GetValue(DropdownMenuOffsetProperty); }
            set { SetValue(DropdownMenuOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropdownMenuOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropdownMenuOffsetProperty =
            DependencyProperty.Register("DropdownMenuOffset", typeof(double), typeof(NewNavMenuItem), new PropertyMetadata(0.0));




        public ImageSource DropdownIconSource
        {
            get { return (ImageSource)GetValue(DropdownIconSourceProperty); }
            set { SetValue(DropdownIconSourceProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSourceProperty =
            DependencyProperty.Register("DropdownIconSource", typeof(ImageSource), typeof(NewNavMenuItem), new PropertyMetadata(null));


        public double DropdownIconSize
        {
            get { return (double)GetValue(DropdownIconSizeProperty); }
            set { SetValue(DropdownIconSizeProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSizeProperty =
            DependencyProperty.Register("DropdownIconSize", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(10.0, OnDropdownIconSizesChanged));




        public Thickness DropdownIconMargin
        {
            get { return (Thickness)GetValue(DropdownIconMarginProperty); }
            set { SetValue(DropdownIconMarginProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconMarginProperty =
            DependencyProperty.Register("DropdownIconMargin", typeof(Thickness), typeof(NewNavMenuItem), new PropertyMetadata(new Thickness()));

        public double DropdownIconMinLeftOffset
        {
            get { return (double)GetValue(DropdownIconMinLeftOffsetProperty); }
            set { SetValue(DropdownIconMinLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconMinLeftOffsetProperty =
            DependencyProperty.Register("DropdownIconMinLeftOffset", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(30.0, OnDropdownIconSizesChanged));






        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NewNavMenuItem), new PropertyMetadata(""));




        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(NewNavMenuItem), new PropertyMetadata(new Thickness()));

        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TextFontFamilyProperty =
            DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(NewNavMenuItem), new PropertyMetadata(new FontFamily("Arial")));




        public FontWeight TextFontWeight
        {
            get { return (FontWeight)GetValue(TextFontWeightProperty); }
            set { SetValue(TextFontWeightProperty, value); }
        }
        public static readonly DependencyProperty TextFontWeightProperty =
            DependencyProperty.Register("TextFontWeight", typeof(FontWeight), typeof(NewNavMenuItem), new PropertyMetadata(FontWeights.Bold));



        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(NewNavMenuItem), new UIPropertyMetadata(12.0, OnTextPropertiesChanged));




        public Brush ShowedTextBrush
        {
            get { return (Brush)GetValue(ShowedTextBrushProperty); }
            private set { SetValue(ShowedTextBrushProperty, value); }
        }
        public static readonly DependencyProperty ShowedTextBrushProperty =
            DependencyProperty.Register("ShowedTextBrush", typeof(Brush), typeof(NewNavMenuItem), new PropertyMetadata(null));



        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }
        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(NewNavMenuItem), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255)), OnTextBrushesChanged));



        public Brush SelectedTextBrush
        {
            get { return (Brush)GetValue(SelectedTextBrushProperty); }
            set { SetValue(SelectedTextBrushProperty, value); }
        }
        public static readonly DependencyProperty SelectedTextBrushProperty =
            DependencyProperty.Register("SelectedTextBrush", typeof(Brush), typeof(NewNavMenuItem), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255)), OnTextBrushesChanged));



        public Brush TextMouseEnterBrush
        {
            get { return (Brush)GetValue(TextMouseEnterBrushProperty); }
            set { SetValue(TextMouseEnterBrushProperty, value); }
        }
        public static readonly DependencyProperty TextMouseEnterBrushProperty =
            DependencyProperty.Register("TextMouseEnterBrush", typeof(Brush), typeof(NewNavMenuItem), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0)), OnTextBrushesChanged));


        public Duration MouseEnterOverAnimationDuration
        {
            get { return (Duration)GetValue(MouseEnterOverAnimationDurationProperty); }
            set { SetValue(MouseEnterOverAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty MouseEnterOverAnimationDurationProperty =
            DependencyProperty.Register("MouseEnterOverAnimationDuration", typeof(Duration), typeof(NewNavMenuItem), new UIPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500)), OnMouseEnterOverAnimationDataChanged));





        public Brush ShowedItemBackground
        {
            get { return (Brush)GetValue(ShowedItemBackgroundProperty); }
            set { SetValue(ShowedItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ShowedItemBackgroundProperty =
            DependencyProperty.Register("ShowedItemBackground", typeof(Brush), typeof(NewNavMenuItem), new PropertyMetadata(null));


        public Brush SelectedItemBackground
        {
            get { return (Brush)GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(NewNavMenuItem), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0)), OnItemBackgroundsChanged));



        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(NewNavMenuItem), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 255, 0)), OnItemBackgroundsChanged));



        public Brush ItemMouseEnterBackground
        {
            get { return (Brush)GetValue(ItemMouseEnterBackgroundProperty); }
            set { SetValue(ItemMouseEnterBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemMouseEnterBackgroundProperty =
            DependencyProperty.Register("ItemMouseEnterBackground", typeof(Brush), typeof(NewNavMenuItem), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255)), OnItemBackgroundsChanged));




        public double MinCorrectWidth
        {
            get { return (double)GetValue(MinCorrectWidthProperty); }
            set { SetValue(MinCorrectWidthProperty, value); }
        }
        public static readonly DependencyProperty MinCorrectWidthProperty =
            DependencyProperty.Register("MinCorrectWidth", typeof(double), typeof(NewNavMenuItem), new PropertyMetadata(500.0));




        public bool IsDropdownable
        {
            get { return (bool)GetValue(IsDropdownableProperty); }
            set { SetValue(IsDropdownableProperty, value); }
        }
        public static readonly DependencyProperty IsDropdownableProperty =
            DependencyProperty.Register("IsDropdownable", typeof(bool), typeof(NewNavMenuItem), new PropertyMetadata(false));



        public bool IsDropdowned
        {
            get { return (bool)GetValue(IsDropdownedProperty); }
            set { SetValue(IsDropdownedProperty, value); }
        }
        public static readonly DependencyProperty IsDropdownedProperty =
            DependencyProperty.Register("IsDropdowned", typeof(bool), typeof(NewNavMenuItem), new UIPropertyMetadata(false, OnIsDropdownedChanged));




        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NewNavMenuItem), new UIPropertyMetadata(false, OnIsSelectedChanged));




        public Duration DropdownAnimationDuration
        {
            get { return (Duration)GetValue(DropdownAnimationDurationProperty); }
            set { SetValue(DropdownAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty DropdownAnimationDurationProperty =
            DependencyProperty.Register("DropdownAnimationDuration", typeof(Duration), typeof(NewNavMenuItem), new UIPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500)), OnDropdownAnimationDataChanged));



        public IEasingFunction DropdownAnimationFunction
        {
            get { return (IEasingFunction)GetValue(DropdownAnimationFunctionProperty); }
            set { SetValue(DropdownAnimationFunctionProperty, value); }
        }
        public static readonly DependencyProperty DropdownAnimationFunctionProperty =
            DependencyProperty.Register("DropdownAnimationFunction", typeof(IEasingFunction), typeof(NewNavMenuItem), new PropertyMetadata(new CircleEase()));




        public ObservableCollection<NavMenuItemData> DropdownItems
        {
            get { return (ObservableCollection<NavMenuItemData>)GetValue(DropdownItemsProperty); }
            set { SetValue(DropdownItemsProperty, value); }
        }
        public static readonly DependencyProperty DropdownItemsProperty =
            DependencyProperty.Register("DropdownItems", typeof(ObservableCollection<NavMenuItemData>), typeof(NewNavMenuItem), new PropertyMetadata(null));







        public static void OnIsDropdownedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {   
                if (nmi.IsDropdowned)
                {
                    double dropdownMenuHeight = 0;
                    for (int i = 0; i < nmi.dropdownMenu.Items.Count; i++)
                    {
                        dropdownMenuHeight += ((UIElement)nmi.dropdownMenu.ItemContainerGenerator.ContainerFromIndex(i)).RenderSize.Height;
                    }


                    DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
                    animation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = 0, EasingFunction = nmi.DropdownAnimationFunction });
                    animation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(nmi.DropdownAnimationDuration.TimeSpan.Milliseconds - 1)), Value = dropdownMenuHeight, EasingFunction = nmi.DropdownAnimationFunction });
                    animation.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(nmi.DropdownAnimationDuration.TimeSpan), Value = 100000 });

                    nmi.dropdownMenu.BeginAnimation(Control.MaxHeightProperty, animation);
                    nmi.DropdownIconDropdownedStoryboard.Begin();


                }
                else
                {
                    DoubleAnimation animation = new DoubleAnimation()
                    {
                        EasingFunction = nmi.DropdownAnimationFunction,
                        Duration = nmi.DropdownAnimationDuration,
                        From = nmi.dropdownMenu.RenderSize.Height,
                        To = 0
                    };

                    nmi.dropdownMenu.BeginAnimation(Control.MaxHeightProperty, animation);
                    nmi.DropdownIconCollapsedStoryboard.Begin();
                }


            }
        }
        public static void OnMouseEnterOverAnimationDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.GenerateBackgroundAnimations();
                nmi.GenerateTextAnimations();
            }
        }
        public static void OnDropdownAnimationDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.GenerateDropdownAnimations();
            }
        }
        public static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.ResetColors();
                nmi.GenerateBackgroundAnimations();
                nmi.GenerateTextAnimations();
            }
        }
        public static void OnDropdownIconSizesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.DropdownIconMargin = new Thickness((nmi.ItemHeight - nmi.DropdownIconSize) / 2.0 > nmi.DropdownIconMinLeftOffset ? (nmi.ItemHeight - nmi.DropdownIconSize) / 2.0 : nmi.DropdownIconMinLeftOffset,
                     (nmi.ItemHeight - nmi.DropdownIconSize) / 2,
                     (nmi.ItemHeight - nmi.DropdownIconSize) / 2,
                     (nmi.ItemHeight - nmi.DropdownIconSize) / 2);
            }
        }
        public static void OnTextBrushesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.GenerateTextAnimations();
            }
        }
        public static void OnItemBackgroundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.ResetColors();
                nmi.GenerateBackgroundAnimations();
            }
        }
        public static void OnOffsetsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.DropdownMenuOffset = nmi.Offset + nmi.DropdownIncrementOffset;
                OnIconPropertiesChanged(d, e);
            }
        }
        public static void OnIconPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.IconMargin = new Thickness((nmi.IconSectionWidth - nmi.IconSize) / 2 + nmi.Offset, (nmi.ItemHeight - nmi.IconSize) / 2, (nmi.IconSectionWidth - nmi.IconSize) / 2, (nmi.ItemHeight - nmi.IconSize) / 2);
            }
        }
        public static void OnTextPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                nmi.TextMargin = new Thickness(0, 0, (nmi.ItemHeight - nmi.TextFontSize) / 2, 0);
            }
        }
        public static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NewNavMenuItem nmi)
            {
                OnIconPropertiesChanged(d, e);
                OnTextPropertiesChanged(d, e);
                OnDropdownIconSizesChanged(d, e);
            }
        }

    }
}
