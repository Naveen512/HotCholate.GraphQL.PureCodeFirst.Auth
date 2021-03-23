using GraphQL.PureCodeFirst.InputTypes;

namespace GraphQL.PureCodeFirst.Auth.Logics
{
    public interface IAuthLogic
    {
        string Register(RegisterInputType registerInput);
       string Login(LoginInputType loginInput);
    }
}