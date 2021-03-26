using GraphQL.PureCodeFirst.Auth.Models;
using GraphQL.PureCodeFirst.InputTypes;

namespace GraphQL.PureCodeFirst.Auth.Logics
{
    public interface IAuthLogic
    {
        string Register(RegisterInputType registerInput);
        TokenResponseModel Login(LoginInputType loginInput);
        TokenResponseModel RenewAccessToken(RenewTokenInputType renewToken);
    }
}