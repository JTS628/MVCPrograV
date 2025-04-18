using Firebase.Auth;
using Firebase.Auth.Providers;

namespace PrograV.Firebase
{
    public static class FirebaseAuthHelper
    {
        public const string firebaseAppId = "prograv-3a89e";
        public const string firebaseApiKey = "AIzaSyA0XnB5TU7qM6JyE-YD7K-ib5gjBhRxN2I";

        public static FirebaseAuthClient setFirebaseAuthClient()
        {
            var auth = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = firebaseApiKey,
                AuthDomain = $"{firebaseAppId}.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });

            return auth;
        }

    }
}
