using Confluent.Kafka;
using PermissionsApp.Domain.Interfaces;
using System.Text.Json;

namespace PermissionsApp.Infraestructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            _config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _topic = topic;
        }

        public async Task ProduceAsync(string operationType)
        {
            var message = new
            {
                Id = Guid.NewGuid().ToString(),
                NameOperation = operationType
            };

            using var producer = new ProducerBuilder<Null, string>(_config).Build();

            await producer.ProduceAsync(_topic, new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message)
            });
        }
    }
}
