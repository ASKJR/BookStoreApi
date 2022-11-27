using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService) => _usersService = usersService;


    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        bool isUserRegistered = await _usersService.GetAsync(newUser.Email) is not null;
        
        if (isUserRegistered)
        {
            return BadRequest("User already registered.");
        }

        try
        {
            await _usersService.CreateAsync(newUser);
        }
        catch (FormatException) 
        {
            return BadRequest("Invalid request body.");
        }
        catch (Exception)
        {
            return BadRequest("Not able to insert a new user in the DB.");
        }
        
        return CreatedAtAction(nameof(Post), new { id = newUser.Id }, newUser);
    }
}