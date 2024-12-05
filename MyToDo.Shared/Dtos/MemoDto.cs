namespace MyToDo.Shared.Dtos
{
    public class MemoDto : BaseDto
    {
        private string title;
        private string content;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        public string Content
        {
            get { return content; }
            set { content = value; OnPropertyChanged(); }
        }
    }
}
