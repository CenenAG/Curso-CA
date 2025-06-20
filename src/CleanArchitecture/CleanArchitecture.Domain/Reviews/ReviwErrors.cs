using CleanArchitecture.Domain.Abstractions;

namespace CleanArchitecture.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotElegible = new Error("Review.NotElegible", "Alquiler todavia no ha terminado");
    public static readonly Error InvalidCreationDate = new Error("Review.InvalidCreationDate", "La fecha de creación no puede ser futura");
}
