using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace Todos.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; }  }

        public void AddTodoItem(string title, string description, DateTimeOffset date, ImageSource bmi)
        {
            this.allItems.Add(new Models.TodoItem(title, description, date, bmi));
        }

        public void RemoveTodoItem(Models.TodoItem now)
        {
            this.allItems.Remove(now);
            this.selectedItem = null;
        }

        public void UpdateTodoItem(Models.TodoItem now, string title, string description, DateTimeOffset date, ImageSource bmi)
        {
            int id = this.allItems.IndexOf(now);
            this.allItems[id].title = title;
            this.allItems[id].description = description;
            this.allItems[id].date = date;
            this.allItems[id].bmi = bmi;
            this.selectedItem = null;
        }

    }
}
