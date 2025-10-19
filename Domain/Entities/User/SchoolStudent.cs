namespace Domain.Entities.User
{
    public class SchoolStudent : BaseEntity<Guid>
    {
        public string Address { get; set; }
        public string SchoolName { get; set; }
        public string BirthCertificatePath { get; set; }
        public int Level { get; set; }

        // العلاقة
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
