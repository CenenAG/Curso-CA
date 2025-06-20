using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Abstractions;

namespace CleanArchitecture.Domain.Vehiculos;

public class VehiculoErrors
{
    public static Error NotFound = new Error("Vehiculo.NotFound", "The vehiculo was not found.");
    public static Error InvalidDateRange = new Error("Vehiculo.InvalidDateRange", "The date range is invalid.");
}