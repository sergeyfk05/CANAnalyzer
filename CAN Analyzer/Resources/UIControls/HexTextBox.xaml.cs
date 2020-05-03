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
            private set { SetValue(RealTextProperty, value); }
        }
        public static readonly DependencyProperty RealTextProperty =
            DependencyProperty.Register("RealText", typeof(string), typeof(HexTextBox), new PropertyMetadata("0x00 00 00 00 00 00 00 00"));




        public byte[] Data
        {
            get { return (byte[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(byte[]), typeof(HexTextBox), new UIPropertyMetadata(new byte[8], OnDataChanged));

        private bool OnDataChangedHandlingIsEnabled = false;
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexTextBox control)
            {
                if (control.OnDataChangedHandlingIsEnabled)
                    control.RealText = RenderText(control.Data);

                control.OnDataChangedHandlingIsEnabled = true;
            }
        }



        public UInt64 MaxValue
        {
            get { return (UInt64)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(UInt64), typeof(HexTextBox), new PropertyMetadata((UInt64)0));







        private static string RenderText(byte[] data)
        {
            string result = "0x";
            foreach (var b in data)
            {
                result += b.ToString("X2");
                result += " ";
            }

            return result.Trim();
        }

        private static byte[] StringToBytes(string str)
        {
            if (str.Substring(0, 2) != "0x")
                throw new ArgumentException("invalid string");

            str = str.Remove(0, 2);
            string[] bytes = str.Split(' ');
            byte[] result = new byte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i].Length != 2)
                    throw new ArgumentException("invalid string");

                if (!Byte.TryParse(bytes[i], System.Globalization.NumberStyles.HexNumber, null, out result[i]))
                    throw new ArgumentException("invalid string");
            }

            return result;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string prev = tb.Text;
                int pos = tb.CaretIndex;
                string newStr = tb.Text;
                bool isOk = true;

                foreach (char el in e.Text)
                {
                    //проверка выхода курсора за границы
                    if (pos >= tb.Text.Length)
                    {
                        isOk = false;
                        break;
                    }

                    //если курсор перед пробелом
                    if ((pos >= 4) && ((pos - 4) % 3 == 0))
                    {
                        if (el != ' ')
                        {
                            isOk = false;
                            break;
                        }
                    }
                    else
                    {
                        //если курсор перед цифрой
                        char symbol = Char.ToUpper(el, new System.Globalization.CultureInfo("en"));
                        if (((symbol >= '0') && (symbol <= '9')) || ((symbol >= 'A') && (symbol <= 'F')))
                        {
                            newStr = newStr.Remove(pos, 1);
                            newStr = newStr.Insert(pos, symbol.ToString());
                        }
                        else
                        {
                            isOk = false;
                            break;
                        }
                    }

                    pos++;
                }


                if (isOk)
                {
                    byte[] newData = StringToBytes(newStr);

                    //convert bytes to uint
                    UInt64 value = BytesToUInt(newData);

                    if (MaxValue == 0 || value <= MaxValue)
                    {
                        OnDataChangedHandlingIsEnabled = false;
                        Data = StringToBytes(newStr);

                        tb.Text = newStr;
                        tb.CaretIndex = pos;
                    }
                    else
                    {
                        SystemSounds.Beep.Play();
                    }
                }
                else
                {
                    SystemSounds.Beep.Play();
                }
            }



            if (e.RoutedEvent != null)
                e.Handled = true;
        }

        private static UInt64 BytesToUInt(byte[] source)
        {
            UInt64 value = 0;
            for (int i = 0; i < source.Length; i++)
            {
                value |= (UInt64)source[i] << (8 * (source.Length - i - 1));
            }

            return value;
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                //disable opportunity to select text
                if (tb.SelectionLength > 0)
                    tb.SelectionLength = 0;

                //check the caret position for the smallest logically possible
                if (tb.CaretIndex < 2)
                    tb.CaretIndex = 2;

                //check the caret position for the largest logically possible
                if (tb.CaretIndex > tb.Text.Length - 1)
                    tb.CaretIndex = tb.Text.Length - 1;


                //check the caret position for logical trust position and fix it
                if ((tb.CaretIndex >= 4) && (tb.CaretIndex - 4) % 3 == 0)
                {
                    if (tb.CaretIndex >= tb.Text.Length - 1)
                    {
                        tb.CaretIndex -= 1;
                    }
                    else
                    {
                        tb.CaretIndex += 1;
                    }
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e.Key == Key.Left)
                {
                    if ((tb.CaretIndex >= 4) && ((tb.CaretIndex - 5) % 3 == 0))
                    {
                        tb.CaretIndex -= 2;
                        e.Handled = true;
                    }
                }

                if (e.Key == Key.Right)
                {
                    if (tb.CaretIndex % 3 == 0)
                    {
                        tb.CaretIndex += 2;
                        e.Handled = true;
                    }
                }

                if ((e.Key == Key.Delete) || (e.Key == Key.Back))
                    e.Handled = true;

                //ctrl+V handler
                if (Key.V == e.Key && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    TextBox_PreviewTextInput(sender, new TextCompositionEventArgs(null, new TextComposition(null, null, Clipboard.GetText())));
                    e.Handled = true;
                }
            }
        }
    }
}
