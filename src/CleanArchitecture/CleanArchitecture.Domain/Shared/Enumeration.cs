namespace CleanArchitecture.Domain.Shared;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
where TEnum : Enumeration<TEnum>
{
    private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumerations();

    private static Dictionary<int, TEnum> CreateEnumerations()
    {
        var enumerationType = typeof(TEnum);

        var fieldsForType = enumerationType.GetFields(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.FlattenHierarchy
        ).Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
        .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);

        return fieldsForType.ToDictionary(e => e.Id);
    }

    public int Id { get; protected init; }
    public string Name { get; protected init; } = string.Empty;

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public bool Equals(Enumeration<TEnum>? other)
    {
        return other is not null && (GetType() == other.GetType() && Id == other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<TEnum> other && Equals(other);
    }

    public static TEnum? FromValue(int id)
    {
        return Enumerations.TryGetValue(id, out TEnum? enumeration)
                                    ? enumeration
                                    : default;
    }

    public static TEnum? FromName(string name)
    {
        return Enumerations.Values.FirstOrDefault(e => e.Name == name);
    }

    public static List<TEnum> GetValues()
    {
        return Enumerations.Values.ToList();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public override string ToString()
    {
        return Name;
    }
}