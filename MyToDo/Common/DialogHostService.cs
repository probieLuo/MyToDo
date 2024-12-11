using MaterialDesignThemes.Wpf;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyToDo.Common
{
    /// <summary>
    /// 对话主机服务（自定义）
    /// </summary>
    public class DialogHostService : DialogService, IDialogHostService
    {
        private readonly IContainerExtension containerExtension;

        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
        {
            this.containerExtension = containerExtension;
        }

        public async Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string diaHostName = "Root")
        {
            parameters ??= new DialogParameters();

            //从容器当中取出弹出窗口的实例
            var content = containerExtension.Resolve<object>(name);

            //检查实例的有效性
            if (!(content is FrameworkElement dialogContent))
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
                ViewModelLocator.SetAutoWireViewModel(view, true);

            if (!(dialogContent.DataContext is IDialogHostAware viewModel))
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

            viewModel.DialogHostName = diaHostName;
            DialogOpenedEventHandler eventHandler = (sender, eventargs) =>
            {
                if (viewModel is IDialogHostAware aware)
                {
                    aware.OnDialogOpened(parameters);
                }
                eventargs.Session.UpdateContent(content);
            };

            return (IDialogResult)await DialogHost.Show(dialogContent, viewModel.DialogHostName, eventHandler);
        }
    }
}
