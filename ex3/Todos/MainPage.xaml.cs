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
            ViewModel.AddTodoItem("123", "123", DateTime.Today, myPicture.Source);
            ViewModel.AddTodoItem("456", "456", DateTime.Today, myPicture.Source);
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }

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
                ViewModel.AddTodoItem(title.Text, description.Text, DueDate.Date, myPicture.Source);
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
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem, title.Text, description.Text, DueDate.Date, myPicture.Source);
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
        }
    }
}
