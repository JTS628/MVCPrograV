using Google.Cloud.Firestore;
using PrograV.Firebase;
using Firebase.Auth;
using System.Text;
using PrograV.Miselanius;

namespace PrograV.Models
{
    public class UserModel
    {
        public string uuid { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<CondoDetails> condodetail  {get; set;} 

        
    }

    public class CondoDetails
    {
        public string condoname { get; set; }
        public int houseID { get; set; }

    }

   


    public class UserHelper
    {
        public async Task<UserModel> getUser(string email)
        {
         
            var db = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);

            // 1. Check in "User" collection
            Query query = db.Collection("User").WhereEqualTo("email", email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            if (querySnapshot.Documents.Count > 0)
            {
                var data = querySnapshot.Documents[0].ToDictionary();
                return new UserModel
                {
                    email = data["email"].ToString(),
                    name = data["name"].ToString(),
                    type = data["type"].ToString()
                };
            }

            // 2. If not found, check in "officer" collection
            Query queryOfficer = db.Collection("officer").WhereEqualTo("email", email);
            QuerySnapshot querySnapshotofficer = await queryOfficer.GetSnapshotAsync();

            if (querySnapshotofficer.Documents.Count > 0)
            {
                var dataOff = querySnapshotofficer.Documents[0].ToDictionary();
                return new UserModel
                {
                    email = dataOff["email"].ToString(),
                    name = dataOff["name"].ToString(),
                    type = dataOff["type"].ToString()
                };
            }

            // 3. Not found in either collection
            return null;
        }



            //Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").WhereEqualTo("email", email);
            //QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            //Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

            //UserModel user = new UserModel
            //{   

            //    email = data["email"].ToString(),
            //    name = data["name"].ToString(),
            //    type = data["type"].ToString()
                
            //};

            //if (user == null)
            //{

            //    Query queryOfficer = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("officer").WhereEqualTo("email", email);
            //    QuerySnapshot querySnapshotofficer = await queryOfficer.GetSnapshotAsync();

            //    Dictionary<string, object> dataOff = querySnapshot.Documents[0].ToDictionary();

            //    UserModel officer = new UserModel
            //    {

            //        email = dataOff["email"].ToString(),
            //        name = dataOff["name"].ToString(),
            //        type = dataOff["type"].ToString()

            //    };

            //    return officer;

            //}


            //return user };
        

        public static async void postUserWithEmailAndPassword(UserModel userModel,CondoDetails condoDetails,string password)
        {
            UserCredential userCredential = await FirebaseAuthHelper.setFirebaseAuthClient().CreateUserWithEmailAndPasswordAsync(userModel.email, password, userModel.name);

            List<Dictionary<string, object>> objectCondodetail = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "condoname", condoDetails.condoname},
                    { "houseID", condoDetails.houseID}
                }

            };


            DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").AddAsync(
                    new Dictionary<string, object>
                        {
                            {"email", userModel.email },
                            {"name", userModel.name },
                            {"type", userModel.type},
                            {"condo details", objectCondodetail}
                           
                        });

            EmailHelper.SendEmail(userModel,condoDetails, password);
        }


        public class PasswordGenerator
        {
            private static readonly Random _random = new Random();
            private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            private const string Digits = "0123456789";
            private const string SpecialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            public static string GenerateRandomPassword(int length)
            {
                if (length < 4)
                {
                    throw new ArgumentException("Password length should be at least 4 characters.");
                }

                var passwordBuilder = new StringBuilder();
                var characterSet = UppercaseLetters + LowercaseLetters + Digits + SpecialCharacters;

                // Ensure the password contains at least one character from each category
                passwordBuilder.Append(UppercaseLetters[_random.Next(UppercaseLetters.Length)]);
                passwordBuilder.Append(LowercaseLetters[_random.Next(LowercaseLetters.Length)]);
                passwordBuilder.Append(Digits[_random.Next(Digits.Length)]);
                passwordBuilder.Append(SpecialCharacters[_random.Next(SpecialCharacters.Length)]);

                // Fill the rest of the password length with random characters from the combined set
                for (int i = 4; i < length; i++)
                {
                    passwordBuilder.Append(characterSet[_random.Next(characterSet.Length)]);
                }

                // Shuffle the characters to ensure randomness
                var passwordArray = passwordBuilder.ToString().ToCharArray();
                Array.Sort(passwordArray, (a, b) => _random.Next(-1, 2));

                return new string(passwordArray);
            }
        }


        public static async void postofficerWithEmailAndPassword(UserModel userModel, CondoDetails condoDetails, string password)
        {
            UserCredential userCredential = await FirebaseAuthHelper.setFirebaseAuthClient().CreateUserWithEmailAndPasswordAsync(userModel.email, password, userModel.name);

            //List<Dictionary<string, object>> objectCondodetail = new List<Dictionary<string, object>>
            //{
            //    new Dictionary<string, object>
            //    {
            //        { "condoname", condoDetails.condoname},
            //        { "houseID", condoDetails.houseID}
            //    }

            //};


            DocumentReference docRef = await FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("officer").AddAsync(
                    new Dictionary<string, object>
                        {
                            {"email", userModel.email },
                            {"name", userModel.name },
                            {"type", userModel.type},
                            {"condo", condoDetails.condoname}

                            //{"condo details", objectCondodetail}

                        });

            EmailHelper.SendEmail(userModel, condoDetails, password);
        }


    }





}
