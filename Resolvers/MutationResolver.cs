using GraphQL.PureCodeFirst.Auth.Logics;
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

        public string Login([Service] IAuthLogic authLogic,LoginInputType loginInput)
        {
            return authLogic.Login(loginInput);
        }
    }
}