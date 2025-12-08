    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Extensions;
using UserService.App.Interfaces;
using UserService.Contracts.Profile;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetUserId();
            
            if (request.Name != null)
            {
                await _userService.ChangeNameAsync(userId, request.Name);
            }

            if (request.PhoneNumber != null)
            {
                await _userService.ChangePhoneNumberAsync(userId, request.PhoneNumber);
            }
           
            return Ok();
        }

        [Authorize (Roles = "Admin")]
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            var user = await _userService.GetUserAsync(email);
            return Ok(user);       
        }
        
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId();
            var user = await _userService.GetUserAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] string userId)
        {
            var user = await _userService.GetUserAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _userService.GetAllUsersCount();
            return Ok(count);
        }
    }
}