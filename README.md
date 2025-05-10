# Run RabbitMQ

First, 'docker-compose up' to run the rabbitmq

http://localhost:15672/
guest
guest

# Publish first "Hello word"

## Connection and channels

An AMQP **connection** is a link between the client and the broker that
performs underlying networking tasks, including initial authentication, IP resolution, and
networking

Each AMQP **connection** maintains a set of underlying channels. A channel reuses a
connection, forgoing the need to reauthorize and open a new TCP stream, making it more
resource-efficient.

![connectionandchannels](imgs/connectionandchannels.png)

```cs
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
```

## Create a Queue via code (a queue can also be created via RabbitMQ Management -> http://localhost:15672/#/)

Declaring a queue is idempotent - it will only be created if it doesn't exist already.

```cs
await channel.QueueDeclareAsync(queue: "q.hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);
```

- queue: "q.hello" -> The name of the queue
- durable: false -> Should this queue survive a broker restart
- exclusive: false -> Should this queue use be limited to its declaring connection? Such a queue will be deleted when its declaring connection closes
- autoDelete: false -> Should this queue be auto-deleted when its last consumer (if any) unsubscribes
- arguments: null -> Optional; additional queue arguments, e.g. "x-queue-type"


![createqueue](imgs/createqueue.png)

## Message as a byte array.

```cs
const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
```

## Publish message.

When the code above finishes running, the channel and the connection will be disposed. That's it for our publisher.

**Default Exchange** is a direct exchange with no name (empty string) pre-declared by the broker. It has one special property that makes it very useful for simple applications: every queue that is created is automatically bound to it with a routing key which is the same as the queue name.

```cs
await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "q.hello", body: body);
```

- exchange -> if it is empty, message goes to default-exchange
- routingKey -> for this case, default-exchange, routingKey must match queue name. otherwise, message will not be routed to a queue.
- body -> message

![messagepublished](imgs/messagepublished.png)

## References:

https://medium.com/@deshan.m/6-fantastic-mistakes-that-you-can-do-using-rabbitmq-nodejs-cbf5db99613c

https://medium.com/codait/handling-failure-successfully-in-rabbitmq-22ffa982b60f

https://www.cloudamqp.com/blog/part1-rabbitmq-best-practice.html

https://www.cloudamqp.com/blog/part4-rabbitmq-13-common-errors.html

https://www.cloudamqp.com/blog/part3-rabbitmq-best-practice-for-high-availability.html

https://www.cloudamqp.com/blog/part2-rabbitmq-best-practice-for-high-performance.html

