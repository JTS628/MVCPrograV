using System.Net.Mail;
using System.Net;
using PrograV.Models;
using Google.Cloud.Firestore;
using PrograV.Firebase;
using Firebase.Auth;

namespace PrograV.Miselanius
{
    public static class EmailHelper
    {
        public static void SendEmail(UserModel userModel, CondoDetails condoDetails, string password)
        {
            string sender = "jamcrests@gmail.com";
            string senderPwd = "qtef ioxa fzbn lmas";

            //string sender = "";
            //string senderPwd = "";

            using (MailMessage mm = new MailMessage(sender, userModel.email))
            {
                mm.Subject = "Bienvenido al Sistema Automatico de Condominios";
                mm.IsBodyHtml = true;

                using (var sr = new StreamReader("wwwroot/templates/newOwner.html"))
                {
                    string body = sr.ReadToEnd().Replace("{userName}", userModel.name);
                    body = body.Replace("{email}", userModel.email);
                    body = body.Replace("{password}",password );
                    body = body.Replace("{condominiumName}", condoDetails.condoname);
                    body = body.Replace("{houseNumber}", condoDetails.houseID.ToString());
                    mm.Body = body;
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(sender, senderPwd);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
    }


}





