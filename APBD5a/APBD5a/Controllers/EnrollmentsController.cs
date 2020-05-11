using Microsoft.AspNetCore.Mvc;
using APBD5a.DTOs.Responses;
using APBD5a.DTOs.Requests;

//http://localhost:59610/api/enrollments

namespace APBD5a.Controllers
{
    [Route("api/enrollments")]
    [ApiController] 
    public class EnrollmentsController : ControllerBase
    {
        //private IStudentDbService _service;

       // public EnrollmentsController(IStudentDbService service)
        //{
         //   _service = service;
       // }


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            // _service.EnrollStudent(request);
            var response = new EnrollStudentResponse();
            //response.LastName = st.LastName;
            //...

            return Ok(response);
        }




    }
}