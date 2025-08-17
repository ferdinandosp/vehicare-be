using HotChocolate;
using Vehicare.API.Models;
using Vehicare.API.GraphQL.Inputs;

namespace Vehicare.API.GraphQL.Types;

public class VehicleType : ObjectType<Vehicle>
{
    protected override void Configure(IObjectTypeDescriptor<Vehicle> descriptor)
    {
        descriptor.Field(v => v.Id).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.UserId).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.Make).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Model).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Year).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.VIN).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.LicensePlate).Type<StringType>();
        descriptor.Field(v => v.CurrentMileage).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.Color).Type<StringType>();
        descriptor.Field(v => v.PurchaseDate).Type<NonNullType<DateTimeType>>();
        descriptor.Field(v => v.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(v => v.UpdatedAt).Type<NonNullType<DateTimeType>>();

        // Navigation properties
        descriptor.Field(v => v.User).Type<UserType>();
    }
}

public class CreateVehicleInputType : InputObjectType<CreateVehicleInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CreateVehicleInput> descriptor)
    {
        descriptor.Field(v => v.Make).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Model).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.Year).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.VIN).Type<NonNullType<StringType>>();
        descriptor.Field(v => v.LicensePlate).Type<StringType>();
        descriptor.Field(v => v.CurrentMileage).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.Color).Type<StringType>();
        descriptor.Field(v => v.PurchaseDate).Type<NonNullType<DateTimeType>>();
    }
}

public class UpdateVehicleInputType : InputObjectType<UpdateVehicleInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<UpdateVehicleInput> descriptor)
    {
        descriptor.Field(v => v.Id).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.Make).Type<StringType>();
        descriptor.Field(v => v.Model).Type<StringType>();
        descriptor.Field(v => v.Year).Type<IntType>();
        descriptor.Field(v => v.VIN).Type<StringType>();
        descriptor.Field(v => v.LicensePlate).Type<StringType>();
        descriptor.Field(v => v.CurrentMileage).Type<IntType>();
        descriptor.Field(v => v.Color).Type<StringType>();
        descriptor.Field(v => v.PurchaseDate).Type<DateTimeType>();
    }
}

public class CreateVehiclePayloadType : ObjectType<CreateVehiclePayload>
{
    protected override void Configure(IObjectTypeDescriptor<CreateVehiclePayload> descriptor)
    {
        descriptor.Field(p => p.Vehicle).Type<VehicleType>();
        descriptor.Field(p => p.Error).Type<StringType>();
        descriptor.Field(p => p.Success).Type<NonNullType<BooleanType>>();
    }
}

public class UpdateVehiclePayloadType : ObjectType<UpdateVehiclePayload>
{
    protected override void Configure(IObjectTypeDescriptor<UpdateVehiclePayload> descriptor)
    {
        descriptor.Field(p => p.Vehicle).Type<VehicleType>();
        descriptor.Field(p => p.Error).Type<StringType>();
        descriptor.Field(p => p.Success).Type<NonNullType<BooleanType>>();
    }
}

public class DeleteVehiclePayloadType : ObjectType<DeleteVehiclePayload>
{
    protected override void Configure(IObjectTypeDescriptor<DeleteVehiclePayload> descriptor)
    {
        descriptor.Field(p => p.Success).Type<NonNullType<BooleanType>>();
        descriptor.Field(p => p.Error).Type<StringType>();
        descriptor.Field(p => p.DeletedVehicleId).Type<IntType>();
    }
}
