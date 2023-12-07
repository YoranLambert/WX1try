using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Options;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading;

namespace WX1try
{
    internal class Program
    {
        public static HiveMQClient Client { get; set; }
        static async Task Main(string[] args)
        {
            var options = new HiveMQClientOptions();
            options.Host = "508b55b4d3794a40af7e9ade9d709bee.s2.eu.hivemq.cloud";
            options.Port = 8883;
            options.UseTLS = true;
            options.UserName = "Boars";
            options.Password = "BoarCrane1";
            options.CleanStart = true;

            Client = new HiveMQClient(options);
            await Client.ConnectAsync().ConfigureAwait(false);

            await Client.SubscribeAsync("teams/list");
            await Client.SubscribeAsync("teams/10/question/new");
            await Client.SubscribeAsync("teams/10/question/error");
            Client.OnMessageReceived += (sender, e) =>
            {
                OnMessageReceived(sender, e);
            };

            while (true) await Task.Delay(100000);

        }
        public static async Task KeepBusy()
        {
            await Task.Delay(100000);
            await Client.DisconnectAsync().ConfigureAwait(false);
        }
        public static async void OnMessageReceived(object Sender, OnMessageReceivedEventArgs args)
        {
            var output = JsonConvert.DeserializeObject<Response>(args.PublishMessage.PayloadAsString);
            Console.WriteLine(output.Question);
            await Client.PublishAsync(output.TopicToPublishAnswerTo, Console.ReadLine());
        }

    }
    public class Response
    {
        public string Question { get; set; }
        public string TopicToPublishAnswerTo { get; set; }
    }
}