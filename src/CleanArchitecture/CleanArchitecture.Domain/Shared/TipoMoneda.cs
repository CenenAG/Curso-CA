namespace CleanArchitecture.Domain.Shared;

public record TipoMoneda
{
    public static readonly TipoMoneda Usd = new("USD");
    public static readonly TipoMoneda Eur = new("EUR");
    public static readonly TipoMoneda None = new("");

    public string? Codigo { get; init; }

    public TipoMoneda(string codigo)
    {
        Codigo = codigo;
    }

    public static readonly IReadOnlyCollection<TipoMoneda> All = new List<TipoMoneda>
    {
        Usd,
        Eur,
    };

    public static TipoMoneda FromCodigo(string codigo)
    {
        return All.FirstOrDefault(tm => tm.Codigo == codigo) ??
                throw new ArgumentException($"TipoMoneda with codigo '{codigo}' does not exist.");
    }

}