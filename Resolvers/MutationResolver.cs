using GraphQL.PureCodeFirst.Auth.Logics;
using GraphQL.PureCodeFirst.Auth.Models;
using GraphQL.PureCodeFirst.InputTypes;
using HotChocolate;

namespace GraphQL.PureCodeFirst.Auth.Resolvers
{
    public class MutationResolver
    {
        public string Register([Service] IAuthLogic authLogic, RegisterInputType registerInput)
        {
            return authLogic.Register(registerInput);
        }

        public TokenResponseModel Login([Service] IAuthLogic authLogic,LoginInputType loginInput)
        {
            return authLogic.Login(loginInput);
        }

        public TokenResponseModel RenewAccessToken([Service] IAuthLogic authLogic,RenewTokenInputType renewToken)
        {
            return authLogic.RenewAccessToken(renewToken);
        }
    }
}