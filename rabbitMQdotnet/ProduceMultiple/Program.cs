using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();

var channelOpts = new CreateChannelOptions(
    publisherConfirmationsEnabled: true,
    publisherConfirmationTrackingEnabled: true,
    outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(50)
);

using var channel = await connection.CreateChannelAsync(channelOpts);

await channel.QueueDeclareAsync(queue: "q.hello", durable: true, exclusive: false, autoDelete: false, arguments: null);

var properties = new BasicProperties
{
    Persistent = true
};

for (int i = 0; i < 100; i++)
{
    await Task.Delay(100);
    var message = $"MESSAGE: {i + 1}";
    var body = Encoding.UTF8.GetBytes(message);

    try
    {
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "q.hello", true, basicProperties: properties, body: body);
        Console.WriteLine($" [x] Sent {message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [] Failed to send {message}. Error {ex.Message}");
        i--;
    }
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();