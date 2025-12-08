using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Extensions;
using UserService.App.Interfaces;
using UserService.Contracts.DeliveryAddress;
using UserService.Domain.Entities;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Users/addresses")]
    [Authorize]
    public class DeliveryAddressController : ControllerBase
    {
        private readonly IUserService _userService;

        public DeliveryAddressController(IUserService userService)
        {
            _userService = userService;
        }
        
        
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = User.GetUserId();
            var addresses = await _userService.GetDeliveryAddressesAsync(userId);
            return Ok(addresses);
        }
        
        [HttpGet("me/{addressId}")]
        public async Task<IActionResult> GetMyAddressById([FromRoute] string addressId)
        {
            var userId = User.GetUserId();
            var address = await _userService.GetDeliveryAddressByIdAsync(userId, addressId);
            return Ok(address);
        }

        [HttpPost("me")]
        public async Task<IActionResult> AddMyAddress([FromBody] AddDeliveryAddressRequest dto)
        {
            var userId = User.GetUserId();
            var newAddress = new DeliveryAddress(dto.Street, dto.Number, dto.City, dto.State, dto.ZipCode);

            await _userService.AddDeliveryAddressAsync(userId, newAddress);
            return CreatedAtAction(nameof(GetMyAddresses), new { }, newAddress);
        }

        [HttpPut("me/{addressId}")]
        public async Task<IActionResult> UpdateMyAddress(string addressId, [FromBody] UpdateDeliveryAddressRequest dto)
        {
            var userId = User.GetUserId();
            var updatedAddress = new DeliveryAddress(dto.Street, dto.Number, dto.City, dto.State, dto.ZipCode);

            try
            {
                await _userService.UpdateDeliveryAddressAsync(userId, addressId, updatedAddress);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("me/{addressId}")]
        public async Task<IActionResult> DeleteMyAddress(string addressId)
        {
            var userId = User.GetUserId();
            
            try
            {
                await _userService.RemoveDeliveryAddressAsync(userId, addressId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}