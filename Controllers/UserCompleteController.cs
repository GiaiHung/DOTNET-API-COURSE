using System.Data;
using Dapper;
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
        string stringParameter = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameter += ", @UserId = @UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive < 2)
        {
            stringParameter += ", @Active = @ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (stringParameter != "")
        {
            sql += stringParameter.Substring(1);
        }

        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
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
