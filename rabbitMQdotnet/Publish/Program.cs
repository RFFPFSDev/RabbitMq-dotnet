using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "emailmessages", type: ExchangeType.Fanout);

const string message = "message 1";
var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(exchange: "emailmessages", routingKey: string.Empty, body: body);
Console.WriteLine($" [x] Publish '{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();