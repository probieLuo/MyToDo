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
    public class MemoViewModel : BindableBase
    {
        public MemoViewModel(IMemoService service)
        {
            MemoDtos = new ObservableCollection<MemoDto>();

            AddMomeCommand = new DelegateCommand(AddMemo);
            this.service = service;
            CreateTodoList();
        }

        private void AddMemo()
        {
            IsRightDrawerOpen = true;
        }
        private bool isRightDrawerOpen;

        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }


        public DelegateCommand AddMomeCommand { get; set; }

        private ObservableCollection<MemoDto> memoDtos;
        private readonly IMemoService service;

        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
        }
        async Task CreateTodoList()
        {
            var memoResult = await service.GetAllAsync(new Shared.Parameter.QueryParameter()
            {
                PageIndex = 0,
                PageSize = 100
            });
            if (memoResult.Status)
            {
                MemoDtos.Clear();
                foreach (var item in memoResult.Result.Items)
                {
                    MemoDtos.Add(item);
                }
            }
        }
    }
}
