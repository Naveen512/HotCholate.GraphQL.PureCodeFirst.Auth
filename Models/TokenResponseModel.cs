namespace GraphQL.PureCodeFirst.Auth.Models
{
    public class TokenResponseModel
    {
        // Message property helps to serve validation errors to client
        public string Message{get;set;}
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}