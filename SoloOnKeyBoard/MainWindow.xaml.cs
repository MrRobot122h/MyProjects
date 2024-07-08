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
using System.Windows.Threading;

namespace SoloOnKeyBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tempTimer = 0;
        DispatcherTimer timer = null;
        bool start = false;
        int nextToPaint = 0;


        int failsCount = 0;

        string AllSymbols = "q w e r t y u i o p a s d f g h j k l z x c v b n m Q W E R T Y U I O P A S D F G H J K L Z X C V B N M 1 2 3 4 5 6 7 8 9 0 - = [ \\ ] ; ' , . / ! @ # $ % ^ & * ( ) _ + { } | : \" < > ?";
        string @string1;


        bool capsLockState;
        bool shiftState = false;
        int EasyLevelRandomSymbols = 2;
        int MediumLevelRandomSymbols = 4;
        int HardLevelRandomSymbols = 8;

        int EasyLevelAmount = 25;
        int MediumLevelAmount = 50;
        int HardLevelAmount = 75;

        KeyConverter keyConverter;
        Test1 test1form = null;
        char letter;
        char number;
        public MainWindow()
        {
            InitializeComponent();
            this.KeyUp += MainWindow_KeyUp;
            this.PreviewKeyDown += MainWindow_KeyDown;
            this.MouseMove += MainWindow_MouseMove;
            HideKeyBoardCheckBox1.Click += HideKeyBoard;
            keyConverter = new KeyConverter();
            StartButton1.IsEnabled = false;
            StopButton1.IsEnabled = false;
            StartButton1.Click += StartButton1_Click;
            StopButton1.Click += StopButton1_Click;
            DifficultyComboBox1.SelectionChanged += DifficultyComboBox1_SelectionChanged;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);

            RandomSym1.TextChanged += RandomSym1_TextChanged;
            WriteLine1.TextChanged += WriteLine1_TextChanged;

            GoToTestButton1.Click += GoToTestButton1_Click;
        }

        private void GoToTestButton1_Click(object sender, RoutedEventArgs e)
        {
            test1form = new Test1();
            this.Hide();
            test1form.ShowDialog();
            this.Close();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (start)
            {
                capsLockState = Console.CapsLock;
                var BoardKey = e.Key;
                switch (BoardKey)
                {
                    case Key.CapsLock:
                        if (capsLockState) { CapsLock_ON(); ButtonsPaint(); }
                        if (!capsLockState) { CapsLock_OFF(); ButtonsPaint(); }
                        break;

                    case Key.Space:
                        if (shiftState) { ALLShiftEvent_ON(); }
                        WriteLine1.Text += " ";
                        break;

                    case Key.Tab:
                        if (shiftState) { ALLShiftEvent_ON(); }
                        WriteLine1.Text += "\t";
                        break;

                    case Key.Enter:
                        if (shiftState) { ALLShiftEvent_ON(); }
                        WriteLine1.Text += "\n";
                        break;

                    case Key.Back:
                        if (shiftState) { ALLShiftEvent_ON(); }
                        //Back();
                        break;

                    case Key.LeftShift:
                    case Key.RightShift:
                        shiftState = true;
                        ALLShiftEvent_ON();
                        ButtonsPaintBack();
                        ButtonsPaint();

                        break;
                    case Key.OemTilde: WriteSymbols(shiftState ? '~' : '`'); break;
                    case Key.D1: WriteSymbols('!'); break;
                    case Key.D2: WriteSymbols('@'); break;
                    case Key.D3: WriteSymbols('#'); break;
                    case Key.D4: WriteSymbols('$'); break;
                    case Key.D5: WriteSymbols('%'); break;
                    case Key.D6: WriteSymbols('^'); break;
                    case Key.D7: WriteSymbols('&'); break;
                    case Key.D8: WriteSymbols('*'); break;
                    case Key.D9: WriteSymbols('('); break;
                    case Key.D0: WriteSymbols(')'); break;
                    case Key.OemMinus: WriteSymbols(shiftState ? '_' : '-'); break;
                    case Key.OemPlus: WriteSymbols(shiftState ? '+' : '='); break;
                    case Key.OemOpenBrackets: WriteSymbols(shiftState ? '{' : '['); break;
                    case Key.OemCloseBrackets: WriteSymbols(shiftState ? '}' : ']'); break;
                    case Key.Oem5: WriteSymbols(shiftState ? '|' : '\\'); break;
                    case Key.OemSemicolon: WriteSymbols(shiftState ? ':' : ';'); break;
                    case Key.OemQuotes: WriteSymbols(shiftState ? '"' : '\''); break;
                    case Key.OemComma: WriteSymbols(shiftState ? '<' : ','); break;
                    case Key.OemPeriod: WriteSymbols(shiftState ? '>' : '.'); break;
                    case Key.OemQuestion: WriteSymbols(shiftState ? '?' : '/'); break;

                }

                if (char.TryParse(e.Key.ToString(), out letter) && char.IsLetter(letter))
                {
                    if (shiftState)
                    {
                        ALLShiftEvent_ON();
                        WriteLetters(shiftState, letter);
                    }
                    WriteLetters(capsLockState, letter);
                }
                if (char.TryParse(keyConverter.ConvertToString(e.Key), out number) && char.IsDigit(number))
                {
                    if (shiftState) { ALLShiftEvent_ON(); }
                    WriteNumbers(shiftState, number);
                }

                if (RandomSym1.Text == WriteLine1.Text)
                {
                    nextToPaint = 0;
                    MessageBox.Show($"Training is over!\nNumber of characters: {RandomSym1.Text.Length}\nNumber of fails: {failsCount}");
                    StopButton1_Click(sender, e);
                }
            }
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                shiftState = false;
                ALLShiftEvent_OFF();
                ButtonsPaintBack();
                ButtonsPaint();
            }

        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
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
        private void WriteLine1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RandomSym1.Text != null && WriteLine1.Text != null)
            {
                for (int i = 0; i < WriteLine1.Text.Length; i++)
                {
                    if (RandomSym1.Text[i] != WriteLine1.Text[i])
                    {
                        failsCount++;
                        if (nextToPaint > 0)
                        {
                            nextToPaint--;
                        }
                        Back();
                        break;
                    }
                    else if (i == WriteLine1.Text.Length - 1)
                    {
                        if (RandomSym1.Text[i] == WriteLine1.Text[i])
                        {
                            ButtonsPaintBack();
                            nextToPaint++;

                            if (nextToPaint < RandomSym1.Text.Length)
                            { ButtonsPaint(); }
                            break;
                        }
                    }
                }
                FailCount1.Content = $"Fails: {failsCount}";
            }
        }

        private void RandomSym1_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonsPaint();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tempTimer++;
            Speed();
        }
        void Speed()
        {
            CharsSpeed1.Content = $"Speed: {Math.Round(((double)WriteLine1.Text.Length / tempTimer) * 60).ToString()} chars/min";
        }
        private void DifficultyComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DifficultyComboBox1.SelectedItem != null)
            {
                StartButton1.IsEnabled = true;
            }
        }

        private void ButtonsPaintBack()
        {
            foreach (Button button in GetAllButtons())
            {
                if (button.Background != Brushes.LightGray)
                    button.Background = Brushes.LightGray;
            }
        }
        private void ButtonsPaint()
        {
            if (nextToPaint < RandomSym1.Text.Length && isLetter(RandomSym1.Text[nextToPaint].ToString()))
            {
                foreach (Button button in GetAllButtons())
                {
                    if (button.Content.ToString() == RandomSym1.Text[nextToPaint].ToString())
                    {
                        button.Background = Brushes.Green;
                        break;
                    }
                }
            }
            else if (nextToPaint < RandomSym1.Text.Length && isDigit(RandomSym1.Text[nextToPaint].ToString()))
            {
                foreach (Button button in GetAllButtons())
                {
                    if (button.Content.ToString() == RandomSym1.Text[nextToPaint].ToString())
                    {
                        button.Background = Brushes.Green;
                        break;
                    }
                }
            }
            else if (nextToPaint < RandomSym1.Text.Length)
            {
                foreach (Button button in GetAllButtons())
                {
                    if ((nextToPaint < RandomSym1.Text.Length && RandomSym1.Text[nextToPaint] == ' ' && button.Content.ToString() == "Space"))
                    {
                        button.Background = Brushes.Green;
                        break;
                    }
                    else
                    {
                        if (button.Content.ToString() == RandomSym1.Text[nextToPaint].ToString())
                        {
                            button.Background = Brushes.Green;
                            break;
                        }
                    }
                }
            }
        }
        private void StopButton1_Click(object sender, RoutedEventArgs e)
        {
            StartButton1.IsEnabled = false;
            StopButton1.IsEnabled = false;
            WriteLine1.Text = string.Empty;
            RandomSym1.Text = string.Empty;
            DifficultyComboBox1.IsEnabled = true;
            DifficultyComboBox1.SelectedItem = null;
            FailCount1.Content = "Fails: 0";
            failsCount = 0;
            start = true;
            nextToPaint = 0;
            ButtonsPaintBack();
        }

        private void StartButton1_Click(object sender, RoutedEventArgs e)
        {
            if (DifficultyComboBox1.Text == "Easy")
            {
                MakeTraining(EasyLevelRandomSymbols, EasyLevelAmount);
            }
            if (DifficultyComboBox1.Text == "Medium")
            {
                MakeTraining(MediumLevelRandomSymbols, MediumLevelAmount);
            }
            if (DifficultyComboBox1.Text == "Hard")
            {
                RandomSym1.FontSize = 18;
                WriteLine1.FontSize = 18;
                MakeTraining(HardLevelRandomSymbols, HardLevelAmount);
            }
            StartButton1.IsEnabled = false;
            StopButton1.IsEnabled = true;
            DifficultyComboBox1.IsEnabled = false;
            tempTimer = 0;
            timer.Start();
            start = true;

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
            WriteLine1.Focus();
        }

        private void HideButtons(bool shouldHide)
        {
            foreach (var button in GetAllButtons())
            {
                button.Visibility = shouldHide ? Visibility.Collapsed : Visibility.Visible;
            }
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

        private void WriteNumbers(bool @bool, char @char)
        {
            if (@bool)
            {
                foreach (Button button in GetAllButtons())
                {
                    if (char.TryParse(button.Content.ToString(), out char @char1) && @char1 == @char)
                    {
                        WriteLine1.Text += button.Content.ToString();
                        break;

                    }
                }
            }
            else
            {
                foreach (Button button in GetAllButtons())
                {
                    if (char.TryParse(button.Content.ToString(), out char @char1) && @char1 == @char)
                    {
                        WriteLine1.Text += button.Content.ToString();
                        break;

                    }
                }
            }
        }
        private void WriteSymbols(char @char)
        {
            foreach (Button button in GetAllButtons())
            {
                if (char.TryParse(button.Content.ToString(), out char @char1) && @char1 == @char)
                {
                    WriteLine1.Text += button.Content.ToString();
                    break;

                }
            }
        }

        private void WriteLetters(bool @bool, char @char)
        {
            if (@bool)
            {
                foreach (Button button in GetAllButtons())
                {
                    if (char.TryParse(button.Content.ToString(), out char @char1) && @char1 == @char)
                    {
                        WriteLine1.Text += button.Content.ToString();
                        break;

                    }
                }
            }
            else
            {
                foreach (Button button in GetAllButtons())
                {
                    if (char.TryParse(button.Content.ToString(), out char @char1) && char.ToLower(@char) == @char1)
                    {
                        WriteLine1.Text += button.Content.ToString();
                        break;
                    }
                }
            }
        }

        private void ALLShiftEvent_ON()
        {
            ShiftEvent_ON();
            ShiftEvent_ON1();
            ButtonsPaintBack();
            ButtonsPaint();
        }
        private void ALLShiftEvent_OFF()
        {
            ShiftEvent_OFF();
            ShiftEvent_OFF1();
            ButtonsPaintBack();
            ButtonsPaint();
        }
        private void ShiftEvent_ON()
        {
            CapsLock_ON();
        }

        private void ShiftEvent_OFF()
        {
            CapsLock_OFF();
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
        private void Back()
        {
            if (WriteLine1.Text.Length > 0)
                WriteLine1.Text = WriteLine1.Text.ToString().Substring(0, WriteLine1.Text.Length - 1);
        }

        private void MakeTraining(int @int, int @int1)
        {
            List<string> RandomElements = new List<string>();
            RandomElements = AllSymbols.Split(' ').ToList();
            @string1 = string.Empty;
            RandomSym1.Text = string.Empty;
            Random random = new Random();
            for (int i = 0; i < @int; i++)
            {
                int randomIndex = random.Next(RandomElements.Count);
                string1 += RandomElements[randomIndex] + ' ';
            }
            RandomElements = new List<string>();
            RandomElements = string1.Trim().Split(' ').ToList();
            for (int i = 0; i < @int1; i++)
            {
                int randomIndex = random.Next(RandomElements.Count);
                RandomSym1.Text += RandomElements[randomIndex];
                if (i % 2 == 0 || i % 4 == 0)
                {
                    RandomSym1.Text += " ";
                }
            }
            RandomSym1.Text = RandomSym1.Text.Trim();
        }

        private IEnumerable<Button> GetAllButtons()
        {
            return KeyBoardButtons.Children.OfType<Button>();
        }
    }
}
