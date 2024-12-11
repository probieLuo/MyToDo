using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Api.Context.UnitOfWork;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameter;
using System;
using MyToDo.Shared.Contact;

namespace MyToDo.Api.Service
{
    public class MemoService : IMemoService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MemoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> AddAsync(MemoDto model)
        {
            try
            {
                Memo dbMemo = mapper.Map<Memo>(model);//数据传输层Dto与数据库实体层转换，AutoMapper实现
                await unitOfWork.GetRepository<Memo>().InsertAsync(dbMemo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true, Result = dbMemo };
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
                var repository = unitOfWork.GetRepository<Memo>();
                var memo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                repository.Delete(memo);
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
                var repository = unitOfWork.GetRepository<Memo>();
                var memos = await repository.GetPagedListAsync(predicate:
                   x => string.IsNullOrWhiteSpace(parameter.Search) ? true : x.Title.Contains(parameter.Search),
                   pageSize: parameter.PageSize,
                   pageIndex: parameter.PageIndex,
                   orderBy: source => source.OrderByDescending(t => t.CreateDate));
                return new ApiResponse() { Status = true, Result = memos };
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
                var repository = unitOfWork.GetRepository<Memo>();
                var memo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(id));
                return new ApiResponse() { Status = true, Result = memo };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> UpdateAsync(MemoDto model)
        {
            try
            {
                Memo dbMemo = mapper.Map<Memo>(model);//数据传输层Dto与数据库实体层转换，AutoMapper实现
                var repository = unitOfWork.GetRepository<Memo>();
                var memo = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id.Equals(dbMemo.Id));

                memo.Title = dbMemo.Title;
                memo.Content = dbMemo.Content;
                memo.UpdateDate = DateTime.Now;

                repository.Update(memo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true, Result = memo }; ;
                return new ApiResponse() { Status = false, Message = "更新数据失败" };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = ex.Message };
            }
        }
    }
}
