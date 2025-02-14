using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListAPI.Model
{
    public class ToDoList
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ToDoListItem> Items { get; set; }
    }
}
