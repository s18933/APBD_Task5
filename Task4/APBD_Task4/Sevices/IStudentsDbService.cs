using APBD_Task4.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_Task4.Sevices
{
     public interface IStudentsDbService
    {
        EnrollStudentRequest EnrollStudent(EnrollStudentRequest request);
        EnrollStudentRequest PromoteStudents(EnrollStudentRequest request);
    }
}
