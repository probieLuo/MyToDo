using MyToDo.Common.Models;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class ToDoViewModel : NavigationViewModel
    {
        public ToDoViewModel(IToDoService service, IContainerProvider containerProvider)
            : base(containerProvider)
        {
            ToDoDtos = new ObservableCollection<ToDoDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<ToDoDto>(Selected);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);
            this.service = service;

        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增":
                    Add();
                    break;
                case "查询":
                    GetDataAsync();
                    break;
                case "保存":
                    Save();
                    break;
            }
        }



        private string seach;
        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Seach
        {
            get { return seach; }
            set { seach = value; RaisePropertyChanged(); }
        }
        private int selectedIndex;
        /// <summary>
        /// 下拉框列表选中值
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; }
        }


        private void Add()
        {
            CurrentDto = new ToDoDto();
            IsRightDrawerOpen = true;
        }
        private async void Selected(ToDoDto dto)
        {
            try
            {
                UpdateLoading(true);
                var todoResult = await service.GetFirstOfDefaultAsync(dto.Id);
                if (todoResult.Status)
                {
                    CurrentDto = todoResult.Result;
                }
                IsRightDrawerOpen = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }

        }
        private async void Delete(ToDoDto dto)
        {
            try
            {
                UpdateLoading(true);
                var todoResult = await service.DeleteAsync(dto.Id);
                if (todoResult.Status)
                {
                    var model = ToDoDtos.FirstOrDefault(t => t.Id.Equals(dto.Id));
                    if (model != null) { ToDoDtos.Remove(model); }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }
        private bool isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }

        private ToDoDto currentDto;
        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>
        public ToDoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> SelectedCommand { get; private set; }
        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }
        private ObservableCollection<ToDoDto> toDoDtos;
        private readonly IToDoService service;

        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }
        async void GetDataAsync()
        {
            UpdateLoading(true);
            var todoResult = await service.GetAllAsync(new Shared.Parameter.QueryParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                Search = Seach,
            });
            if (todoResult.Status)
            {
                toDoDtos.Clear();
                foreach (var item in todoResult.Result.Items)
                {
                    toDoDtos.Add(item);
                }
            }
            UpdateLoading(false);
        }
        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) || string.IsNullOrWhiteSpace(CurrentDto.Content))
            {
                return;
            }
            UpdateLoading(true);
            try
            {
                if (CurrentDto.Id > 0)
                {
                    var updateResult = await service.UpdateAsync(CurrentDto);
                    if (updateResult.Status)
                    {
                        var todo = ToDoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (todo != null)
                        {
                            todo.Title = CurrentDto.Title;
                            todo.Content = CurrentDto.Content;
                            todo.Status = CurrentDto.Status;

                        }
                    }
                    IsRightDrawerOpen = false;
                }
                else
                {
                    var addResult = await service.AddAsync(CurrentDto);
                    if (addResult.Status)
                    {
                        ToDoDtos.Add(addResult.Result);
                        IsRightDrawerOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            GetDataAsync();
        }
    }
}
