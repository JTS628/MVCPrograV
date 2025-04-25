namespace PrograV.Models
{
    public class Visits
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public int  cedula { get; set; }
        public string marcavehiculo { get; set; }
        public string modelo { get; set; }
        public string color { get; set; }
        public string placa { get; set; }
        public string creadopor { get; set; }
        public string correoOrigen { get; set; }

        public DateTime fecha { get; set; }
        


    }

    public class QuickDelivery
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public string creadopor { get; set; }
        public string correoOrigen { get; set; }
        public string correo { get; set; }

        public string code { get; set; }
    }

    public class Vehiculo
    {
        public string id { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string color { get; set; }
        public string placa { get; set; }
        public string creadopor { get; set; }
        public string correoOrigen { get; set; }


    }

}
