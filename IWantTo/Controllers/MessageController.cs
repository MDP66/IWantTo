using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Telegram.Bot.Types;

namespace IWantTo.Controllers
{
    public class MessageController : Controller
    {
        //private IModel _channel;
        //public MessageController(IModel channel)
        //{
        //    _channel = channel;
        //}

        [HttpGet]
        public IActionResult Index()
        {
            return Content(DateTime.Now.ToLongDateString());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://zxmblypp:R4eBqMzeU1yBzUvWxoCajpQVXRcm99M_@albatross.rmq.cloudamqp.com/zxmblypp")
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "messages",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    var obj = JsonConvert.SerializeObject(update);
                    var body = Encoding.UTF8.GetBytes(obj);

                    channel.BasicPublish(exchange: "",
                        routingKey: "messages",
                        basicProperties: null,
                        body: body);
                }
            }

            return Ok();
        }
    }
}