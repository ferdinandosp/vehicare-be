using HotChocolate;
using Vehicare.API.Models;
using Vehicare.API.GraphQL.Inputs;

namespace Vehicare.API.GraphQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(u => u.Id).Type<NonNullType<IntType>>();
        descriptor.Field(u => u.Email).Type<NonNullType<StringType>>();
        descriptor.Field(u => u.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(u => u.LastName).Type<NonNullType<StringType>>();
        descriptor.Field(u => u.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(u => u.LastLoginAt).Type<DateTimeType>();
        descriptor.Field(u => u.IsActive).Type<NonNullType<BooleanType>>();

        // Exclude sensitive fields
        descriptor.Field(u => u.PasswordHash).Ignore();
    }
}

public class LoginInputType : InputObjectType<LoginInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<LoginInput> descriptor)
    {
        descriptor.Field(l => l.Email).Type<NonNullType<StringType>>();
        descriptor.Field(l => l.Password).Type<NonNullType<StringType>>();
    }
}

public class RegisterInputType : InputObjectType<RegisterInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<RegisterInput> descriptor)
    {
        descriptor.Field(r => r.Email).Type<NonNullType<StringType>>();
        descriptor.Field(r => r.Password).Type<NonNullType<StringType>>();
        descriptor.Field(r => r.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(r => r.LastName).Type<NonNullType<StringType>>();
    }
}

public class LoginPayloadType : ObjectType<LoginPayload>
{
    protected override void Configure(IObjectTypeDescriptor<LoginPayload> descriptor)
    {
        descriptor.Field(l => l.Token).Type<StringType>();
        descriptor.Field(l => l.User).Type<UserType>();
        descriptor.Field(l => l.Error).Type<StringType>();
        descriptor.Field(l => l.Success).Type<NonNullType<BooleanType>>();
    }
}

public class RegisterPayloadType : ObjectType<RegisterPayload>
{
    protected override void Configure(IObjectTypeDescriptor<RegisterPayload> descriptor)
    {
        descriptor.Field(r => r.User).Type<UserType>();
        descriptor.Field(r => r.Error).Type<StringType>();
        descriptor.Field(r => r.Success).Type<NonNullType<BooleanType>>();
    }
}
