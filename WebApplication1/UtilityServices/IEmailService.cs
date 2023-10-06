using WebApplication1.Model;

namespace WebApplication1.UtilityServices
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
