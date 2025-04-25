using System.Net.Mail;
using System.Net;
using PrograV.Models;
using Google.Cloud.Firestore;
using PrograV.Firebase;
using Firebase.Auth;
using System.Net.Mime;

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
                    body = body.Replace("{password}", password);
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

        public static void SendEmailtoVisit(Visits visit)
        {
            try
            {
                string QRimg = QRCodeHelper.GenerateQRCode(visit); // e.g. "/qr/filename.png"
                string qrFullPath = "wwwroot" + QRimg; // full server path to the image

                string sender = "jamcrests@gmail.com";
                string senderPwd = "qtef ioxa fzbn lmas";

                using (MailMessage mm = new MailMessage(sender, visit.correo))
                {
                    mm.Subject = "Visita programada a condominio";
                    mm.IsBodyHtml = true;

                    string bodyHtml;

                    using (var sr = new StreamReader("wwwroot/templates/visitaprogramada.html"))
                    {
                        bodyHtml = sr.ReadToEnd()
                            .Replace("{userName}", visit.nombre)
                            .Replace("{email}", visit.correo)
                            .Replace("{qrImageUrl}", "cid:qrCodeImage"); // CID for embedding
                    }

                    // Create alternate view for HTML email
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(bodyHtml, null, MediaTypeNames.Text.Html);

                    // Attach the QR code image as a LinkedResource
                    LinkedResource qrImgRes = new LinkedResource(qrFullPath, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = "qrCodeImage",
                        TransferEncoding = TransferEncoding.Base64,
                        ContentType = new ContentType("image/png") // adjust if you're using png
                    };

                    avHtml.LinkedResources.Add(qrImgRes);
                    mm.AlternateViews.Add(avHtml);

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(sender, senderPwd);
                        smtp.Send(mm);



                        //string QRimg = QRCodeHelper.GenerateQRCode(visit);

                        //string sender = "jamcrests@gmail.com";
                        //string senderPwd = "qtef ioxa fzbn lmas";


                        //using (MailMessage mm = new MailMessage(sender, visit.correo))
                        //{
                        //    mm.Subject = "Visita programada a condominio";
                        //    mm.IsBodyHtml = true;

                        //    using (var sr = new StreamReader("wwwroot/templates/visitaprogramada.html"))
                        //    {
                        //        string body = sr.ReadToEnd().Replace("{userName}", visit.nombre);
                        //        body = body.Replace("{email}", visit.correo);
                        //        body = body.Replace("{qrImageUrl}", QRimg);
                        //        //body = body.Replace("{condominiumName}", condoDetails.condoname);
                        //        //body = body.Replace("{houseNumber}", condoDetails.houseID.ToString());
                        //        mm.Body = body;
                        //    }

                        //    SmtpClient smtp = new SmtpClient();
                        //    smtp.Host = "smtp.gmail.com";
                        //    smtp.EnableSsl = true;
                        //    NetworkCredential NetworkCred = new NetworkCredential(sender, senderPwd);
                        //    smtp.UseDefaultCredentials = false;
                        //    smtp.Credentials = NetworkCred;
                        //    smtp.Port = 587;
                        //    smtp.Send(mm);
                    }
                }


            }
            catch { }
               
        }

        public static void SendEmailtoQuickPass(QuickDelivery quickDelivery)
        {
            try
            {
                
                


                string QRimg = QRCodeHelper.GenerateQRCodeQuickDelivry(quickDelivery); // e.g. "/qr/filename.png"
                string qrFullPath = "wwwroot" + QRimg; // full server path to the image

                string sender = "jamcrests@gmail.com";
                string senderPwd = "qtef ioxa fzbn lmas";

                using (MailMessage mm = new MailMessage(sender, quickDelivery.correo))
                {
                    mm.Subject = "Visita programada a condominio";
                    mm.IsBodyHtml = true;

                    string bodyHtml;

                    using (var sr = new StreamReader("wwwroot/templates/quickpass.html"))
                    {
                        bodyHtml = sr.ReadToEnd()
                            
                            .Replace("{email}", quickDelivery.correo)
                            .Replace("{qrImageUrl}", "cid:qrCodeImage") // CID for embedding
                            .Replace("{accessCode}", quickDelivery.code);
                    }

                    // Create alternate view for HTML email
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString(bodyHtml, null, MediaTypeNames.Text.Html);

                    // Attach the QR code image as a LinkedResource
                    LinkedResource qrImgRes = new LinkedResource(qrFullPath, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = "qrCodeImage",
                        TransferEncoding = TransferEncoding.Base64,
                        ContentType = new ContentType("image/png") // adjust if you're using png
                    };

                    avHtml.LinkedResources.Add(qrImgRes);
                    mm.AlternateViews.Add(avHtml);

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(sender, senderPwd);
                        smtp.Send(mm);

                    }
                }


            }
            catch { }

        }


    }
}





