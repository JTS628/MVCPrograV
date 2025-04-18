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

            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").WhereEqualTo("email", email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

            UserModel user = new UserModel
            {   

                email = data["email"].ToString(),
                name = data["name"].ToString(),
                type = data["type"].ToString()
                
            };


            return user;
        }

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
                if (length < 8)
                {
                    throw new ArgumentException("Password length should be at least 8 characters.");
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
