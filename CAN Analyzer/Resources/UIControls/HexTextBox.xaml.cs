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



        public UInt64 MaxValue
        {
            get { return (UInt64)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(UInt64), typeof(HexTextBox), new PropertyMetadata((UInt64)0));




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
                if((MaxValue != 0) && (Convert.ToUInt64(tb.Text, 16) > MaxValue))
                {
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
