using System;
using System.Collections.Generic;
using System.Linq;
using ToDoListAPI.Model;

namespace ToDoListAPI.Services
{
    public class ToDoListService
    {
        private readonly List<ToDoList> _toDoLists = new List<ToDoList>();

        public List<ToDoList> GetLists() => _toDoLists;

        public ToDoList GetListById(Guid id) => _toDoLists.FirstOrDefault(l => l.Id == id);

        public ToDoList AddList(ToDoList checklist)
        {
            _toDoLists.Add(checklist);
            return checklist;
        }

        public bool DeleteList(Guid id)
        {
            var checklist = GetListById(id);
            if (checklist == null) return false;

            _toDoLists.Remove(checklist);
            return true;
        }

        public ToDoList AddItemToList(Guid checklistId, ToDoListItem item)
        {
            var checklist = GetListById(checklistId);
            if (checklist == null) return null;

            checklist.Items.Add(item);
            return checklist;
        }

        public ToDoList UpdateItemInList(Guid checklistId, Guid itemId, string newName)
        {
            var checklist = GetListById(checklistId);
            if (checklist == null) return null;

            var item = checklist.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            item.Name = newName;
            return checklist;
        }

        public ToDoList UpdateItemStatus(Guid checklistId, Guid itemId, bool isCompleted)
        {
            var checklist = GetListById(checklistId);
            if (checklist == null) return null;

            var item = checklist.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            item.IsCompleted = isCompleted;
            return checklist;
        }
    }
}
