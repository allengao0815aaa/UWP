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
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;


namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            init();
        }

        private ViewModels.TodoItemViewModel ViewModel;
        public StorageFile currentfile, initfile;

        private async void init()
        {
            initfile = await Package.Current.InstalledLocation.GetFileAsync("Assets\\default_image.jpg");
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

            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);

            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
                myPicture.Source = new BitmapImage(new Uri(BaseUri, "Assets/default_image.jpg"));
                currentfile = initfile;
            }
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

                long id;
                using (var statement = db.Prepare("SELECT max(Id) FROM TodoItem"))
                {
                    statement.Step();
                    id = (long)statement[0];
                }

                ViewModel.AddTodoItem(id, title.Text, description.Text, DueDate.Date, myPicture.Source, currentfile);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                var db = App.conn;
                using (var statement = db.Prepare("DELETE FROM TodoItem WHERE Id = ?"))
                {
                    statement.Bind(1, ViewModel.SelectedItem.id);
                    statement.Step();
                }

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
        
        private async void SelectPictureButton_Clicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                currentfile = file;
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 350;
                    bitmapImage.DecodePixelHeight = 180;
                    await bitmapImage.SetSourceAsync(fileStream);
                    myPicture.Source = bitmapImage;
                }
            }
        }
    }
}
