using System;
using Kremis.Api.Initializer;
using Kremis.Domain.Contexts;
using Kremis.Domain.Entities;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kremis.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DbInitializerController : ControllerBase
    {

        private readonly IDbInitializer _dbInitializer;
        public DbInitializerController(IDbInitializer dbInitializer)
        {
            _dbInitializer = dbInitializer;
        }

        [HttpPost("{ensureDeleted}")]
        public IActionResult Post(bool ensureDeleted = false)
        {
            try
            {
                _dbInitializer.Initialize(ensureDeleted);
                return Ok("Database initialization succeed");
            }
            catch (Exception)
            {
                return BadRequest("Database initialization failed. Please checks the log files for more information about the error.");
            }
        }
    }
}
