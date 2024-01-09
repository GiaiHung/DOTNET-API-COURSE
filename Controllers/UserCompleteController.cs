using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, int isActive)
    {
        string sql = "exec TutorialAppSchema.GetUserDetail";
        string parameters = "";

        if (userId != 0)
        {
            parameters += $", @UserId = {userId}";
        }
        if (isActive < 2)
        {
            parameters += $", @Active = {isActive}";
        }
        if (parameters != "")
        {
            sql += parameters.Substring(1);
        }

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }

    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        string sql = @$"
            EXEC TutorialAppSchema.UserUpsert
                @FirstName = '{user.FirstName}',
                @LastName = '{user.LastName}',
                @Email = '{user.Email}',
                @Gender = '{user.Gender}',
                @Active = {user.Active},
                @JobTitle = '{user.JobTitle}',
                @Department = '{user.Department}',
                @Salary = {user.Salary},
                @AvgSalary = {user.AvgSalary},
                @UserId = {user.UserId}
        ";

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully updated user");
        }

        throw new Exception("Failed to Update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @$"EXEC TutorialAppSchema.UserDelete @UserId = {userId}";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok($"Successfully delete user {userId}");
        }

        throw new Exception("Failed to Delete User");
    }
}
