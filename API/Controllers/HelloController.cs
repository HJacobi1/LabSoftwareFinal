using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class HelloController : ControllerBase
    {
        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult GetHello()
        {
            return Ok("Ooioioioi");
        }
        [HttpGet]
        [Route("api/[controller]/{name}")]
        public IActionResult GetHello(string name)
        {
            return Ok($"Hello, {name}!");
        }

        [HttpGet]
        [Route("api[controller]/{name}/{age}")]
        public IActionResult GetHello(string name, int age)
        {
            return Ok($"Hello, {name}! You are {age} years old.");
        }
    }
}
