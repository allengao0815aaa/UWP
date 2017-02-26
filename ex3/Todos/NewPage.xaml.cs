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
        }

        private ViewModels.TodoItemViewModel ViewModel;

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
            }
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
