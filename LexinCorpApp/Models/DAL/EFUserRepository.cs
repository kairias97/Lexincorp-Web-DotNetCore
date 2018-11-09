using LexincorpApp.Models.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LexincorpApp.Models
{
    public class EFUserRepository : IUserRepository
    {
        private ApplicationDbContext context;
        private readonly IGuidManager _guidManager;
        private readonly ICryptoManager _cryptoManager;
        private readonly IMailSender _mailSender; 
        public EFUserRepository(ApplicationDbContext ctx,
            IGuidManager guidManager,
            ICryptoManager cryptoManager,
            IMailSender mailSender)
        {
            this.context = ctx;
            _guidManager = guidManager;
            _cryptoManager = cryptoManager;
            _mailSender = mailSender;
        }
        public IQueryable<User> Users { get => context.Users; }

        public void Save(User user)
        {
            if (user.Id == 0)
            {
                context.Users.Add(user);
            } else
            {
                //context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Attach(user).Context.Entry(user).Property(x => x.IsAdmin).IsModified = true;
            }
            context.SaveChanges();
        }

        public bool VerifyUsername(string username)
        {
            return !context.Users.Any(u => u.Username == username);
        }

        public bool VerifyAttorneyIDAndUsername(int attorneyID, int userID)
        {
            return context.Users.Any(u => u.Attorney.Id == attorneyID && u.Id == userID);
        }

        public User GetUserByUsername(string username)
        {
            return context.Users.Include(u => u.Attorney).Where(x => x.Username == username && (bool)x.Active).FirstOrDefault();
        }
        public void UpdateUserPassword(User user)
        {
            context.Update(user);
            context.Entry<User>(user).Property(x => x.Password).IsModified = true;
            context.SaveChanges();
        }

        public void ResetPassword(int attorneyId)
        {
            User user = context.Users
                .Include(u => u.Attorney)
                .Where(u => u.Attorney.Id == attorneyId)
                .First();
            string guidGenerated = _guidManager.GenerateGuid();
            string passwordRestored = guidGenerated.Substring(guidGenerated.Length - 12, 12);
            string passwordHashed = _cryptoManager.HashString(passwordRestored);
            user.Password = passwordHashed;
            context.SaveChanges();
            //Envío de password sin hash al usuario
            string emailBody = $"Se le ha restablecido su clave de acceso a la aplicación Lexincorp Nicaragua Web," +
                $"y su clave de acceso nueva es *{passwordRestored}*. Se recomienda cambiar su clave una vez ingrese nuevamente al sistema." +
                $" \n**Este es un mensaje autogenerado por el sistema, favor no responder**";
            _mailSender.SendMail(user.Attorney.Email, "Contraseña restablecida para aplicación Lexincorp Nicaragua Web", emailBody);
        }
    }
}
