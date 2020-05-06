/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace CANAnalyzer.Resources.UIControls
{
    /// <summary>
    /// Interaction logic for HexTextBox.xaml
    /// </summary>
    public partial class HexTextBox : UserControl
    {
        public HexTextBox()
        {
            InitializeComponent();
        }





        public string RealText
        {
            get { return (string)GetValue(RealTextProperty); }
            set { SetValue(RealTextProperty, value); }
        }
        public static readonly DependencyProperty RealTextProperty =
            DependencyProperty.Register("RealText", typeof(string), typeof(HexTextBox), new PropertyMetadata(""));




        public UInt64 NullStrValue
        {
            get { return (UInt64)GetValue(NullStrValueProperty); }
            set { SetValue(NullStrValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NullStrValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NullStrValueProperty =
            DependencyProperty.Register("NullStrValue", typeof(UInt64), typeof(HexTextBox), new PropertyMetadata((UInt64)0));





        public UInt64 MaxValue
        {
            get { return (UInt64)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(UInt64), typeof(HexTextBox), new UIPropertyMetadata((UInt64)0, OnMaxValueChanged));

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is HexTextBox control)
            {
                if ((control.MaxValue != 0) && (control.Value > control.MaxValue))
                {
                    control.HandlingOnValueChanged = true;
                    control.Value = control.MaxValue;
                }
                    
            }
        }

        public UInt64 Value
        {
            get { return (UInt64)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(UInt64), typeof(HexTextBox), new UIPropertyMetadata((UInt64)0, OnValueChanged));

        private bool HandlingOnValueChanged = true;
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexTextBox control)
            {
                if(control.HandlingOnValueChanged)
                {
                    control.RealText = control.Value.ToString("X");
                }

                control.HandlingOnValueChanged = true;
            }
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach(var el in e.Text)
            {
                if (!((el >= '0' && el <= '9') || (el >= 'a' && el <= 'f') || (el >= 'A' && el <= 'F')))
                    e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox tb)
            {
                tb.Text = tb.Text.TrimStart('0');
                try
                {
                    UInt64 newValue = String.IsNullOrEmpty(tb.Text) ? 0 : Convert.ToUInt64(tb.Text, 16);
                    if ((MaxValue != 0) && (newValue > MaxValue))
                    {
                        HandlingOnValueChanged = false;
                        this.Value = MaxValue;
                        tb.Text = MaxValue.ToString("X");
                        tb.CaretIndex = tb.Text.Length;
                        SystemSounds.Beep.Play();
                    }
                    else
                    {
                        HandlingOnValueChanged = false;
                        this.Value = newValue;
                        int pos = tb.CaretIndex;
                        tb.Text = tb.Text.ToUpper();
                        tb.CaretIndex = pos;
                    }
                }
                catch
                {
                    HandlingOnValueChanged = false;
                    this.Value = MaxValue;
                    tb.Text = MaxValue.ToString("X");
                    tb.CaretIndex = tb.Text.Length;
                    SystemSounds.Beep.Play();
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    HandlingOnValueChanged = false;
                    Value = NullStrValue;
                    tb.Text = NullStrValue.ToString("X");
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }
    }
}
