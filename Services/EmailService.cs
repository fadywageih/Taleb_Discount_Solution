namespace Services
{
    public class EmailService:IEmailService
    {
        public async Task SendEmailAsync(EmailDto email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("fadywageih14@gmail.com", "phcikrtwerfqetqb");
            await Client.SendMailAsync("fadywageih14@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
