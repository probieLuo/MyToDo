using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Api.Context.UnitOfWork;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Contact;

namespace MyToDo.Api.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public LoginService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> LoginAsync(string account, string password)
        {
            try
            {
                var model = await unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: x => x.Account.Equals(account) && x.PassWord.Equals(password));
                if (model == null)
                    return new ApiResponse() { Status = false, Message = "账号或密码错误，请重试！" };
                return new ApiResponse() { Status = true, Result = model };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = "登录失败！" };
            }
        }

        public async Task<ApiResponse> Resgiter(UserDto user)
        {
            try
            {
                var model = mapper.Map<User>(user);
                var repository = unitOfWork.GetRepository<User>();
                var userModel = await repository.GetFirstOrDefaultAsync(predicate: x => x.Account.Equals(model.Account));
                if (userModel != null)
                    return new ApiResponse() { Message = $"当前账号：{model.Account}已存在，请重新注册！", Status = false };
                model.CreateDate = DateTime.Now;
                await repository.InsertAsync(model);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse() { Status = true, Result = model };
                return new ApiResponse() { Status = false, Message = "添加失败，请稍后重试！" };
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = "注册账号失败！" };
            }
        }
    }
}
