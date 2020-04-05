using APBD_Task4.DTOs.Requests;
using APBD_Task4.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Task4.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18933;Integrated Security=True";
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service) => _service = service;
        [HttpPost]
        public IActionResult EnrollStudent([FromBody] EnrollStudentRequest request)
        {
            var enrollment = _service.EnrollStudent(request);
            if (enrollment == null)
            {
                return BadRequest("400 Bad Request Error!");
            }
            return CreatedAtAction(nameof(EnrollStudent), enrollment);
        }


        [HttpPost("promotion")]
        public IActionResult PromoteStudents(EnrollStudentRequest request)
        {
            var enrollment = _service.PromoteStudents(request);
            if (enrollment == null)
            {
                return BadRequest("400 Bad Request Error!");
            }
            return CreatedAtAction(nameof(EnrollStudent), enrollment);
        }

        [HttpGet("{MoodPill}")]
        public IActionResult MoodPill(int MoodPill)
        {
            if (MoodPill == 888)
            {
                return Ok("Don't accidentally choke: https://www.youtube.com/watch?v=_tSWSpvNT1Y");
            }
            return BadRequest("Ok, but try again next time, if you like :)");
        }
    }
}