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

        // public bool Verify([Service] IAuthLogic authLogic, string password)
        // {
        //     return authLogic.CheckPassword(password);
        // }
    }
}