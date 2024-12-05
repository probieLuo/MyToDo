using Microsoft.AspNetCore.Mvc;
using MyToDo.Api.Service;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameter;
using MyToDo.Shared.Contact;

namespace MyToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService service;

        public LoginController(ILoginService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<ApiResponse> Login(string account, string password) => await service.LoginAsync(account, password);

        [HttpPost]
        public async Task<ApiResponse> Resgiter([FromBody] UserDto para) => await service.Resgiter(para);

    }
}
