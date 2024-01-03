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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public Post GetPostSingle(int postId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostId = " + postId.ToString();

            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE UserId = " + userId.ToString();

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE UserId = " + this.User.FindFirst("userId")?.Value;

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostTitle LIKE '%" + searchParam + "%'" +
                        " OR PostContent LIKE '%" + searchParam + "%'";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("AddPost")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated]
                ) VALUES (" + this.User.FindFirst("userId")?.Value
                + ",'" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "', GETDATE())";
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpPut("EditPost")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sqlEditPost = @$"
                update TutorialAppSchema.Posts 
                set
                    PostTitle = '{postToEdit.PostTitle}',
                    PostContent = '{postToEdit.PostContent}',
                    PostUpdated = GETDATE()
                where PostId = {postToEdit.PostId}
            ";
            if (_dapper.ExecuteSql(sqlEditPost))
            {
                return Ok($"Post {postToEdit.PostId} has been updated successfully.");
            }
            throw new Exception($"Failed to update post with id: {postToEdit.PostId}");
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sqlDeletePost = @$"
                delete from TutorialAppSchema.Posts
                where PostId = {postId}
            ";
            if (_dapper.ExecuteSql(sqlDeletePost))
            {
                return Ok($"Post {postId} has been deleted.");
            }
            throw new Exception("Failed to delete post.");
        }
    }
}