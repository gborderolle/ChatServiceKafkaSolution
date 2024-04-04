using Confluent.Kafka;

namespace ChatServiceBackend;
public class KafkaProducer
{
    private readonly string bootstrapServers;
    private readonly string topic;

    public KafkaProducer(string bootstrapServers, string topic)
    {
        this.bootstrapServers = bootstrapServers;
        this.topic = topic;
    }

    public async Task ProduceAsync(string message)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }

}
