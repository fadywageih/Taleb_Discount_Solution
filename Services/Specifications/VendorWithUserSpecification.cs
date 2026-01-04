namespace Services.Specifications
{
    public class VendorWithUserSpecification : Specifications<Vendor>
    {
        public VendorWithUserSpecification()
            : base(null)
        {
            AddInclude(v => v.User);
        }
        public VendorWithUserSpecification(Guid vendorId)
            : base(v => v.Id == vendorId)
        {
            AddInclude(v => v.User);
        }
        public VendorWithUserSpecification(Expression<Func<Vendor, bool>> criteria)
            : base(criteria)
        {
            AddInclude(v => v.User);
        }
    }
}