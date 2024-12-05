using MyToDo.Common.Models;
using MyToDo.Extensions;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IRegionManager regionManager)
        {
            menuBars = new ObservableCollection<MenuBar>();

            createMenuBar();

            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);

            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                    journal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                    journal.GoForward();
            });

            this.regionManager = regionManager;
        }

        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace))
                return;
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(bar.NameSpace, back =>
            {
                journal = back.Context.NavigationService.Journal;
            });


        }

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 创建菜单
        /// </summary>
        void createMenuBar()
        {
            MenuBars.Add(new MenuBar { Icon = "Home", NameSpace = "IndexView", Title = "首页" });
            MenuBars.Add(new MenuBar { Icon = "NotebookOutline", NameSpace = "ToDoView", Title = "待办事项" });
            MenuBars.Add(new MenuBar { Icon = "NotebookPlus", NameSpace = "MemoView", Title = "备忘录" });
            MenuBars.Add(new MenuBar { Icon = "Settings", NameSpace = "SettingsView", Title = "设置" });
        }
    }
}
