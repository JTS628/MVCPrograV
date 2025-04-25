using Google.Cloud.Firestore;
using PrograV.Firebase;

namespace PrograV.Models
{
    public class Security
    {

        public class OfficerModel
        {
            public string uuid { get; set; }

            public string name { get; set; }
            public string email { get; set; }
            public string type { get; set; }
            public string condo { get; set; }


        }

        public class OfficerHelper
        {
            public static async Task<List<Visits>> OfficerViewVisits()
            {
                List<Visits> visitList = new List<Visits>();

                Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("visitas");
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                foreach (var item in querySnapshot)
                {
                    Dictionary<string, object> data = item.ToDictionary();

                    visitList.Add(new Visits
                    {
                        correoOrigen = data["correoOrigen"].ToString(),
                        creadopor = data["creadopor"].ToString(),

                        nombre = data["nombre"].ToString(),
                        cedula = Convert.ToInt32(data["cedula"]),
                        correo = data["correo"].ToString(),
                        marcavehiculo = data["marcavehiculo"].ToString(),
                        modelo = data["modelo"].ToString(),
                        color = data["color"].ToString(),
                        placa = data["placa"].ToString(),
                        fecha = ((Timestamp)data["fecha"]).ToDateTime().ToUniversalTime(),
                        id = item.Id,

                    });

                }

                return visitList;
            }

            public static async Task<bool> officerSavevisit(Visits owner, UserModel user)
            {
                try
                {
                    DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("visitas").AddAsync(
                        new Dictionary<string, object>
                        {
                        //esto es un objeto json, por eso va entre parentesis
                        {"nombre",owner.nombre },
                        {"cedula",owner.cedula },
                        {"marcavehiculo",owner.marcavehiculo },
                        {"modelo",owner.modelo },
                        {"correo",owner.correo },
                        {"color",owner.color },
                        {"placa",owner.placa },
                        {"fecha",owner.fecha },
                        {"creadopor",user.name },
                        {"correoOrigen",user.email},


                        });

                    return true;
                }
                catch
                {
                    return false;
                }

            }


            public static async Task<bool> OfficerRemoveVisit(string ID)
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("visitas").Document(ID);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    await docRef.DeleteAsync();
                    Console.WriteLine("Document deleted successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Document not found.");
                }

                return false;
            }


        }

        



    }
}
