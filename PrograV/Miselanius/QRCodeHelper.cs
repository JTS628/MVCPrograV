using Google.Cloud.Firestore;
using PrograV.Firebase;
using PrograV.Models;
using QRCoder;
using System.Drawing;

namespace PrograV.Miselanius
{
  
    public class QRCodeHelper
    {
        public static string GenerateQRCode(Visits visit)
        {

            // Genera el password Random)

            UserHelper.PasswordGenerator password = new UserHelper.PasswordGenerator();

            string content =  $"Autorizado {visit.nombre} {visit.cedula} {visit.marcavehiculo} {visit.modelo} {visit.placa}"; //UserHelper.PasswordGenerator.GenerateRandomPassword(10);

            // Direccion donde se va a guardar la imagen de QR
            string path = "wwwroot/qr/" + content + ".png";

            // se crea la instancia de QR
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Se crea el QR usando la info predefinida
            QRCodeData data = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // se crea el objeto QR
            QRCode qrCode = new QRCode(data);

            // Generate the graphic (image) from the QR Code data
            using (Bitmap bit = qrCode.GetGraphic(20)) // 20 is the pixel size
            {
                // Save the generated QR code to the file path
                bit.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }

            // Return the relative path (removes "wwwroot" from the path)
            return path.Replace("wwwroot", string.Empty);
        }

        public static string GenerateQRCodeQuickDelivry(QuickDelivery quickDelivery)
        {

            // Genera el password Random)

            UserHelper.PasswordGenerator password = new UserHelper.PasswordGenerator();

            string content = $"Autorizado {quickDelivery.nombre}   {quickDelivery.descripcion}"; //UserHelper.PasswordGenerator.GenerateRandomPassword(10);

            // Direccion donde se va a guardar la imagen de QR
            string path = "wwwroot/qr/" + content + ".png";

            // se crea la instancia de QR
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // Se crea el QR usando la info predefinida
            QRCodeData data = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // se crea el objeto QR
            QRCode qrCode = new QRCode(data);

            // Generate the graphic (image) from the QR Code data
            using (Bitmap bit = qrCode.GetGraphic(20)) // 20 is the pixel size
            {
                // Save the generated QR code to the file path
                bit.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }

            // Return the relative path (removes "wwwroot" from the path)
            return path.Replace("wwwroot", string.Empty);
        }

        public static async Task<List<QuickDelivery>> Getcode()
        {
            List<QuickDelivery> codeList = new List<QuickDelivery>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("QuickDelivery");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                codeList.Add(new QuickDelivery
                {

                    code = data["code"].ToString(),
                    nombre = data["nombre"].ToString(),
                    descripcion = data["descripcion"].ToString(),
                    creadopor = data["creadopor"].ToString(),
                                    
                   

                });

            }

            return codeList;
        }



    }
}
