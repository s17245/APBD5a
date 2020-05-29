using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD5a.DAL;
using APBD5a.Models;
using APBD5a.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace APBD5a.Controllers
{
    [ApiController]
    [Route("api/students")]
    //[Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;
        private readonly IStudentDbService _service;      

        public IConfiguration Configuration { get; }
        public StudentsController(IDbService dbService, IStudentDbService service, IConfiguration configuration)
        {
            _dbService = dbService;
            _service = service;                     
            Configuration = configuration;              
        }

        [HttpGet]
        [Authorize]                 
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        //[HttpGet("{id}")]
        //public IActionResult GetEnrollment(string id)
        //{
        //    return Ok(_dbService.GetEnrollment(id));
        //}
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            var response = _service.LoginStudent(request.Login, request.Haslo);
            if (response == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost]                             
        [Route("refreshToken/{token}")]
        public IActionResult RefreshToken(string token)
        {
            var response = _service.RefreshToken(token);
            if (response == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(response);
            }                                    
        }
    }
}