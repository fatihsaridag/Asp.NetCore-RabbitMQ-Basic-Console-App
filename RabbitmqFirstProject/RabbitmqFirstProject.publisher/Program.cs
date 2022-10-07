using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitmqFirstProject.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            //RabbitMq ya bağlanmamız için connection factory isminde bir nesne örneğği alalım
            var factory = new ConnectionFactory();
            //Uri yımızı belirticez. CloudAmqp deki instancedan urli alıyoruz. Gerçek seneryoda bunları appsetting.json içeçrisinde tutuyoruz.
            factory.Uri = new Uri("amqps://eznbdupx:Qf4h0Avxf0yEipy5VaR1D7UHRfIL0Gfn@gerbil.rmq.cloudamqp.com/eznbdupx");

            //factory üzerinden bir bağlantı açıyoruz.
            using var connection = factory.CreateConnection();
            //Artık bir bağlantımız var ve bu bağlantı  üzerinden kanal oluşturuyoruz onun üzerinden bağlanıcaz.
            var channel = connection.CreateModel(); //Bu kanal üzerinden rabbitMq ile haberleşebiliriz.
            channel.QueueDeclare("hello-queue",true,false,false);   //Eğer 2.parametreyi false yaparsak memory de durmaz ve res atınca gider. True da ise gitmez. 
                                                                    //Eğer 3. paramtre true olursa bizim oluşturduğumuz kanal üzerinden bağlanabiliriz. Ama biz subsriber tarafından farklı bir kanal üzerinden bağlanmak istiyoruz o yüzden false
                                                                    // 4. parametre de subscribera bağlı olan son kuyruk bağlantısını kopartırsa bağlantısını siler yani subscriber olmasa bile kuyruk her zaman hayatta dursun o yüzden false  
            Enumerable.Range(1, 50).ToList().ForEach(x =>
             {
                 string message = $"Message {x}"; // Byte çevirip istediğimiz formatı gönderebiliriz.
                var messageBody = Encoding.UTF8.GetBytes(message);
                 channel.BasicPublish(string.Empty, "hello-queue", null, messageBody); //Exchange kullanmadığımız işleme default exchange diyoruz(2.paramtre) 
                Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
             });

           
               
        }
    }
}
