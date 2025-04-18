namespace PermissionsApp.Domain.Interfaces
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string operationType);
    }
}
