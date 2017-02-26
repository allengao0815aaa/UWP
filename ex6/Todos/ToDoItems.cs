using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos
{
    class ToDoItems
    {
        public string Title { get; set; }

        public ToDoItems(String title)
        {
            this.Title = title;
        }
    }
}
