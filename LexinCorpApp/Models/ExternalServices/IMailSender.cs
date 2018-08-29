using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ExternalServices
{
    public interface IMailSender
    {
        Task<bool> SendMail(string to, string subject, string body, string htmlBody = "");
    }
}
