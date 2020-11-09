//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kurs.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Quiz
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quiz()
        {
            this.Notes = new HashSet<Note>();
            this.Questions = new HashSet<Question>();
            this.StudentQuizs = new HashSet<StudentQuiz>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> CourseID { get; set; }
        public Nullable<int> TeacherID { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string StratHour { get; set; }
        public string EndHour { get; set; }
        public int PeriodID { get; set; }
        public Nullable<int> BranchID { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual Cours Cours { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentQuiz> StudentQuizs { get; set; }
    }
}
