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



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(HexTextBox), new PropertyMetadata(""));




        public UInt64 MaxValue
        {
            get { return (UInt64)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(UInt64), typeof(HexTextBox), new PropertyMetadata((UInt64)0));



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
                    control.Text = control.MaxValue.ToString("X");
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
                UInt64 newValue = Convert.ToUInt64(tb.Text, 16);
                if ((MaxValue != 0) && (newValue > MaxValue))
                {
                    HandlingOnValueChanged = false;
                    this.Value = newValue;
                    tb.Text = MaxValue.ToString("X");
                    tb.CaretIndex = tb.Text.Length;
                    SystemSounds.Beep.Play();
                }
                else
                {
                    int pos = tb.CaretIndex;
                    tb.Text = tb.Text.ToUpper();
                    tb.CaretIndex = pos;
                }
            }
        }
    }
}
