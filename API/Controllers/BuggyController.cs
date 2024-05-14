using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//返回http錯誤
namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        // 404
        [HttpGet("not-found")]
        public ActionResult GetNotFound(){
            return NotFound();
        }

        // 錯誤的請求
        [HttpGet("bad-request")]
        public ActionResult GetBadRequest(){
            return BadRequest(new ProblemDetails{Title = "This is a Bad Request"});
        }

        // 未授權
        [HttpGet("unauthorised")]
        public ActionResult GetUnauthorised(){
            return Unauthorized();
        }
        // 驗證錯誤
        [HttpGet("validation-error")]
        public ActionResult GetValidationError(){
            ModelState.AddModelError("Problem1","This is the First Error!");
            ModelState.AddModelError("Problem2","This is the Second Error!");
            return ValidationProblem();
        }

        // 伺服器錯誤
        [HttpGet("server-error")]
        public ActionResult GetServerError(){
            throw new Exception("This is a Server Error!");
        }
    }
}