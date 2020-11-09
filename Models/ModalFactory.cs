using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kurs.Models
{
    public class ModalFactory
    {
        public StudentClassesModal create(StudentClass studentClass)
        {
            return new StudentClassesModal
            {
               
                UserID = studentClass.UserID,
                Student_Name = studentClass.User.Name,
                Grade = null,
      

              
    
    
    };

        }



    }
}