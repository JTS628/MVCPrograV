using Google.Cloud.Firestore;
using PrograV.Firebase;
using PrograV.Models;

namespace PrograV.Models
{
    public class Condominium
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Count { get; set; }
        public string Photo { get; set; }
        
    }


    public class CondominiumHelper 
    {

        public async Task<List<Condominium>> getCondominiums()
        {
            List<Condominium> condominiumList = new List<Condominium>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("condominium");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                condominiumList.Add(new Condominium
                {
                    Name = data["Name"].ToString(),
                    Address = data["Address"].ToString(),
                    Count = Convert.ToInt32(data["Count"]),
                    Photo = data["Photo"].ToString(),
                    Id = item.Id

                });
            
            }

            return condominiumList;
        }

        public async Task<bool> saveCondominium(Condominium condominium)
        {
            try
            {    // ejmplo paso a paso de lo que hace el add

                //FirestoreDb bd = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                //CollectionReference coll = bd.Collection("condominium");
                //Dictionary<string, object> newCondo = new Dictionary<string, object>
                //{{
                //        "Name",condominium.Name },
                //        {"Address",condominium.Address },
                //        {"Count",condominium.Count },
                //        {"Photo",condominium.Photo },
                //};

                //await coll.AddAsync(newCondo);


                //Que hace?: FirestoreDB conecta con firebase. bd.collection direcciona a la coleccion. En el Collection reference se guarda lo que esta en la coleccion de firestore (dicionario en formato Json)
                // ejemplo del paso a paso comentado arriba

                DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("condominium").AddAsync(
                    new Dictionary<string, object>
                    {
                        //esto es un objeto json, por eso va entre parentesis
                        
                        {"Name",condominium.Name },
                        {"Address",condominium.Address },
                        {"Count",condominium.Count },
                        {"Photo",condominium.Photo },
                    });

                return true;
            }
            catch
            { 
            return false;
            }

        }

        public async Task<List<Condominium>> getCondominiumsbycondoName( List<CondoDetails> condoDetail )
        {
            List<Condominium> condominiumList = new List<Condominium>();

           

            foreach (var condo in condoDetail)
            {
                //var condoName = condo;

                Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("condominium").WhereEqualTo("Name", condo.condoname);
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                foreach (var item in querySnapshot)
                {
                    Dictionary<string, object> data = item.ToDictionary();

                    condominiumList.Add(new Condominium
                    {
                        Name = data["Name"].ToString(),
                        Address = data["Address"].ToString(),
                        Count = Convert.ToInt32(data["Count"]),
                        Photo = data["Photo"].ToString(),
                        Id = item.Id

                    });

                }

            }

            return condominiumList;
        }

        public static async Task<bool> UpdateCondotinfo(Condominium condominium)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("condominium").Document(condominium.Id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();


                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {

                        {"Name", condominium.Name },
                        {"Address",condominium.Address },
                        {"Count",condominium.Count },
                        {"Photo",condominium.Photo },
                      
                    };
                    await docRef.UpdateAsync(updates);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                Console.WriteLine($"Error updating records: {ex.Message}");
            }
            return false;
        }

        public static async Task<bool> RemoveCondo(string ID)
        {
            DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("condominium").Document(ID);
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
