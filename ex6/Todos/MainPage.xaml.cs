using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using SQLitePCL;


//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel();
            myPicture.Source = new BitmapImage(new Uri(BaseUri, "Assets/default_image.jpg"));
            init();
            ViewModel.AddTodoItem(1, "first", "Hello!", DateTime.Today, myPicture.Source, currentfile);
            ViewModel.AddTodoItem(2, "second", "Nice to meet you!", DateTime.Today, myPicture.Source, currentfile);
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }
        public StorageFile currentfile, initfile;

        private async void init()
        {
            initfile = await Package.Current.InstalledLocation.GetFileAsync("Assets\\default_image.jpg");
            currentfile = initfile;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rect rect);

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            IntPtr hwnd = GetForegroundWindow();
            Rect rect = new Rect();
            GetWindowRect(hwnd, out rect);
            int width = rect.Right - rect.Left;

            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);

            if (width <= 800)
                Frame.Navigate(typeof(NewPage), ViewModel);
            else
            {
                createButton.Content = "Update";
                createButton.Click -= CreateButton_Clicked;
                createButton.Click += UpdateButton_Clicked;
                title.Text = ViewModel.SelectedItem.title;
                description.Text = ViewModel.SelectedItem.description;
                DueDate.Date = ViewModel.SelectedItem.date;
                myPicture.Source = ViewModel.SelectedItem.bmi;
                currentfile = ViewModel.SelectedItem.file;
            }
        }
        
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = GetForegroundWindow();
            Rect rect = new Rect();
            GetWindowRect(hwnd, out rect);
            int width = rect.Right - rect.Left;
            if (width <= 800)
                Frame.Navigate(typeof(NewPage), ViewModel);
        }

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ok())
            {
                var db = App.conn;
                using (var statement = db.Prepare("INSERT INTO TodoItem (Title, Context, Date) VALUES (?, ?, ?)"))
                {
                    statement.Bind(1, title.Text);
                    statement.Bind(2, description.Text);
                    statement.Bind(3, DueDate.Date.Date.ToString("yyyy/MM/dd"));
                    statement.Step();
                }

                int id;
                using (var statement = db.Prepare("SELECT max(Id) FROM TodoItem"))
                {
                    statement.Step();
                    id = (int)statement[0];
                }

                ViewModel.AddTodoItem(id, title.Text, description.Text, DueDate.Date, myPicture.Source, currentfile);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                if (ok())
                {
                    var db = App.conn;
                    using (var statement = db.Prepare("UPDATE TodoItem SET Title = ?, Context = ?, Date = ? WHERE Id = ?"))
                    {
                        statement.Bind(1, title.Text);
                        statement.Bind(2, description.Text);
                        statement.Bind(3, DueDate.Date.Date.ToString("yyyy/MM/dd"));
                        statement.Bind(4, ViewModel.SelectedItem.id);
                        statement.Step();
                    }

                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem, title.Text, description.Text, DueDate.Date, myPicture.Source, currentfile);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
            }
        }

        private bool ok()
        {
            if (title.Text == "" || description.Text == "" || DueDate.Date < DateTime.Today)
            {
                string WrongMessage = "";
                if (title.Text == "")
                    WrongMessage += "Please input your title!\n";
                if (description.Text == "")
                    WrongMessage += "Please input your description!\n";
                if (DueDate.Date < DateTime.Today)
                    WrongMessage += "The date is wrong! You can't choose a date which is earlier than today.\n";
                var i = new MessageDialog(WrongMessage).ShowAsync();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            description.Text = "";
            DueDate.Date = DateTime.Today;
            myPicture.Source = new BitmapImage(new Uri(BaseUri, "Assets/default_image.jpg"));
            currentfile = initfile;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            XmlDocument tileXml = new XmlDocument();
            tileXml.LoadXml(File.ReadAllText("tiles.xml"));

            XmlNodeList str = tileXml.GetElementsByTagName("text");
            for (int i = 0; i < str.Count; i += 2)
            {
                Models.TodoItem lastItem = ViewModel.AllItems.Last();
                ((XmlElement)str[i]).InnerText = lastItem.title;
                ((XmlElement)str[i+1]).InnerText = lastItem.description;
            }

            TileNotification noti = new TileNotification(tileXml);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Update(noti);
        }

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            var def = e.Request.GetDeferral();
            DataPackage package = new DataPackage();
            package.Properties.Title = "共享文本";
            package.Properties.Description = "分享一些内容";
            package.SetText("Title: \n"+ViewModel.SelectedItem.title+"    \n\nDescription: \n"+ ViewModel.SelectedItem.description+"\n");
            package.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(ViewModel.SelectedItem.file);
            package.SetBitmap(RandomAccessStreamReference.CreateFromFile(ViewModel.SelectedItem.file));
            e.Request.Data = package;
            def.Complete();
        }

        private void share_click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(((MenuFlyoutItem)sender).DataContext);
            DataTransferManager.ShowShareUI();
        }

        private void BtnGetAll_Click(object sender, RoutedEventArgs e)
        {
            var db = App.conn;
            string result = "";
            using (var statement = db.Prepare("SELECT Id, Title, Context, Date FROM TodoItem WHERE Title LIKE ? OR Context LIKE ? OR Date LIKE ?"))
            {
                statement.Bind(1, "%" + Query.Text + "%");
                statement.Bind(2, "%" + Query.Text + "%");
                statement.Bind(3, "%" + Query.Text + "%");
                while (SQLiteResult.ROW == statement.Step())
                {
                    result += "Id : " + (long)statement[0] + " ;   ";
                    result += "Title : " + (string)statement[1] + " ;   ";
                    result += "Context : " + (string)statement[2] + " ;   ";
                    result += "Date : " + (string)statement[3] + " ;  ";
                    result += "\n";
                }
            }
            if (result == "") result = "Cannot find results.\n";
            var i = new MessageDialog(result).ShowAsync();
        }
    }
}
