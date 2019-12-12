using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DentakuCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string Number;
        private string Buffer;
        private double Result;
        private string Expression;

        public MainWindow()
        {
            InitializeComponent();
            Number = "0";
            Buffer = "0";

            _ = calculate();
            UpdateDisplay();
            UpdateSubDisplay();
        }

        private void ButtonCE_Click(object sender, RoutedEventArgs e)
        {
            Number = "0";
            Buffer = "0";

            UpdateDisplay();
            UpdateSubDisplay();
        }

        private void ButtonC_Click(object sender, RoutedEventArgs e)
        {
            Number = "0";

            UpdateDisplay();
        }

        private void ButtonBackspace_Click(object sender, RoutedEventArgs e)
        {
            var length = Number.Length;

            if (length == 1 || (length == 2 && int.Parse(Number) < 0))
                Number = "0";
            else
                Number = Number.Remove(length - 1, 1);

            UpdateDisplay();
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            if (Buffer == "0")
                Buffer = "";

            if (Number == "" && (Buffer[Buffer.Length - 2] == '+' || Buffer[Buffer.Length - 2] == '-' || Buffer[Buffer.Length - 2] == '*' || Buffer[Buffer.Length - 2] == '/'))
            {
                Buffer = Buffer.Remove(Buffer.Length - 2, 2) + "/ ";
                UpdateSubDisplay();
                return;
            }

            if (Number[Number.Length - 1] == '.')
                Number = Number.Remove(Number.Length - 1, 1);

            Buffer += $"{Number} / ";
            UpdateDisplay();
            Number = "";
            UpdateSubDisplay();
        }

        private void ButtonMulti_Click(object sender, RoutedEventArgs e)
        {
            if (Buffer == "0")
                Buffer = "";

            if (Number == "" && (Buffer[Buffer.Length - 2] == '+' || Buffer[Buffer.Length - 2] == '-' || Buffer[Buffer.Length - 2] == '*' || Buffer[Buffer.Length - 2] == '/'))
            {
                Buffer = Buffer.Remove(Buffer.Length - 2, 2) + "* ";
                UpdateSubDisplay();
                return;
            }

            if (Number[Number.Length - 1] == '.')
                Number = Number.Remove(Number.Length - 1, 1);

            Buffer += $"{Number} * ";
            UpdateDisplay();
            Number = "";
            UpdateSubDisplay();
        }

        private void ButtonSub_Click(object sender, RoutedEventArgs e)
        {
            if (Buffer == "0")
                Buffer = "";

            if (Number == "" && (Buffer[Buffer.Length - 2] == '+' || Buffer[Buffer.Length - 2] == '-' || Buffer[Buffer.Length - 2] == '*' || Buffer[Buffer.Length - 2] == '/'))
            {
                Buffer = Buffer.Remove(Buffer.Length - 2, 2) + "- ";
                UpdateSubDisplay();
                return;
            }

            if (Number[Number.Length - 1] == '.')
                Number = Number.Remove(Number.Length - 1, 1);

            Buffer += $"{Number} - ";
            UpdateDisplay();
            Number = "";
            UpdateSubDisplay();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Buffer == "0")
                Buffer = "";

            if (Number == "" && (Buffer[Buffer.Length - 2] == '+' || Buffer[Buffer.Length - 2] == '-' || Buffer[Buffer.Length - 2] == '*' || Buffer[Buffer.Length - 2] == '/'))
            {
                Buffer = Buffer.Remove(Buffer.Length - 2, 2) + "+ ";
                UpdateSubDisplay();
                return;
            }

            if (Number[Number.Length - 1] == '.')
                Number = Number.Remove(Number.Length - 1, 1);

            Buffer += $"{Number} + ";
            UpdateDisplay();
            Number = "";
            UpdateSubDisplay();
        }

        private void ButtonInvert_Click(object sender, RoutedEventArgs e)
        {
            if (Number == "0")
                return;

            if (Number[0] == '-')
                Number = Number.Remove(0, 1);
            else
                Number = $"-{Number}";

            UpdateDisplay();
            UpdateSubDisplay();
        }

        private void ButtonPoint_Click(object sender, RoutedEventArgs e)
        {
            if (!Number.Contains("."))
                Number += ".";

            UpdateDisplay();
            UpdateSubDisplay();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (Number[Number.Length - 1] == '.')
                Number = Number.Remove(Number.Length - 1, 1);

            if (Buffer == "0")
                Buffer = "";

            Buffer += Number;
            var length = Buffer.Length;

            if (Buffer[length - 1] == ' ')
                Buffer = Buffer.Remove(length - 1, 1);

            length = Buffer.Length;

            if (Buffer[length - 1] == '+' || Buffer[length - 1] == '-' || Buffer[length - 1] == '*' || Buffer[length - 1] == '/')
                if (length != 1)
                    Buffer = Buffer.Remove(length - 2, 2);

            if (Buffer.Contains("/ 0"))
            {
                UpdateSubDisplay();
                Number = "0";
                Buffer = "0";
                Display.Content = "NaN";
                return;
            }

            _ = calculate();
            Number = Result.ToString();
            UpdateSubDisplay();
            Buffer = "0";
            UpdateDisplay();
        }

        private void ButtonNumber_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (Number == "0")
                Number = "";

            Number += button.Content;

            UpdateDisplay();
            UpdateSubDisplay();
        }

        private void UpdateDisplay()
        {
            Display.Content = Number;
        }

        private void UpdateSubDisplay()
        {
            if (Buffer == "0")
                Expression = "";
            else
                Expression = Buffer.Replace("*", "×").Replace("/", "÷");

            SubDisplay.Content = Expression;
        }

        private async Task calculate()
        {
            if (Number == "")
                Number = "0";

            if (Buffer == "")
                Buffer = "0";

            var code = $"double result = (double)({Buffer});";

            try
            {
                var script = CSharpScript.Create(code);
                var state = await script.RunAsync();
                var calcResult = state.GetVariable("result");

                Result = (double)calcResult.Value;
            }
            catch (CompilationErrorException ex)
            {
                MessageBox.Show(ex.Message, "構文エラー");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "不明なエラー");
            }
        }
    }
}
