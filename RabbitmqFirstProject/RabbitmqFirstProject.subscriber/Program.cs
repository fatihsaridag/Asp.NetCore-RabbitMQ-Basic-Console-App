using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitmqFirstProject.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            //RabbitMq ya bağlanmamız için connection factory isminde bir nesne örneğği alalım
            var factory = new ConnectionFactory(); //Uri yımızı belirticez. CloudAmqp deki instancedan urli alıyoruz. Gerçek seneryoda bunları appsetting.json içeçrisinde tutuyoruz.
            factory.Uri = new Uri("amqps://eznbdupx:Qf4h0Avxf0yEipy5VaR1D7UHRfIL0Gfn@gerbil.rmq.cloudamqp.com/eznbdupx");  //factory üzerinden bir bağlantı açıyoruz.
            using var connection = factory.CreateConnection(); //Artık bir bağlantımız var ve bu bağlantı  üzerinden kanal oluşturuyoruz onun üzerinden bağlanıcaz.
            var channel = connection.CreateModel(); //Bu kanal üzerinden rabbitMq ile haberleşebiliriz.
                                                    // channel.QueueDeclare("hello-queue", true, false, false);   //Eğer 2.parametreyi false yaparsak memory de durmaz ve res atınca gider. True da ise gitmez. 
            channel.BasicQos(0, 1, false); //1. parametre herhangi boyuttaki bir mesajı gönderebilirsin. , 2.parametre mesajlar kaç kaç gelsin 3.parametre : false dersek 5 birine 5 diğerine verir ama true dersek toplam iki suba 5 tane verir
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("hello-queue", false,consumer); //Eğer ben 2. paramtreye true verirsem  rabbitmq subscriber a mesaj gönderdiğinde bu mesaj doğru da işlense yanlış da işlense mesajı siler . Eğer false ise yanlış da olsa sen mesajı silme ben sana haber edicem diyor.  
            consumer.Received += (object sender, BasicDeliverEventArgs e) => {  //Bu event subscribera mesaj gönderince fırlayacak.
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);     //Her bir mesaj 1.5 sn gecikmeli gelecek.
                Console.WriteLine("Gelen mesaj : " + message);

                channel.BasicAck(e.DeliveryTag, false); //Sen artık ilgili mesajı silebilirsin. 1. parametre hangi tag la geldiyse , 2.parametre multiple değeri eğer true dersek memory de işlenmiş ama rabbitmq ya gitmememiş başka mesajlar da varsa onun bilgilerini rabbitmq ya haber eder. False diyince sadece ilgili mesajın durumunu rabbit mq ya bildir diyoruz.
            };
            Console.ReadLine();
        }
    }
}
