using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetPosts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.GetPosts";
            string parameters = "";

            if (postId != 0)
            {
                parameters += $", @PostId = {postId}";
            }
            if (userId != 0)
            {
                parameters += $", @UserId = {userId}";
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += $", @Search = '{searchParam}'";
            }

            if (parameters != "")
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.GetPosts @UserId = " + this.User.FindFirst("userId")?.Value;

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToAdd)
        {
            string sql = @$"
            Exec TutorialAppSchema.PostUpsert
                @UserId = {this.User.FindFirst("userId")?.Value},
                @PostTitle = '{postToAdd.PostTitle}',
                @PostContent = '{postToAdd.PostContent}'
            ";

            if (postToAdd.PostId != 0)
            {
                sql += $", @PostId = {postToAdd.PostId}";
            }

            if (_dapper.ExecuteSql(sql))
            {
                return Ok("Upsert Post Completed");
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sqlDeletePost = @$"
                delete from TutorialAppSchema.Posts
                where PostId = {postId}
                and UserId = {this.User.FindFirst("userId")?.Value}
            ";
            if (_dapper.ExecuteSql(sqlDeletePost))
            {
                return Ok($"Post {postId} has been deleted.");
            }
            throw new Exception("Failed to delete post.");
        }
    }
}