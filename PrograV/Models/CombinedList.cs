namespace PrograV.Models
{
    public class CombinedList
    {
        public class VisitWithOwnerViewModel
        {
            public Visits Visit { get; set; }
            public UserModel Owner { get; set; }

            public string CondoName { get; set; }
        }

    }
}
