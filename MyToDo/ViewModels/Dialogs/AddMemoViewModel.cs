﻿using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Shared.Dtos;

namespace MyToDo.ViewModels.Dialogs
{
    public class AddMemoViewModel : BindableBase, IDialogHostAware
    {
        public AddMemoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        private MemoDto model;
        /// <summary>
        /// 新增或编辑的实体
        /// </summary>
        public MemoDto Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
        }
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                Model = parameters.GetValue<MemoDto>("Value");
            }
            else
            {
                Model = new MemoDto();
            }
        }
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Model.Title) ||
               string.IsNullOrWhiteSpace(Model.Content))
                return;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();
                param.Add("Value", Model);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK) { Parameters = param });
            }
        }
    }
}
