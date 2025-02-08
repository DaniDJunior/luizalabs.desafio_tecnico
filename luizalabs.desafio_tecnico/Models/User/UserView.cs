namespace luizalabs.desafio_tecnico.Models.User
{
    public class UserView
    {
        public Guid user_id { get; set; }
        public string name { get; set; }

        public UserView() 
        { 
            name = string.Empty;
        }
    }
}
