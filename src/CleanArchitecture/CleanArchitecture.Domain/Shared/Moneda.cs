namespace CleanArchitecture.Domain.Shared;

public record Moneda(decimal Monto, TipoMoneda TipoMoneda)
{
    public static Moneda operator +(Moneda m1, Moneda m2)
    {
        if (m1.TipoMoneda.Codigo != m2.TipoMoneda.Codigo)
        {
            throw new InvalidOperationException("Cannot add Monedas with different TipoMoneda.");
        }

        return new Moneda(m1.Monto + m2.Monto, m1.TipoMoneda);
    }

    public static Moneda Zero()
    {
        return new Moneda(0, TipoMoneda.None);
    }

    public static Moneda Zero(TipoMoneda tipoMoneda = null!)
    {
        return new Moneda(0, tipoMoneda ?? TipoMoneda.None);
    }

    public bool IsZero()
    {
        return this == Zero(TipoMoneda);
    }
}