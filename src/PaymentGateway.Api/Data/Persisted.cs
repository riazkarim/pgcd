namespace PaymentGateway.Api.Data;

public class Persisted<K, V>
{
    public Persisted(K id, V value)
    {
        Id = id;
        Value = value;
    }

    public K Id { get; }
    public V Value { get; }
}