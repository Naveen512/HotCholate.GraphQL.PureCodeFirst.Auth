using GraphQL.PureCodeFirst.Auth.Logics;
using GraphQL.PureCodeFirst.InputTypes;
using HotChocolate;

namespace GraphQL.PureCodeFirst.Auth.Resolvers
{
    public class QueryResolver
    {
        public string Welcome()
        {
            return "Welcome To Custom Authentication Servies In GraphQL In Pure Code First";
        }
    }
}