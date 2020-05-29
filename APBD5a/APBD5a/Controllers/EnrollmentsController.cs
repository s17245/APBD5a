using Microsoft.AspNetCore.Mvc;
using APBD5a.DTOs.Responses;
using APBD5a.DTOs.Requests;
using APBD5a.Service;
using Microsoft.AspNetCore.Authorization;

//http://localhost:59610/api/enrollments

namespace APBD5a.Controllers
{
    [ApiController]

    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }


        [HttpPost]
        [Route("api/enrollments")]
        [Authorize(Roles = "employee")]  
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);

            if (response.dtoResponse == "Done"){
                return Created(response.dtoResponse, response);
            }
            else{
                return BadRequest(response);
            }

        }


        [HttpPost]
        [Route("api/enrollments/promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            var response = _service.PromoteStudents(request);

            return Created("promoted", response);

        }   
    }
}