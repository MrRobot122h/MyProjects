using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace SoloOnKeyBoard
{
    /// <summary>
    /// Interaction logic for Test1.xaml
    /// </summary>
    public partial class Test1 : Window
    {
        MainWindow mainform = null;
        bool start = false;
        private DispatcherTimer timer;
        private int secondsLeft;
        List<string> ListWords = null;
        bool capsLockState;
        bool ShiftState;
        List<string> Symbols;
        int AllSymbolSize = 0;
        int AllCurrentSymbolSize = 0;

        int tempTimer = 0;
        DispatcherTimer timerSpeed = null;


        int currentIndex = 0;

        int FailsCount = 0;
        public Test1()
        {
            InitializeComponent();
            GoToTrainingButton1.Click += GoToTrainingButton1_Click;
            HideKeyBoardCheckBox1.Click += HideKeyBoard;
            this.KeyDown += Test1_KeyDown;
            this.KeyUp += Test1_KeyUp;
            this.MouseMove += Test1_MouseMove;
            InitializeTimer();
            StartButton.Click += StartButton_Click;
            ReStartButton.Click += ReStartButton_Click;
            GetTextButton.Click += GetTextButton_Click;
            StartButton.IsEnabled = false;
            ReStartButton.IsEnabled = false;
            timerSpeed = new DispatcherTimer();
            timerSpeed.Tick += Timer_Tick1;
            timerSpeed.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            Symbols = new List<string>();
            Symbols.AddRange(new string[] { "-", "=", "[", "\\", ",", "]", ";", "'", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "{", "}", "|", ":", "\"", "<", ">", "?" });
        }

        private void Timer_Tick1(object sender, EventArgs e)
        {
            tempTimer++;
            Speed();
        }
        void Speed()
        { 
            CharsSpeed1.Content = $"Speed: {Math.Round(((double)AllCurrentSymbolSize / tempTimer) * 60).ToString()} chars/min";
        }


        private void Test1_MouseMove(object sender, MouseEventArgs e)
        {
            capsLockState = Console.CapsLock;
            if (capsLockState)
            {
                CapsLock_ON();
            }
            else if (!capsLockState)
            {
                CapsLock_OFF();
            }
        }

        private void ReStartButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
        

        private void GetTextButton_Click(object sender, RoutedEventArgs e)
        { 
            string FilePath = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
            if (FilePath != null && FilePath.Contains(".txt"))
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    AllTextBox1.Text = reader.ReadToEnd();
                }
                
                ListWords = AllTextBox1.Text.Trim().Split(new char[] { ' ', '.', ',', '!', '?', ';', ':', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                AllTextBox1.Text = AllTextBox1.Text.Trim();
                AllSymbolSize = AllTextBox1.Text.Length;

                if (ListWords.Count >= 100)
                {
                    StartButton.IsEnabled = true;
                    ReStartButton.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("the text must be at least 100 words or more!");
                    ReStartButton_Click(sender, e);
                }
            }
        }
       

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllTextBox1.Text != "")
            {
                start = true;
                secondsLeft = 60;
                UpdateTimerLabel();
                timer.Start();
                timerSpeed.Start();
                AllTextBox1.Focus();    
            }
        }
        private void UpdateTimerLabel()
        {
            int minutes = secondsLeft / 60;
            int seconds = secondsLeft % 60;
            TimeLabel.Content = $"{minutes:D2}:{seconds:D2}";
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            secondsLeft--;
            if (secondsLeft <= 0)
            {
                timer.Stop();
                MessageBox.Show($"Test is over!\nNumber of characters: {AllCurrentSymbolSize}/{AllSymbolSize}\nNumber of fails: {FailsCount}");
                Clear();
            }
            else
            {
                UpdateTimerLabel();
            }
        }
        private void Clear()
        {
            start = false;
            AllTextBox1.Text = string.Empty;
            StartButton.IsEnabled = false;
            ReStartButton.IsEnabled = false;
            TimeLabel.Content = "Time: 1:00";
            CharsSpeed1.Content = "Speed: 0 chars/min";
            FailCount1.Content = "Fails: 0";
            FailsCount = 0;
            AllCurrentSymbolSize = 0;
            timer.Stop();
            timerSpeed.Stop();
        }
        private void Test1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ShiftState = false;
                ALLShiftEvent_OFF();
            }
            if(e.Key == Key.Space)
            {
                Test1_KeyDown(sender, e);
            }
        }

        private void Test1_KeyDown(object sender, KeyEventArgs e)
        {
            if (start) 
            {
                capsLockState = Console.CapsLock;
                if(e.Key == Key.Tab)
                {
                    AllTextBox1.Focus();
                }
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    ShiftState = true;
                    ALLShiftEvent_ON();
                }
                if (capsLockState)
                {
                    CapsLock_ON();
                }
                if (!capsLockState  && !ShiftState)
                {
                    CapsLock_OFF();
                }
                
                
                if (isDigit(KeySymbolMapper(e)) || isLetter(e.Key.ToString()) || Symbols.Contains(KeySymbolMapper(e)) || e.Key == Key.Space)
                {
                    for (int i = 0; i < AllTextBox1.Text.Length; i++)
                    {
                        if (AllTextBox1.Text[i].ToString() == ConvertKeyToSymbol(e).ToString())
                        {
                            AllCurrentSymbolSize++;
                            AllTextBox1.Text = AllTextBox1.Text.Remove(i, 1);
                            if(AllTextBox1.Text == "")
                            {
                                timer.Stop();
                                MessageBox.Show($"Test is over!\nNumber of characters: {AllCurrentSymbolSize}/{AllSymbolSize}\nNumber of fails: {FailsCount}\n{TimeLabel.Content}");
                                Clear();
                                break;
                            }
                            if (AllTextBox1.Text[i].ToString() == "\r" || AllTextBox1.Text[i].ToString() == "\n")
                            {
                                AllTextBox1.Text = AllTextBox1.Text.Trim();
                            }
                            break;
                        }
                        else
                        {
                            FailCount1.Content = $"Fails: {++FailsCount}";
                            break;
                        }
                    }
                }

               
            }
        }

        private string ConvertKeyToSymbol(KeyEventArgs e)
        {   
            if(isLetter(e.Key.ToString()))
            {
                if(capsLockState || ShiftState)
                {
                    return e.Key.ToString();
                }
                else
                {
                    string upperCaseChar = e.Key.ToString();
                    char lowerCaseChar = (char)(upperCaseChar[0] + ('a' - 'A'));
                    return lowerCaseChar.ToString();
                }
            }
            else
            {
                return KeySymbolMapper(e);
            }

        }

        
        private string KeySymbolMapper(KeyEventArgs e)
        {
            var BoardKey = e.Key;
            switch (BoardKey)
            {
                case Key.Space: return " "; break;
                case Key.OemTilde: return ShiftState ? "~" : "`"; break;
                case Key.D1: return ShiftState ? "!" : "1"; break;
                case Key.D2: return ShiftState ? "@" : "2"; break;
                case Key.D3: return ShiftState ? "#" : "3"; break;
                case Key.D4: return ShiftState ? "$" : "4"; break;
                case Key.D5: return ShiftState ? "%" : "5"; break;
                case Key.D6: return ShiftState ? "^" : "6"; break;
                case Key.D7: return ShiftState ? "&" : "7"; break;
                case Key.D8: return ShiftState ? "*" : "8"; break;
                case Key.D9: return ShiftState ? "(" : "9"; break;
                case Key.D0: return ShiftState ? ")" : "0"; break;
                case Key.OemMinus: return (ShiftState ? "_" : "-"); break;
                case Key.OemPlus: return (ShiftState ? "+" : "="); break;
                case Key.OemOpenBrackets: return (ShiftState ? "{" : "["); break;
                case Key.OemCloseBrackets: return (ShiftState ? "}" : "]"); break;
                case Key.Oem5: return (ShiftState ? "|" : "\\"); break;
                case Key.OemSemicolon: return (ShiftState ? ":" : ";"); break;
                case Key.OemQuotes: return (ShiftState ? "\"" : "'"); break;
                case Key.OemComma: return (ShiftState ? "<" : ","); break;
                case Key.OemPeriod: return (ShiftState ? ">" : "."); break;
                case Key.OemQuestion: return (ShiftState ? "?" : "/"); break;
            }
            return "";
        }
        private bool isLetter(string @string)
        {
            if (char.TryParse(@string, out char @char) && char.IsLetter(@char))
            {
                return true;
            }
            return false;
        }
        private bool isDigit(string @string)
        {
            if (char.TryParse(@string, out char @char) && char.IsDigit(@char))
            {
                return true;
            }
            return false;
        }
        private void ALLShiftEvent_ON()
        {
            ShiftEvent_ON();
            ShiftEvent_ON1();
        }
        private void ALLShiftEvent_OFF()
        {
            ShiftEvent_OFF();
            ShiftEvent_OFF1();
        }
        private void ShiftEvent_ON()
        {
            CapsLock_ON();
        }

        private void ShiftEvent_OFF()
        {
            CapsLock_OFF();
        }
        private void CapsLock_ON()
        {
            foreach (Button button in GetAllButtons())
            {
                if (char.TryParse(button.Content.ToString(), out char @char))
                {
                    if (@char != null && char.IsLetter(@char))
                    {
                        button.Content = char.ToUpper(@char);
                    }
                }
            }
        }
        private void CapsLock_OFF()
        {
            foreach (Button button in GetAllButtons())
            {
                if (char.TryParse(button.Content.ToString(), out char @char))
                {
                    if (@char != null && char.IsLetter(@char))
                    {
                        button.Content = char.ToLower(@char);
                    }
                }
            }
        }

        private void ShiftEvent_ON1()
        {
            foreach (Button button in GetAllButtons())
            {
                if (button.Content.ToString() == "`") button.Content = "~";
                if (button.Content.ToString() == "1") button.Content = "!";
                if (button.Content.ToString() == "2") button.Content = "@";
                if (button.Content.ToString() == "3") button.Content = "#";
                if (button.Content.ToString() == "4") button.Content = "$";
                if (button.Content.ToString() == "5") button.Content = "%";
                if (button.Content.ToString() == "6") button.Content = "^";
                if (button.Content.ToString() == "7") button.Content = "&";
                if (button.Content.ToString() == "8") button.Content = "*";
                if (button.Content.ToString() == "9") button.Content = "(";
                if (button.Content.ToString() == "0") button.Content = ")";
                if (button.Content.ToString() == "-") button.Content = "_";
                if (button.Content.ToString() == "=") button.Content = "+";
                if (button.Content.ToString() == "[") button.Content = "{";
                if (button.Content.ToString() == "]") button.Content = "}";
                if (button.Content.ToString() == "\\") button.Content = "|";
                if (button.Content.ToString() == ";") button.Content = ":";
                if (button.Content.ToString() == "'") button.Content = "\"";
                if (button.Content.ToString() == ",") button.Content = "<";
                if (button.Content.ToString() == ".") button.Content = ">";
                if (button.Content.ToString() == "/") button.Content = "?";
            }
        }

        private void ShiftEvent_OFF1()
        {
            foreach (Button button in GetAllButtons())
            {
                if (button.Content.ToString() == "~") button.Content = "`";
                if (button.Content.ToString() == "!") button.Content = "1";
                if (button.Content.ToString() == "@") button.Content = "2";
                if (button.Content.ToString() == "#") button.Content = "3";
                if (button.Content.ToString() == "$") button.Content = "4";
                if (button.Content.ToString() == "%") button.Content = "5";
                if (button.Content.ToString() == "^") button.Content = "6";
                if (button.Content.ToString() == "&") button.Content = "7";
                if (button.Content.ToString() == "*") button.Content = "8";
                if (button.Content.ToString() == "(") button.Content = "9";
                if (button.Content.ToString() == ")") button.Content = "0";
                if (button.Content.ToString() == "_") button.Content = "-";
                if (button.Content.ToString() == "+") button.Content = "=";
                if (button.Content.ToString() == "{") button.Content = "[";
                if (button.Content.ToString() == "}") button.Content = "]";
                if (button.Content.ToString() == "|") button.Content = "\\";
                if (button.Content.ToString() == ":") button.Content = ";";
                if (button.Content.ToString() == "\"") button.Content = "'";
                if (button.Content.ToString() == "<") button.Content = ",";
                if (button.Content.ToString() == ">") button.Content = ".";
                if (button.Content.ToString() == "?") button.Content = "/";
            }
        }


        private void GoToTrainingButton1_Click(object sender, RoutedEventArgs e)
        {
            mainform = new MainWindow();
            this.Hide();
            mainform.ShowDialog();
            this.Close();
        }
        private void HideKeyBoard(object sender, RoutedEventArgs e)
        {
            if (HideKeyBoardCheckBox1.IsChecked == true)
            {
                HideButtons(true);
            }
            else
            {
                HideButtons(false);
            }
            AllTextBox1.Focus();
        }

        private void HideButtons(bool shouldHide)
        {
            foreach (var button in GetAllButtons())
            {
                button.Visibility = shouldHide ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private IEnumerable<Button> GetAllButtons()
        {
            return KeyBoardButtons.Children.OfType<Button>();
        }


    }
}
