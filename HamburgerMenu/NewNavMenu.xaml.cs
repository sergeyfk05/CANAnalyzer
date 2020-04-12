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
    /// Interaction logic for NewNavMenu.xaml
    /// </summary>
    public partial class NewNavMenu : UserControl
    {
        public NewNavMenu()
        {
            InitializeComponent();
        }

        //mincorrectwidth
        //click event



        public ObservableCollection<NavMenuItemData> Items
        {
            get { return (ObservableCollection<NavMenuItemData>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<NavMenuItemData>), typeof(NewNavMenu), new PropertyMetadata(null));



        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(NewNavMenu), new PropertyMetadata(30.0));



        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(NewNavMenu), new PropertyMetadata(20.0));



        public double IconSectionWidth
        {
            get { return (double)GetValue(DropdownIconSectionWidthProperty); }
            set { SetValue(DropdownIconSectionWidthProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSectionWidthProperty =
            DependencyProperty.Register("IconSectionWidth", typeof(double), typeof(NewNavMenu), new PropertyMetadata(100.0));


        public double DropdownIncrementOffset
        {
            get { return (double)GetValue(DropdownIncrementOffsetProperty); }
            set { SetValue(DropdownIncrementOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownIncrementOffsetProperty =
            DependencyProperty.Register("DropdownIncrementOffset", typeof(double), typeof(NewNavMenu), new PropertyMetadata(20.0));




        public double DropdownMenuOffset
        {
            get { return (double)GetValue(DropdownMenuOffsetProperty); }
            set { SetValue(DropdownMenuOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownMenuOffsetProperty =
            DependencyProperty.Register("DropdownMenuOffset", typeof(double), typeof(NewNavMenu), new PropertyMetadata(0.0));




        public ImageSource DropdownIconSource
        {
            get { return (ImageSource)GetValue(DropdownIconSourceProperty); }
            set { SetValue(DropdownIconSourceProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSourceProperty =
            DependencyProperty.Register("DropdownIconSource", typeof(ImageSource), typeof(NewNavMenu), new PropertyMetadata(null));


        public double DropdownIconSize
        {
            get { return (double)GetValue(DropdownIconSizeProperty); }
            set { SetValue(DropdownIconSizeProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconSizeProperty =
            DependencyProperty.Register("DropdownIconSize", typeof(double), typeof(NewNavMenu), new PropertyMetadata(10.0));

        public double DropdownIconMinLeftOffset
        {
            get { return (double)GetValue(DropdownIconMinLeftOffsetProperty); }
            set { SetValue(DropdownIconMinLeftOffsetProperty, value); }
        }
        public static readonly DependencyProperty DropdownIconMinLeftOffsetProperty =
            DependencyProperty.Register("DropdownIconMinLeftOffset", typeof(double), typeof(NewNavMenu), new PropertyMetadata(30.0));


        public FontFamily TextFontFamily
        {
            get { return (FontFamily)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TextFontFamilyProperty =
            DependencyProperty.Register("TextFontFamily", typeof(FontFamily), typeof(NewNavMenu), new PropertyMetadata(new FontFamily("Arial")));




        public FontWeight TextFontWeight
        {
            get { return (FontWeight)GetValue(TextFontWeightProperty); }
            set { SetValue(TextFontWeightProperty, value); }
        }
        public static readonly DependencyProperty TextFontWeightProperty =
            DependencyProperty.Register("TextFontWeight", typeof(FontWeight), typeof(NewNavMenu), new PropertyMetadata(FontWeights.Bold));



        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(NewNavMenu), new PropertyMetadata(12.0));

        public Brush ShowedTextBrush
        {
            get { return (Brush)GetValue(ShowedTextBrushProperty); }
            private set { SetValue(ShowedTextBrushProperty, value); }
        }
        public static readonly DependencyProperty ShowedTextBrushProperty =
            DependencyProperty.Register("ShowedTextBrush", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(null));



        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }
        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255))));



        public Brush SelectedTextBrush
        {
            get { return (Brush)GetValue(SelectedTextBrushProperty); }
            set { SetValue(SelectedTextBrushProperty, value); }
        }
        public static readonly DependencyProperty SelectedTextBrushProperty =
            DependencyProperty.Register("SelectedTextBrush", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255))));



        public Brush TextMouseEnterBrush
        {
            get { return (Brush)GetValue(TextMouseEnterBrushProperty); }
            set { SetValue(TextMouseEnterBrushProperty, value); }
        }
        public static readonly DependencyProperty TextMouseEnterBrushProperty =
            DependencyProperty.Register("TextMouseEnterBrush", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));

        public Duration MouseEnterOverAnimationDuration
        {
            get { return (Duration)GetValue(MouseEnterOverAnimationDurationProperty); }
            set { SetValue(MouseEnterOverAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty MouseEnterOverAnimationDurationProperty =
            DependencyProperty.Register("MouseEnterOverAnimationDuration", typeof(Duration), typeof(NewNavMenu), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500))));

        public Brush SelectedItemBackground
        {
            get { return (Brush)GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 0, 0))));



        public Brush ItemBackground
        {
            get { return (Brush)GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 255, 0))));



        public Brush ItemMouseEnterBackground
        {
            get { return (Brush)GetValue(ItemMouseEnterBackgroundProperty); }
            set { SetValue(ItemMouseEnterBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ItemMouseEnterBackgroundProperty =
            DependencyProperty.Register("ItemMouseEnterBackground", typeof(Brush), typeof(NewNavMenu), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 255))));




        public double MinCorrectWidth
        {
            get { return (double)GetValue(MinCorrectWidthProperty); }
            set { SetValue(MinCorrectWidthProperty, value); }
        }
        public static readonly DependencyProperty MinCorrectWidthProperty =
            DependencyProperty.Register("MinCorrectWidth", typeof(double), typeof(NewNavMenu), new PropertyMetadata(500.0));

        public Duration DropdownAnimationDuration
        {
            get { return (Duration)GetValue(DropdownAnimationDurationProperty); }
            set { SetValue(DropdownAnimationDurationProperty, value); }
        }
        public static readonly DependencyProperty DropdownAnimationDurationProperty =
            DependencyProperty.Register("DropdownAnimationDuration", typeof(Duration), typeof(NewNavMenu), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500))));



        public IEasingFunction DropdownAnimationFunction
        {
            get { return (IEasingFunction)GetValue(DropdownAnimationFunctionProperty); }
            set { SetValue(DropdownAnimationFunctionProperty, value); }
        }
        public static readonly DependencyProperty DropdownAnimationFunctionProperty =
            DependencyProperty.Register("DropdownAnimationFunction", typeof(IEasingFunction), typeof(NewNavMenu), new PropertyMetadata(new CircleEase()));

    }
}
