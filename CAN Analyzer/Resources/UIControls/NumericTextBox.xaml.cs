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

namespace CANAnalyzer.Resources.UIControls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : UserControl
    {
        public NumericTextBox()
        {
            InitializeComponent();
        }




        public string RealText
        {
            get { return (string)GetValue(RealTextProperty); }
            set { SetValue(RealTextProperty, value); }
        }
        public static readonly DependencyProperty RealTextProperty =
            DependencyProperty.Register("RealText", typeof(string), typeof(NumericTextBox), new PropertyMetadata(""));



        public uint Value
        {
            get { return (uint)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(uint), typeof(NumericTextBox), new UIPropertyMetadata((uint)0, OnValueChanged));

        private bool HandlingValueChangedCallback = true;
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericTextBox control)
            {
                if (control.HandlingValueChangedCallback)
                {
                    control.RealText = control.Value.ToString();
                }

                control.HandlingValueChangedCallback = true;
            }
        }


        public uint MaxValue
        {
            get { return (uint)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(uint), typeof(NumericTextBox), new UIPropertyMetadata((uint)0));

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericTextBox control)
            {
                if(control.Value > control.MaxValue)
                {
                    control.HandlingValueChangedCallback = true;
                    control.Value = control.MaxValue;
                }
            }
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var el in e.Text)
            {
                if (!(el >= '0' && el <= '9'))
                    e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox tb)
            {
                if(String.IsNullOrEmpty(tb.Text))
                {
                    HandlingValueChangedCallback = false;
                    Value = 0;
                    return;
                }

                UInt32 newValue;
                if (UInt32.TryParse(tb.Text, out newValue))
                {
                    HandlingValueChangedCallback = false;

                    if (newValue > MaxValue)
                    {
                        HandlingValueChangedCallback = true;
                        newValue = MaxValue;
                        tb.Text = newValue.ToString();
                        tb.CaretIndex = tb.Text.Length;
                    }

                    Value = newValue;
                }
                else
                {
                    HandlingValueChangedCallback = true;
                    Value = MaxValue;
                }
            }
        }
    }
}
