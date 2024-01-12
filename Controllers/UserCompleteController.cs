using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    private readonly ReusableSQL _reusableSql;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSQL(config);
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
        if (_reusableSql.UserUpsert(user))
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
