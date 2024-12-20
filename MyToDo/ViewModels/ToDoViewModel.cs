﻿using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
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
        private readonly IToDoService service;
        private readonly IDialogHostService dialogHost;
        public ToDoViewModel(IToDoService service, IContainerProvider containerProvider)
            : base(containerProvider)
        {
            ToDoDtos = new ObservableCollection<ToDoDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<ToDoDto>(Selected);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);
            dialogHost = containerProvider.Resolve<IDialogHostService>();
            this.service = service;
        }
        private ToDoDto currentDto;
        public ToDoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        private string search;
        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
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
        private bool isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<ToDoDto> toDoDtos;
        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> SelectedCommand { get; private set; }
        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }
        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>

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
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项：{dto.Title}？");
                if (dialogResult.Result != ButtonResult.OK)
                    return;
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

        async void GetDataAsync()
        {
            UpdateLoading(true);
            int? status = selectedIndex == 0 ? null : selectedIndex == 2 ? 1 : 0;
            var todoResult = await service.GetAllFilteredAsync(new Shared.Parameter.ToDoParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                Search = Search,
                Status = status,
            });
            if (todoResult.Status)
            {
                ToDoDtos.Clear();
                foreach (var item in todoResult.Result.Items)
                {
                    ToDoDtos.Add(item);
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
            if (navigationContext.Parameters.TryGetValue("Value", out int value))
            {
                SelectedIndex = value;
            }
            else
            {
                SelectedIndex = 0;
            }
            GetDataAsync();
        }
    }
}
