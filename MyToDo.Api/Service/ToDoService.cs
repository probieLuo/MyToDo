using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Api.Context.UnitOfWork;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameter;
using System.Reflection.Metadata;
using MyToDo.Shared.Contact;
using System.Data.Common;

namespace MyToDo.Api.Service
{
    public class ToDoService : IToDoService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ToDoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> AddAsync(ToDoDto model)
        {
            try
            {
                ToDo dbTodo = mapper.Map<ToDo>(model);//数据传输层Dto与数据库实体层转换，AutoMapper实现
                await unitOfWork.GetRepository<ToDo>().InsertAsync(dbTodo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true, Result = dbTodo };
                return new ApiResponse() { Status = false, Message = "添加数据失败" };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var repository = unitOfWork.GetRepository<ToDo>();
                var todo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                repository.Delete(todo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true };
                return new ApiResponse() { Status = false, Message = "删除数据失败" };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter parameter)
        {
            try
            {
                var repository = unitOfWork.GetRepository<ToDo>();
                //var todos = await repository.GetAllAsync();
                var todos = await repository.GetPagedListAsync(predicate:
                x => string.IsNullOrWhiteSpace(parameter.Search) ? true : x.Title.Contains(parameter.Search),
                pageSize: parameter.PageSize,
                   pageIndex: parameter.PageIndex,
                   orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse() { Status = true, Result = todos };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> GetAllAsync(ToDoParameter parameter)
        {
            try
            {
                var repository = unitOfWork.GetRepository<ToDo>();
                //var todos = await repository.GetAllAsync();
                var todos = await repository.GetPagedListAsync(
                    predicate: x => string.IsNullOrWhiteSpace(parameter.Search) ? true : x.Title.Contains(parameter.Search)
                    && parameter.Status == null ? true : x.Status.Equals(parameter.Status),
                    pageSize: parameter.PageSize,
                    pageIndex: parameter.PageIndex,
                    orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse() { Status = true, Result = todos };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> GetSingleAsync(int id)
        {
            try
            {
                var repository = unitOfWork.GetRepository<ToDo>();
                var todo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                return new ApiResponse() { Status = true, Result = todo };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> UpdateAsync(ToDoDto model)
        {
            try
            {
                ToDo dbTodo = mapper.Map<ToDo>(model);//数据传输层Dto与数据库实体层转换，AutoMapper实现
                var repository = unitOfWork.GetRepository<ToDo>();
                var todo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(dbTodo.Id));

                todo.Title = dbTodo.Title;
                todo.Content = dbTodo.Content;
                todo.Status = dbTodo.Status;
                todo.UpdateDate = DateTime.Now;

                repository.Update(todo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true, Result = todo };
                return new ApiResponse() { Status = false, Message = "更新数据失败" };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }
    }
}
