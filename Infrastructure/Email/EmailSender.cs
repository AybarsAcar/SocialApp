using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
  /// <summary>
  /// 
  /// </summary>
  public class EmailSender : IEmailSender
  {
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
      _config = config;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userEmail"></param>
    /// <param name="emailSubject"></param>
    /// <param name="msg"></param>
    public async Task SendEmailAsync(string userEmail, string emailSubject, string msg)
    {
      var client = new SendGridClient(_config["SendGrid:Key"]);

      var message = new SendGridMessage
      {
        From = new EmailAddress("oaca0001@student.monash.edu", _config["SendGrid:User"]),
        Subject = emailSubject,
        PlainTextContent = msg,
        HtmlContent = msg,
      };
      
      message.AddTo(new EmailAddress(userEmail));
      message.SetClickTracking(false, false);

      await client.SendEmailAsync(message);
    }
  }
}