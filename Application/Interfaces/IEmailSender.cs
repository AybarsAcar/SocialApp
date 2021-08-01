using System.Threading.Tasks;

namespace Application.Interfaces
{
  public interface IEmailSender
  {
    public Task SendEmailAsync(string userEmail, string emailSubject, string msg);
  }
}