using MyToDo.Common.Models;
using MyToDo.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            this.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            CreateMenuBar();
        }
        private ObservableCollection<MenuBar> menuBars;
        private readonly IRegionManager regionManager;

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }

        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrEmpty(bar.NameSpace))
                return;
            regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate(bar.NameSpace);


        }

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set
            {
                menuBars = value; RaisePropertyChanged();
            }
        }

        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Palette", Title = "个性化", NameSpace = "SkinView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "系统设置", NameSpace = "" });
            MenuBars.Add(new MenuBar() { Icon = "Information", Title = "关于更多", NameSpace = "AboutView" });
        }
    }
}
