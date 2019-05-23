using System;
using RabbitMQ.Client;

namespace IWantTo.Queue
{
    public class QueueBuilder
    {
        public static IModel CreateChannel()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://zxmblypp:R4eBqMzeU1yBzUvWxoCajpQVXRcm99M_@albatross.rmq.cloudamqp.com/zxmblypp")
            };
            using (var connection = factory.CreateConnection())
            {
                var channel = connection.CreateModel();
                return channel;
            }
        }
    }
}
