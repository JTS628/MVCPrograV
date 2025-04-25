using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using PrograV.Firebase;
using PrograV.Models;

namespace PrograV.Miselanius
{
    public class OfficerHelper
    {

        public static async Task<List<Security.OfficerModel>> SearchOfficers()
        {
            List<Security.OfficerModel> officerList = new List<Security.OfficerModel>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("officer").WhereEqualTo("type", "officer");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();


            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                officerList.Add(new Security.OfficerModel
                {
                    uuid = item.Id,
                    name = data["name"].ToString(),
                    email = data["email"].ToString(),
                    type = data["type"].ToString(),
                    condo = data["condo"].ToString(),


                });

            }

            return officerList;
        }


        public static async Task<Security.OfficerModel> SearchOfficerByEmail(string email)
        {
            Security.OfficerModel officer = null;

            // Create a query to find a document with the specified email
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                .Collection("officer")
                .WhereEqualTo("type", "officer")
                .WhereEqualTo("email", email);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            // Loop through the query results (should be at most 1 item if email is unique)
            foreach (var item in querySnapshot.Documents)
            {
                Dictionary<string, object> data = item.ToDictionary();

                //List<CondoDetails> objectCondodetail = new List<CondoDetails>();

                //// Check and extract condo details if they exist
                //if (data.ContainsKey("condo details"))
                //{
                //    var condoDetailsArray = (List<object>)data["condo details"];
                //    foreach (var condoDetail in condoDetailsArray)
                //    {
                //        var condoDetailMap = (Dictionary<string, object>)condoDetail;
                //        objectCondodetail.Add(new CondoDetails
                //        {
                //            condoname = condoDetailMap["condoname"].ToString(),
                //            houseID = Convert.ToInt16(condoDetailMap["houseID"])
                //        });
                //    }
                //}

                // Populate the owner object with the retrieved data
                officer = new Security.OfficerModel
                {
                    uuid = item.Id,
                    email = data["email"].ToString(),
                    name = data["name"].ToString(),
                    type = data["type"].ToString(),
                    condo = data["condo"].ToString(),
                };
            }

            return officer; // Returns null if no owner found
        }

        public static async Task UpdateOfficerDetails(string ownerName, string email, string condoName, string uuid)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("officer").Document(uuid);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();

                 
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "name", ownerName },
                        { "condo", condoName }
                    };
                    await docRef.UpdateAsync(updates);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating records: {ex.Message}");
            }
        }

        public static async Task RemoveOfficer(string email, string uuid)
        {
            try
            {
               
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("officer").Document(uuid);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    await docRef.DeleteAsync();
                    Console.WriteLine("Document deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Document not found.");
                }

                // Step 2: Eliminar tambien de Firebase Authentication
                try
                {
                  
                    FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;

                    // Lookup user by email to get UID
                    UserRecord user = await firebaseAuth.GetUserByEmailAsync(email);
                    string uid = user.Uid;

                    // Now delete using UID
                    await firebaseAuth.DeleteUserAsync(uid);
                    Console.WriteLine("Officer's Firebase Authentication account deleted successfully.");
                }
                catch (FirebaseAuthException authEx)
                {
                    Console.WriteLine($"Error deleting Firebase Authentication account: {authEx.Message}");
                }


            //string userId = email; 

            //try
            //{
            //    FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
            //    await firebaseAuth.DeleteUserAsync(userId);
            //    Console.WriteLine("Officer's Firebase Authentication account deleted successfully.");
            //}
            //catch (FirebaseAuthException authEx)
            //{
            //    Console.WriteLine($"Error deleting Firebase Authentication account: {authEx.Message}");
            //}

        }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting document: {ex.Message}");
            }

        }







    }
}






