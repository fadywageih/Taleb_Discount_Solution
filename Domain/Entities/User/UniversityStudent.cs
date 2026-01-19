    namespace Domain.Entities.User
    {
        public class UniversityStudent : BaseEntity<Guid>
        {
            public string UniversityName { get; set; }
            public string Faculty { get; set; }
            public string UniversityEmail { get; set; }
            public string NationalIdImagePath { get; set; }
            public int Level { get; set; }
            public Guid UserId { get; set; }
            public ApplicationUser User { get; set; }
        }
    }
