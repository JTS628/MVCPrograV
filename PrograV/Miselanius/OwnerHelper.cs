using Firebase.Auth;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using PrograV.Firebase;
using PrograV.Models;
using System;
using System.Security.Cryptography;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace PrograV.Miselanius
{
    public static class OwnerHelper
    {
        public static async Task<List<UserModel>> SearchOwner()
        {
            List<UserModel> ownerList = new List<UserModel>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").WhereEqualTo("type", "owner");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot.Documents)
            {
                Dictionary<string, object> data = item.ToDictionary();

                List<CondoDetails> objectCondodetail = new List<CondoDetails>();

                if (data.ContainsKey("condo details"))
                {
                    var condoDetailsArray = (List<object>)data["condo details"];
                    foreach (var condoDetail in condoDetailsArray)
                    {
                        var condoDetailMap = (Dictionary<string, object>)condoDetail;
                        objectCondodetail.Add(new CondoDetails
                        {
                            condoname = condoDetailMap["condoname"].ToString(),
                            houseID = Convert.ToInt16(condoDetailMap["houseID"])

                        });
                    }
                }

                ownerList.Add(new UserModel
                {
                    uuid = item.Id,
                    email = data["email"].ToString(),
                    name = data["name"].ToString(),
                    type = data["type"].ToString(),
                    condodetail = objectCondodetail,

                });

            }

            return ownerList;
        }

        public static async Task<UserModel> SearchOwnerByEmail(string email)
        {
            UserModel owner = null;

            // Create a query to find a document with the specified email
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId)
                .Collection("User")
                .WhereEqualTo("type", "owner")
                .WhereEqualTo("email", email);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            // Loop through the query results (should be at most 1 item if email is unique)
            foreach (var item in querySnapshot.Documents)
            {
                Dictionary<string, object> data = item.ToDictionary();

                List<CondoDetails> objectCondodetail = new List<CondoDetails>();

                // Check and extract condo details if they exist
                if (data.ContainsKey("condo details"))
                {
                    var condoDetailsArray = (List<object>)data["condo details"];
                    foreach (var condoDetail in condoDetailsArray)
                    {
                        var condoDetailMap = (Dictionary<string, object>)condoDetail;
                        objectCondodetail.Add(new CondoDetails
                        {
                            condoname = condoDetailMap["condoname"].ToString(),
                            houseID = Convert.ToInt16(condoDetailMap["houseID"])
                        });
                    }
                }

                // Populate the owner object with the retrieved data
                owner = new UserModel
                {
                    uuid = item.Id,
                    email = data["email"].ToString(),
                    name = data["name"].ToString(),
                    type = data["type"].ToString(),
                    condodetail = objectCondodetail,
                };
            }

            return owner; // Returns null if no owner found
        }

        public static async Task<bool> UpdateOwner(string ownerName, string email, string condoName, int houseID)
        {
            try
            {
                // crea la coneccion en firebase
                FirestoreDb db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);


                // busca en firebase por correo
                Query query = db.Collection("User").WhereEqualTo("email", email);
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                // verifica que exista algo en firebase
                if (querySnapshot.Count > 0)
                {
                    foreach (DocumentSnapshot snapshot in querySnapshot.Documents)
                    {
                        object condoObj = snapshot.GetValue<object>("saldo");

                        //// obtiene el saldo el forma de objeto
                        //int currentSaldo = condoObj is int ? (int)condoObj : Convert.ToInt32(condoObj);

                        // convierte el objeto a int
                        int newhouseID = houseID;

                        // diccionario con el cambio
                        Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                        {
                            { "houseID", newhouseID }
                        };

                        // actualiza en firebase
                        await snapshot.Reference.UpdateAsync(dataToUpdate);
                    }
                    return true;
                }
                else
                {
                    // manejo de error
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task UpdateUserDetails(string ownerName, string email, string CondoName, int newHouseID, string uuid)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").Document(uuid);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();

                    // Safely parse the "condo details" array
                    var condoDetailsRaw = documentData["condo details"] as IEnumerable<object>;
                    List<Dictionary<string, object>> condoDetails = new List<Dictionary<string, object>>();

                    foreach (var item in condoDetailsRaw)
                    {
                        // Convert each map to a dictionary
                        condoDetails.Add(item as Dictionary<string, object>);
                    }

                    // Step 3: Find and update the specific object in the array
                    bool isUpdated = false;
                    for (int i = 0; i < condoDetails.Count; i++)
                    {
                        if (condoDetails[i].ContainsKey("condoname") && condoDetails[i]["condoname"].ToString() == CondoName)
                        {
                            condoDetails[i]["houseID"] = newHouseID;

                            isUpdated = true;
                            break;
                        }
                    }

                    // Step 4: Add a new object if no matching object is found
                    if (!isUpdated)
                    {
                        condoDetails.Add(new Dictionary<string, object>
                    {
                        { "condoname", CondoName },
                        { "houseID", newHouseID }
                        });
                    }

                    // Step 5: Update the document in Firestore
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "name", ownerName },
                        { "condo details", condoDetails }
                    };
                    await docRef.UpdateAsync(updates);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating records: {ex.Message}");
            }

        }

        public static async Task AddNewProperty(string ownerName, string email, string CondoName, int newHouseID, string uuid)
        {

            try
            {
                Dictionary<string, object> objectProperties = new Dictionary<string, object>
                {
                    { "condoname", CondoName },
                    { "houseID", newHouseID }
                };

                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").Document(uuid);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    {"name", ownerName },
                    {"condo details", FieldValue.ArrayUnion(objectProperties) }
                };

                WriteResult result = await docRef.UpdateAsync(dataToUpdate);
            }
            catch
            {

            }

        }

        public static async Task<bool> RemoveProperty(string ownerName, string email, string CondoName, int newHouseID, string uuid)
        {
            try
            {
                var ownerList = await OwnerHelper.SearchOwnerByEmail(email);

                if (ownerList.condodetail.Count <= 1)
                     {
                         await RemoveOwnerIfnoproperties(email);
                         await RemoveUser(uuid);
                    return true;
                     }
                else
                {
                    Dictionary<string, object> objectProperties = new Dictionary<string, object>
                        {
                            { "condoname", CondoName },
                            { "houseID", newHouseID }
                        };

                    DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").Document(uuid);
                    Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                        {
                            {"condo details", FieldValue.ArrayRemove(objectProperties) }
                        };

                    WriteResult result = await docRef.UpdateAsync(dataToUpdate);
                    return true;
                }


            }
            catch
            {
                return false;
            }
        }

        public static async Task RemoveOwnerIfnoproperties(string email)
        {
            try
            {
                Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").WhereEqualTo("email", email);
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                if (querySnapshot.Count == 1)
                {
                    FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;

                    UserRecord user = await firebaseAuth.GetUserByEmailAsync(email);
                    string uid = user.Uid;

                    // Now delete using UID
                    await firebaseAuth.DeleteUserAsync(uid);
                    Console.WriteLine("Officer's Firebase Authentication account deleted successfully.");


                }

            }
            catch
            {

            }
        }


        public static async Task<bool> Savevisit(Visits owner, UserModel user)
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

        public static async Task<List<Visits>> ViewVisits(UserModel user)
        {
            List<Visits> visitList = new List<Visits>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("visitas").WhereEqualTo("correoOrigen", user.email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                visitList.Add(new Visits
                {
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


        public static async Task<bool> RemoveVisit(string ID)
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

        public static async Task<bool> UpdateVisitinfo(Visits visit)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("visitas").Document(visit.id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();


                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {

                        {"nombre",visit.nombre },
                        {"cedula",visit.cedula },
                        {"marcavehiculo",visit.marcavehiculo },
                        {"modelo",visit.modelo },
                        {"color",visit.color },
                        {"placa",visit.placa },
                        {"fecha",visit.fecha },

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

        public static async Task<bool> Savefavorito(Visits owner, UserModel user)
        {
            try
            {
                DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Favoritos").AddAsync(
                    new Dictionary<string, object>
                    {
                        //esto es un objeto json, por eso va entre parentesis
                        {"nombre",owner.nombre },
                        {"cedula",owner.cedula },
                        {"marcavehiculo",owner.marcavehiculo },
                        {"modelo",owner.modelo },
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

        public static async Task<List<Visits>> ViewFavorito(UserModel user)
        {
            List<Visits> visitList = new List<Visits>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Favoritos").WhereEqualTo("correoOrigen", user.email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                visitList.Add(new Visits
                {
                    nombre = data["nombre"].ToString(),
                    cedula = Convert.ToInt32(data["cedula"]),
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


        public static async Task<bool> UpdateFavoritoinfo(Visits visit)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Favoritos").Document(visit.id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();


                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {

                        {"nombre",visit.nombre },
                        {"cedula",visit.cedula },
                        {"marcavehiculo",visit.marcavehiculo },
                        {"modelo",visit.modelo },
                        {"color",visit.color },
                        {"placa",visit.placa },
                        {"fecha",visit.fecha },

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

        public static async Task<bool> RemoveFavorito(string ID)
        {
            DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("Favoritos").Document(ID);
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

            return true;
        }

        public static async Task<bool> RemoveUser(string ID)
        {
            DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").Document(ID);
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

            return true;
        }

        public static async Task<bool> SaveQuickDelivery(QuickDelivery delivery, UserModel user)
        {
            try
            {
                DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("QuickDelivery").AddAsync(
                    new Dictionary<string, object>
                    {
                        //esto es un objeto json, por eso va entre parentesis
                        {"nombre",delivery.nombre },
                        {"descripcion",delivery.descripcion },
                        {"correo",delivery.correo },
                        //{"modelo",owner.modelo },
                        //{"color",owner.color },
                        {"code",delivery.code },
                        {"fecha",delivery.fecha },
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

        public static async Task<bool> SaveMiVehiculo(Vehiculo vehiculo, UserModel user)
        {
            try
            {
                DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("vehiculos").AddAsync(
                    new Dictionary<string, object>
                    {
                        //esto es un objeto json, por eso va entre parentesis
                       
                        {"marca",vehiculo.marca},
                        {"modelo",vehiculo.modelo },
                        {"color",vehiculo.color },
                        {"placa",vehiculo.placa },
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

        public static async Task<List<Vehiculo>> ViewMiVehiculo(UserModel user)
        {
            List<Vehiculo> vehiculeList = new List<Vehiculo>();

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("vehiculos").WhereEqualTo("correoOrigen", user.email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                vehiculeList.Add(new Vehiculo
                {
                    //nombre = data["nombre"].ToString(),
                    //cedula = Convert.ToInt32(data["cedula"]),
                    marca = data["marca"].ToString(),
                    modelo = data["modelo"].ToString(),
                    color = data["color"].ToString(),
                    placa = data["placa"].ToString(),
                    //fecha = ((Timestamp)data["fecha"]).ToDateTime().ToUniversalTime(),
                    id = item.Id,

                });

            }

            return vehiculeList;
        }

        public static async Task<bool> UpdateMiVehiculoinfo(Vehiculo vehiculo)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("vehiculos").Document(vehiculo.id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> documentData = snapshot.ToDictionary();


                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {

                        //{"nombre",visit.nombre },
                        //{"cedula",visit.cedula },
                        {"marca",vehiculo.marca },
                        {"modelo",vehiculo.modelo },
                        {"color",vehiculo.color },
                        {"placa",vehiculo.placa },
                        //{"fecha",vehiculo.fecha },

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

        public static async Task<bool> RemoveVehiculo(string ID)
        {
            DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("vehiculos").Document(ID);
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

            return true;
        }



    }
}
