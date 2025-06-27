namespace CleanArchitecture.Domain.Vehiculos;

public record Direccion(
     string? Calle,
     string? Ciudad,
     string? Provincia,
     string? Departamento,
     string Pais
);