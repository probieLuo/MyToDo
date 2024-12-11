namespace MyToDo.Common
{
    public interface IDialogHostAware
    {
        /// <summary>
        /// DialogHost名称
        /// </summary>
        string DialogHostName { get; set; }
        /// <summary>
        /// 打开对话框时触发
        /// </summary>
        /// <param name="parameters"></param>
        void OnDialogOpened(IDialogParameters parameters);
        /// <summary>
        /// 确定
        /// </summary>
        DelegateCommand SaveCommand { get; set; }
        /// <summary>
        /// 取消
        /// </summary>
        DelegateCommand CancelCommand { get; set; }
    }
}
