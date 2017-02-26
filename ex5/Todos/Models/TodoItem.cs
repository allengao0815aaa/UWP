using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Storage;

namespace Todos.Models
{
    class TodoItem
    {
        private string id;

        public string title { get; set; }

        public string description { get; set; }

        public DateTimeOffset date { get; set; }

        public ImageSource bmi { get; set; }

        public StorageFile file { get; set; }

        public bool completed { get; set; }

        public TodoItem(string title, string description, DateTimeOffset date, ImageSource bmi, StorageFile file)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.description = description;
            this.date = date;
            this.bmi = bmi;
            this.file = file;
            this.completed = false;
        }
    }
}
