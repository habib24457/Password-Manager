using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Models.Domain;
using PasswordManager.Service;

namespace PasswordManager.Controllers
{
    // /api/passwords
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly PasswordsDbContext _dbContext;

        public PasswordsController(PasswordsDbContext passwordsDbContext)
        {
            _dbContext = passwordsDbContext;
        }

        //Get All Passwords
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var passwords = await _dbContext.PasswordItem.ToListAsync();
            return Ok(passwords);
        }

        //Create password
        [HttpPost]
        public async Task<IActionResult> CreatePassword([FromBody] Password password)
        {
            if (password == null)
                return BadRequest();

            EncryptionService encryptPassword = new EncryptionService();
            var encryptedPassword = encryptPassword.EncryptPassword(password.UserPassword);

            var addPassword = new Password
            {
                UserName = password.UserName,
                Category = password.Category,
                App = password.App,
                UserPassword = encryptedPassword
            };
            _dbContext.Add(addPassword);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction("CreatePassword", new { id = password.Id }, password);
        }

        //Get One Password
        [HttpGet]
        [Route("{id:int}/{isDecrypted}")]
        public async Task<IActionResult> GetOneById([FromRoute] int id, bool isDecrypted)
        {
            if (id == null)
                return BadRequest();

            var passwordItem = await _dbContext.PasswordItem.FindAsync(id);

            if (passwordItem == null)
            {
                return NotFound(); //404 
            }

            if (isDecrypted)
            {
                var encryptedPassword = passwordItem.UserPassword;
                EncryptionService encryptPasswordObj = new EncryptionService();
                var decryptedPassword = encryptPasswordObj.DecryptPassword(encryptedPassword);
                passwordItem.UserPassword = decryptedPassword;
                return Ok(passwordItem);
            }

            return Ok(passwordItem);
        }

        //Update a Password store item
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePassword([FromBody] Password password, [FromRoute] int id)
        {
            if (id == null)
                return BadRequest();
            var existingPassword = await _dbContext.PasswordItem.FindAsync(id);
            if (existingPassword == null)
            {
                return NotFound(); //404 
            }

            EncryptionService encryptPasswordObj = new EncryptionService();

            existingPassword.UserName = password.UserName;
            existingPassword.App = password.App;
            existingPassword.Category = password.Category;
            existingPassword.UserPassword = encryptPasswordObj.EncryptPassword(password.UserPassword);
            await _dbContext.SaveChangesAsync();

            return Ok(existingPassword);
        }

        //Delete a Password store item

    }
}

