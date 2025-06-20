using Bogus;
using Bogus.DataSets;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Domain.Vehiculos;
using Dapper;
using Npgsql.Replication;

namespace CleanArchitecture.Api.Extensions;

public static class SeedDataExtension
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.CreateConnection();

        var faker = new Faker();

        List<object> vehiculos = new();

        for (int i = 0; i < 100; i++)
        {
            vehiculos.Add(new
            {
                id = Guid.NewGuid(),
                vin = faker.Vehicle.Vin(),
                modelo = faker.Vehicle.Model(),
                pais = faker.Address.Country(),
                departamento = faker.Address.State(),
                provincia = faker.Address.County(),
                ciudad = faker.Address.City(),
                calle = faker.Address.StreetName(),
                precioMonto = faker.Random.Decimal(1000, 20000),
                precioTipoMoneda = "USD",
                mantenimientoMonto = faker.Random.Decimal(100, 200),
                mantenimientoTipoMoneda = "USD",
                accesorios = new List<int> { (int)Accesorio.Wifi, (int)Accesorio.AppleCar },
                fechaUltimoAlquiler = DateTime.MinValue
            });
        }

        const string sql = """
                INSERT INTO public.vehiculos
                (
                    id, vin, modelo, direccion_pais, direccion_departamento, direccion_provincia, direccion_ciudad, direccion_calle,
                    precio_monto, precio_tipo_moneda, mantenimiento_monto, mantenimiento_tipo_moneda, accesorios,fecha_ultimo_alquiler 
                )
                values (@id, @vin, @modelo, @pais, @departamento, @provincia, @ciudad, @calle, @precioMonto, @precioTipoMoneda, @mantenimientoMonto, @mantenimientoTipoMoneda, @accesorios, @fechaUltimoAlquiler)
        """;

        connection.Execute(sql, vehiculos);


    }
}