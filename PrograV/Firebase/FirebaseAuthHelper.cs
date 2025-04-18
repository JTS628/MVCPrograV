using Firebase.Auth;
using Firebase.Auth.Providers;

namespace PrograV.Firebase
{
    public static class FirebaseAuthHelper
    {
        public const string firebaseAppId = "";
        public const string firebaseApiKey = "";

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
