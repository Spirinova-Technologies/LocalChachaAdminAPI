using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace LocalChachaAdminApi.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration configuration;

        public NotificationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> SendEmail(Email email)
        {
            var apiKey = configuration["SendGrid:EmailApiKey"];
            var emailFrom = new EmailAddress(configuration["SendGrid:FromEmailId"], configuration["SendGrid:FromEmailUsername"]);

            var client = new SendGridClient(apiKey);
            var tos = email.Tos.Select(to =>
                 new EmailAddress(to)
            ).ToList();

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(emailFrom, tos, email.Subject, email.Body, email.Body);
            var response = await client.SendEmailAsync(msg);

            return true;
        }

        public Task<bool> SendSms()
        {
             string accountSid = configuration["SendGrid:SmsAccountSid"];
             string authToken = configuration["SendGrid:SmsAuthToken"];

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Hi there!",
                from: new Twilio.Types.PhoneNumber("+15005550006"),
                to: new Twilio.Types.PhoneNumber("+918605633693")
            );

            Console.WriteLine(message.Sid);

            return null;
        }
    }
}