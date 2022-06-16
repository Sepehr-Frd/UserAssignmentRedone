using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAssignmentRedone.Models;

namespace UserAssignmentRedone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IEnumerable<User> _users;
        private readonly ILogger<UserApiController> _logger;
        private static readonly Regex NationalIdPattern = new (@"^\d{10}$");
        private static readonly Regex UsernamePattern = new (@"^[A-Za-z][A-Za-z0-9_]{7,20}$");
        private static readonly Regex NamePattern = new (@"^[a-zA-Z]{3,20}$");
        private static readonly Regex PhoneNumberPattern = new (@"^09{1}\d{9}$");
        public UserApiController(UserDbContext context, ILogger<UserApiController> logger)
        {
            _context = context;
            _users = context.Users;
            _logger = logger;

        }

        // GET: api/UserApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("GetUser() started executing");
          if (_context.Users == null)
          {
              _logger.LogWarning("Users table references null!");
              return NotFound();
          }
          _logger.LogInformation("Returning all Users in a List");
          return await _context.Users.ToListAsync();
        }

        // GET: api/UserApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("GetUser(int id) started executing");

            
          if (_context.Users == null)
          {
              _logger.LogWarning("Users table references null!");
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                _logger.LogInformation("No User found with the passed id argument");
                return NotFound();
            }
            _logger.LogInformation("Returning the user requested by GetUser(id)");
            return Ok(user);
        }

        // PUT: api/UserApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            _logger.LogInformation("PutUser(int id, User user) started executing");
            
            if (!UserExists(id))
            {
                _logger.LogInformation("Requested User was not found");
                return NotFound();
            }
            
            if (id != user.Id)
            {
                _logger.LogInformation("Prevented Client from changing Id column");
                return StatusCode(403);
            }
            
            if (UpdateIsValid(user))
            {
                var local = _context.Set<User>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(user.Id));

                // check if local is not null 
                if (local != null)
                {
                    // detach
                    _context.Entry(local).State = EntityState.Detached;
                }
                
                _context.Entry(user).State = EntityState.Modified;
                
                _logger.LogInformation("The input was valid and the User was modified in the context");
                
            } else {
                _logger.LogInformation("The input was not valid, returning ValidationProblem()");
                return StatusCode(400);
            }
            
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Database update successful");
            }
            catch (DbUpdateConcurrencyException)
            {
                
                _logger.LogCritical("Database update failed, throwing DbUpdateConcurrencyException");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
            return Ok("Successfully updated the user.");
        }

        // POST: api/UserApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            _logger.LogInformation("PostUser(User user) started executing");
            
          if (_context.Users == null)
          {
              _logger.LogWarning("Users table references null, returning Problem()");
              return Problem("Entity set 'UserDbContext.Users'  is null.");
          }
          
          if (CreateIsValid(user))
          { 
              _logger.LogInformation("user information valid, adding user to context");
              
              _context.Users.Add(user);
              
              await _context.SaveChangesAsync();

              _logger.LogInformation("context saved successfully, returning CreatedAtAction(...)");
              
              return CreatedAtAction("GetUser", new { id = user.Id }, user);
              
          } 
          _logger.LogInformation("user information invalid, returning ValidationProblem()"); 
          return StatusCode(400);
          
          
        }

        // DELETE: api/UserApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("DeleteUser(int id) started executing");
            if (_context.Users == null)
            {
                _logger.LogWarning("Users table references null, returning Problem()");
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogInformation("user not found");
                return NotFound();
            }

            _context.Users.Remove(user);

            _logger.LogInformation("user found and deleted from context");
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("database update successful, returning NoContent()");
    
            return Ok("User successfully deleted.");
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        private bool UpdateIsValid(User user)
        {
            if (
                NationalIdPattern.IsMatch($"{user.NationalId}") &&
                UsernamePattern.IsMatch($"{user.Username}") &&
                NamePattern.IsMatch($"{user.FirstName}") &&
                NamePattern.IsMatch($"{user.LastName}") &&
                PhoneNumberPattern.IsMatch($"{user.PhoneNumber}") &&
                user.BirthDate < DateTime.Today
                )
            {
                foreach (var person in _users)
                {
                    if (
                        user.Id != person.Id &&
                        ( 
                         user.NationalId == person.NationalId || 
                         user.Username == person.Username ||
                         user.PhoneNumber == person.PhoneNumber)
                    )
                    {
                        return false;
                    }
                }
            } else
            {
                return false;
            }

            return true;
        }
        
        
        private bool CreateIsValid(User user)
        {

            if
            (
                NationalIdPattern.IsMatch($"{user.NationalId}") &&
                UsernamePattern.IsMatch($"{user.Username}") &&
                NamePattern.IsMatch($"{user.FirstName}") &&
                NamePattern.IsMatch($"{user.LastName}") &&
                PhoneNumberPattern.IsMatch($"{user.PhoneNumber}") &&
                user.BirthDate < DateTime.Today
            )
            {
                foreach (var person in _users)
                {
                    if (
                        user.NationalId == person.NationalId ||
                        user.Username == person.Username ||
                        user.PhoneNumber == person.PhoneNumber
                    )
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
