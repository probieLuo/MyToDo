
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyToDo.Api.Context;
using MyToDo.Api.Context.Repository;
using MyToDo.Api.Context.UnitOfWork;
using MyToDo.Api.Extensions;
using MyToDo.Api.Service;

namespace MyToDo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<MyToDoContext>(opt =>
            {
                string? connStr = builder.Configuration.GetConnectionString("ToDoConnection");
                opt.UseSqlite(connStr);
            })
                .AddUnitOfWork<MyToDoContext>()
                .AddCustomRepository<ToDo, ToDoRepository>()
                .AddCustomRepository<Memo, MemoRepository>()
                .AddCustomRepository<User, UserRepository>()
                .AddTransient<IToDoService, ToDoService>()
                .AddTransient<IMemoService, MemoService>()
                .AddTransient<ILoginService, LoginService>();

            //Ìí¼ÓAutoMapper
            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProFile());
            });
            builder.Services.AddSingleton(autoMapperConfig.CreateMapper());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
