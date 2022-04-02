using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

public class Produto
{
    public int Id { get; set; }
    public string Descricao { get; set; }

    public string link { get; set; }

    public Produto(int id, string descricao )
    {
        Id = id;
        Descricao = descricao;
        link = "https://teste.com.br" + descricao;
    }
}
class Send
{
    public static void Main()
    { 
        var factory = new ConnectionFactory() { HostName = "localhost", UserName= "admin", Password = "123456" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Criando a EXchange
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");

            // Criando a Fila
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            
            channel.QueueDeclare(queue: "hello2",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);



            //Bindando uma queue em uma exchange
            channel.QueueBind(queue: "hello",
                  exchange: "direct_logs",
                  routingKey: "black");

            channel.QueueBind(queue: "hello2",
                  exchange: "direct_logs",
                  routingKey: "black");
            

            int contador = 0;
            while (true)
            {
                contador++;
                string message = "Hello World! " + contador.ToString();
                Produto prod = new Produto(contador, message);
                message = JsonConvert.SerializeObject(prod);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: "black",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                System.Threading.Thread.Sleep(5000);
            }

        }
    }
}