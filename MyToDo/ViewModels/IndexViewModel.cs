using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MyToDo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        private readonly IToDoService todoService;
        private readonly IMemoService memoService;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        public IndexViewModel(IDialogHostService dialogService, IContainerProvider provider)
            : base(provider)
        {
            MessageInfo = "你好，probie! 今天是 " + DateTime.Now.ToString("yyyy-MM-dd");
            CreateTaskBars();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.todoService = provider.Resolve<IToDoService>();
            this.memoService = provider.Resolve<MemoService>();
            this.regionManager = provider.Resolve<IRegionManager>();
            this.dialogService = dialogService;
            EditToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompletedCommand = new DelegateCommand<ToDoDto>(Completed);
            NavigateCommand = new DelegateCommand<TaskBar>(Navigate);
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



        private SummaryDto summary;

        public SummaryDto Summary
        {
            get { return summary; }
            set { summary = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand<ToDoDto> ToDoCompletedCommand { get; private set; }
        public DelegateCommand<ToDoDto> EditToDoCommand { get; private set; }
        public DelegateCommand<MemoDto> EditMemoCommand { get; private set; }
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<TaskBar> NavigateCommand { get; private set; }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办":
                    AddToDo(null);
                    break;
                case "新增备忘录":
                    AddMemo(null);
                    break;
                default:
                    break;
            }
        }
        private async void Completed(ToDoDto dto)
        {
            var updatedResult = await todoService.UpdateAsync(dto);
            if (updatedResult.Status)
            {
                var todo = summary.ToDoList.FirstOrDefault(t => t.Id.Equals(dto.Id));
                if (todo != null)
                {
                    summary.ToDoList.Remove(todo);
                    summary.CompletedCount += 1;
                    summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%");
                    Refresh();
                }
            }
        }
        async void AddToDo(ToDoDto dto)
        {
            DialogParameters parameters = new DialogParameters();
            if (dto != null)
            {
                parameters.Add("Value", dto);
            }
            var dialogResult = await dialogService.ShowDialog("AddToDoView", parameters);
            if (dialogResult.Result == ButtonResult.OK)
            {
                ToDoDto toDoDto = dialogResult.Parameters.GetValue<ToDoDto>("Value");
                if (toDoDto.Id > 0)
                {
                    var updateResult = await todoService.UpdateAsync(toDoDto);
                    if (updateResult.Status)
                    {
                        var todoModel = summary.ToDoList.FirstOrDefault(x => x.Id.Equals(toDoDto.Id));
                        if (todoModel != null)
                        {
                            todoModel.Title = toDoDto.Title;
                            todoModel.Content = toDoDto.Content;
                        }
                    }
                }
                else
                {
                    var addResult = await todoService.AddAsync(toDoDto);
                    if (addResult.Status)
                    {
                        summary.Sum += 1;
                        summary.ToDoList.Add(addResult.Result);
                        summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%");
                        Refresh();
                    }
                }
            }
        }
        async void AddMemo(MemoDto dto)
        {
            DialogParameters parameters = new DialogParameters();
            if (dto != null)
            {
                parameters.Add("Value", dto);
            }
            var dialogResult = await dialogService.ShowDialog("AddMemoView", parameters);
            if (dialogResult.Result == ButtonResult.OK)
            {
                MemoDto memoDto = dialogResult.Parameters.GetValue<MemoDto>("Value");
                if (memoDto.Id > 0)
                {
                    var updateResult = await memoService.UpdateAsync(memoDto);
                    if (updateResult.Status)
                    {
                        var memoModel = summary.MemoList.FirstOrDefault(x => x.Id.Equals(memoDto.Id));
                        if (memoModel != null)
                        {
                            memoModel.Title = memoDto.Title;
                            memoModel.Content = memoDto.Content;
                        }
                    }
                }
                else
                {
                    var addResult = await memoService.AddAsync(memoDto);
                    if (addResult.Status)
                    {
                        summary.MemoCount += 1;
                        summary.MemoList.Add(addResult.Result);
                        Refresh();
                    }
                }
            }
        }

        async void Navigate(TaskBar bar)
        {
            if (string.IsNullOrWhiteSpace(bar.Target))
                return;

            NavigationParameters param = new NavigationParameters();
            if (bar.Title == "已完成")
            {
                param.Add("Value", 2);
            }

            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(bar.Target, param);
        }
        void CreateTaskBars()
        {
            TaskBars = new ObservableCollection<TaskBar>();
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Color = "#FF0CA0FF", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Color = "#FF1ECA3A", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "已完成比例", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Color = "#FFFFA000", Target = "MemoView" });
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var summaryResult = await todoService.SummaryAsync();
            if (summaryResult.Status)
            {
                Summary = summaryResult.Result;
                Refresh();
            }

            base.OnNavigatedFrom(navigationContext);
        }

        void Refresh()
        {
            TaskBars[0].Content = summary.Sum.ToString();
            TaskBars[1].Content = summary.CompletedCount.ToString();
            TaskBars[2].Content = summary.CompletedRatio;
            TaskBars[3].Content = summary.MemoCount.ToString();
        }
    }
}
