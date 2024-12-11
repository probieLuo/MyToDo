using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Shared.Dtos;
using System.Collections.ObjectModel;

namespace MyToDo.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        public IndexViewModel(IDialogHostService dialogService)
        {
            TaskBars = new ObservableCollection<TaskBar>();
            ToDoDtos = new ObservableCollection<ToDoDto>();
            MemoDtos = new ObservableCollection<MemoDto>();
            MessageInfo = "你好，probie! 今天是 " + DateTime.Now.ToString("yyyy-MM-dd");
            CreateTaskBars();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.dialogService = dialogService;
        }

        #region 属性
        private string messageInfo;

        public string MessageInfo
        {
            get { return messageInfo; }
            set { messageInfo = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<TaskBar> taskBars;

        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ToDoDto> toDoDtos;

        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<MemoDto> memoDtos;
        private readonly IDialogHostService dialogService;

        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand<string> ExecuteCommand { get; private set; }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办":
                    AddToDo();
                    break;
                case "新增备忘录":
                    AddMemo();
                    break;
                default:
                    break;
            }
        }
        void AddToDo()
        {
            dialogService.ShowDialog("AddToDoView", null);
        }
        void AddMemo()
        {
            dialogService.ShowDialog("AddMemoView", null);
        }
        void CreateTaskBars()
        {
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Content = "9", Color = "#FF0CA0FF", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Content = "9", Color = "#FF1ECA3A", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "已完成比例", Content = "100%", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Content = "19", Color = "#FFFFA000", Target = "" });
        }

    }
}
