using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "emailmessages", type: ExchangeType.Fanout);

var queueDeclareResult = await channel.QueueDeclareAsync(queue: "q.logs", durable: true, exclusive: false, autoDelete: false, arguments: null);

await channel.QueueBindAsync(queue: queueDeclareResult.QueueName, exchange: "emailmessages", routingKey: string.Empty);

Console.WriteLine($" [*] Waiting for messages on queue: {queueDeclareResult.QueueName}");
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Logging {message}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueDeclareResult.QueueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();