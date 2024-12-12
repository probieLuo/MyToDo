using MyToDo.Shared.Contact;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameter;

namespace MyToDo.Service
{
    public interface IToDoService : IBaseService<ToDoDto>
    {
        Task<ApiResponse<PagedList<ToDoDto>>> GetAllFilteredAsync(ToDoParameter toDoParameter);
        Task<ApiResponse<SummaryDto>> SummaryAsync();
    }
}
