using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Dtos
{
    public class SummaryDto : BaseDto
    {
        private int sum;
        public int Sum
        {
            get { return sum; }
            set { sum = value; OnPropertyChanged(); }
        }


        private int completedCount;
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; OnPropertyChanged(); }
        }

        private int memoCount;
        public int MemoCount
        {
            get { return memoCount; }
            set { memoCount = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ToDoDto> todoList;

        private ObservableCollection<MemoDto> memoList;

        public ObservableCollection<ToDoDto> ToDoList
        {
            get { return todoList; }
            set { todoList = value; OnPropertyChanged(); }
        }
        public ObservableCollection<MemoDto> MemoList
        {
            get { return memoList; }
            set { memoList = value; OnPropertyChanged(); }
        }
        private string completedRatio;
        public string CompletedRatio
        {
            get { return completedRatio; }
            set { completedRatio = value; OnPropertyChanged(); }
        }
    }
}
