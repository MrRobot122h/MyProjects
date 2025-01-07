using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace TreeSize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Info info = null;
        CurrentInfo currentInfo = null;
        int filesCount = 0;
        int fouldersCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            BuildTree();
        }
        private void BuildTree()
        {
            TreeViewFileSystem.Items.Clear();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = drive;
                item.Header = drive.ToString();

                item.Items.Add("*");
                TreeViewFileSystem.Items.Add(item);
            }
        }
        private void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.Items.Clear();

            DirectoryInfo dir;
            if (item.Tag is DriveInfo)
            {
                DriveInfo drive = (DriveInfo)item.Tag;
                dir = drive.RootDirectory;
            }
            else if (item.Tag is FileInfo)
            {
                dir = null;
            }
            else
            {
                dir = (DirectoryInfo)item.Tag;
            }

            try
            {
                foreach (var file in dir.GetFiles())
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Tag = file;
                    newItem.Header = file.Name;
                    item.Items.Add(newItem);
                }

                foreach (var subDir in dir.GetDirectories())
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Tag = subDir;
                    newItem.Header = subDir.Name;
                    newItem.Items.Add("*");
                    item.Items.Add(newItem);
                }
            }
            catch
            {
            }
        }

        private void TreeViewFileSystem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeViewFileSystem.SelectedItem is TreeViewItem selectedItem && selectedItem != null)
            {

                if (selectedItem.Tag is DirectoryInfo)
                {
                    GetDirectoryInfo(selectedItem);
                }
                else if (selectedItem.Tag is FileInfo)
                {
                    GetFileInfo(selectedItem);
                }
                else
                {
                    GetDirectoryInfo(selectedItem);
                }
            }
        }

        private double GetDirSize(DirectoryInfo obj)
        {
            ConcurrentBag<double> sizes = new ConcurrentBag<double>();
            double sizeByte = 0;

            try
            {
                var files = obj.GetFiles();
                foreach (var file in files)
                {
                    sizeByte += file.Length;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized Access Exception: {ex.Message}");
            }

            sizes.Add(sizeByte);
            try
            {
                var directories = obj.GetDirectories();
                Parallel.ForEach(directories, directory =>
                {
                    double dirSize = GetDirSize(directory);
                    sizes.Add(dirSize);
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized Access Exception: {ex.Message}");
            }

            return sizes.Sum();
        }


        private void GetCount(DirectoryInfo obj)
        {
            try
            {
                var files = obj.GetFiles();
                var directories = obj.GetDirectories();

                filesCount += files.Length;
                fouldersCount += directories.Length;

                foreach (var directory in directories)
                {
                    GetCount(directory);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized Access Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private double SetProgressBar(object obj)
        {
            double ParentsSize = 0;
            double Procent = 0;
            if (obj is FileInfo Item)
            {
                ParentsSize = GetDirSize(Item.Directory);
                Procent = (info.Size / ParentsSize) * 100;
            }
            else if (obj is DirectoryInfo Item1)
            {
                if (Item1.Parent == null)
                {
                    Procent = 100;
                }
                else
                {
                    ParentsSize = GetDirSize(Item1.Parent);
                    Procent = (info.Size / ParentsSize) * 100;
                }

            }
            else
            {
            }

            return Procent;
        }

        private void GetDirectoryInfo(TreeViewItem Item)
        {
            info = new Info();
            DirectoryInfo directoryInfo = new DirectoryInfo(Item.Tag.ToString());

            double lengthInByte = Math.Round(GetDirSize(directoryInfo));
            fouldersCount = 0;
            filesCount = 0;
            GetCount(directoryInfo);

            info.Size = lengthInByte;
            info.FoldersCount = fouldersCount;
            info.FilesCount = filesCount;
            info.Allocated = info.Size;

            info.Value = Math.Round(SetProgressBar(directoryInfo), 1);
            info.Maximum = 100;
            info.Minimum = 0;
            info.LastModified = directoryInfo.LastWriteTime.ToShortDateString();

            currentInfo = new CurrentInfo();
            currentInfo.AddInfo(info);
            Method();

        }


        private void GetFileInfo(TreeViewItem Item)
        {
            try
            {
                info = new Info();
                FileInfo fileInfo = new FileInfo(Item.Tag.ToString());
                double lengthInByte = Math.Round((double)fileInfo.Length, 1);

                info.Size = lengthInByte;
                info.FoldersCount = 0;
                info.FilesCount = 1;
                info.Allocated = info.Size;
                info.Value = Math.Round(SetProgressBar(fileInfo), 1);
                info.Maximum = 100;
                info.Minimum = 0;
                info.LastModified = fileInfo.LastWriteTime.ToShortDateString();

                currentInfo = new CurrentInfo();
                currentInfo.AddInfo(info);

            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("file is not exist");
            }
            finally { 
                Method();
            }

        }
        private void Method()
        {
            if (CommboBox1.SelectedItem != null && info != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)CommboBox1.SelectedItem;
                string selectedText = selectedItem.Content.ToString();
                double temp = info.Size;

                if (selectedText == "GB") 
                {
                    info.Size = Math.Round(info.Size / (1024 * 1024 * 1024), 1);
                }
                if (selectedText == "MB")
                {
                    info.Size = Math.Round(info.Size / (1024 * 1024), 1);
                }
                if (selectedText == "KB")
                {
                    info.Size = Math.Round(info.Size / (1024), 1);

                }
                if (selectedText == "Byte")
                {
                    info.Size = temp;
                }
                info.Allocated = info.Size;
                currentInfo = new CurrentInfo();
                currentInfo.AddInfo(info);
                this.ListView1.ItemsSource = currentInfo;

                this.ListView1.UpdateLayout();

                info.Size = temp;
            }
        }

        private void CommboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Method();
        }
    }
}