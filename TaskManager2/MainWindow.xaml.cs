using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TaskManager2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        int currentCount = 0;
        int presentCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            currentCount = Process.GetProcesses().Length;

            if (currentCount != presentCount)
            {
                presentCount = currentCount;    
                UpdateCurrentProcess();
            }
        }

        private void ShowProcess()
        {
            foreach (Process process in Process.GetProcesses())
            {
                TasksListBox.Items.Add($"{process.ProcessName} (ID: {process.Id})");
            }
        }

        private void UpdateCurrentProcess()
        {
            TasksListBox.Items.Clear();
            ShowProcess();
        }

        private void KillButton_Click(object sender, RoutedEventArgs e)
        {
            if(TasksListBox.SelectedItems.Count > 0)
            {
                foreach (string selectedItem in TasksListBox.SelectedItems)
                {
                    string processIdString = selectedItem.Substring(selectedItem.IndexOf("(ID: ") + 5);
                    processIdString = processIdString.Substring(0, processIdString.Length - 1); 

                    if (int.TryParse(processIdString, out int processId))
                    {
                        Process process = Process.GetProcessById(processId);
                        process.Kill();
                    }
                    else
                    {
                        MessageBox.Show($"Invalid process ID: {processIdString}");
                    }
                }
            }
        }

        
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FindTextBox.Text))
            {
                string searchText = FindTextBox.Text.ToLower(); 
                bool itemFound = false;

                foreach (string item in TasksListBox.Items)
                {
                    if (item.ToLower().Contains(searchText)) 
                    {
                         
                        TasksListBox.SelectedItem = item;
                        TasksListBox.ScrollIntoView(item);
                        itemFound = true;
                        break; 
                    }
                }

                if (!itemFound)
                {
                    MessageBox.Show("Item not found!");
                }
            }
            else
            {
                MessageBox.Show("Type the name of the process to find it!");
            }
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartTextBox.Text != "")
            {
                ProcessStartInfo process = new ProcessStartInfo(StartTextBox.Text);
                Process.Start(process);
            }
            else
            {
                MessageBox.Show("Type the name or filepath to start process!");
            }
        }
    }
}
