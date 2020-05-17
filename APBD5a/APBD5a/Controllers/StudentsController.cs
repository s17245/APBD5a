using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD5a.DAL;
using APBD5a.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD5a.Controllers
{
    [ApiController]
    [Route("api/students")]
    //[Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        //[HttpGet("{id}")]
        //public IActionResult GetEnrollment(string id)
        //{
        //    return Ok(_dbService.GetEnrollment(id));
        //}

    }
}