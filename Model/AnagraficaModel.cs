using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPExamProject.Model
{
    public class AnagraficaModel
    {
        public AnagraficaModel() { }
        public AnagraficaModel(string enrollment, string fullName, string email, string phone)
        {
            this.Enrollment = enrollment;
            this.FullName = fullName;
            this.Email = email;
            this.Phone = phone;
        }

        public string Enrollment { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}