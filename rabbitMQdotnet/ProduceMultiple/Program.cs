using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "q.hello", durable: true, exclusive: false, autoDelete: false, arguments: null);

var properties = new BasicProperties
{
    Persistent = true
};

for (int i = 0; i < 1000; i++)
{
    var message = $"MESSAGE: {i}";
    var body = Encoding.UTF8.GetBytes(message);
    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "q.hello", true, basicProperties: properties, body: body);
    Console.WriteLine($" [x] Sent {message}");
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();