namespace Domain.Entities.transcation
{
    public enum TransactionStatus
    {
        Pending,    // في انتظار موافقة الـ Vendor
        Accepted,   // قبلها الـ Vendor
        Rejected,   // رفضها الـ Vendor
        Completed   // اكتملت (يمكن تضيفها للمستقبل)
    }
}
