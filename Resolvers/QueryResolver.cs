using GraphQL.PureCodeFirst.Auth.Logics;
using GraphQL.PureCodeFirst.InputTypes;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQL.PureCodeFirst.Auth.Resolvers
{
    public class QueryResolver
    {
        //[Authorize(Roles= new[] {"admin","super-admin"})]
        //[Authorize(Policy="roles-policy")]
        //[Authorize(Policy="claim-policy-1")]
        [Authorize(Policy="claim-policy-2")]
        public string Welcome()
        {
            return "Welcome To Custom Authentication Servies In GraphQL In Pure Code First";
        }
    }
}