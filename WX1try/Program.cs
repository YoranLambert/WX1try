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
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);
        public static HiveMQClient Client { get; set; }
        static async Task Main(string[] args)
        {
            var options = new HiveMQClientOptions();
            options.Host = "87161f7f14664f20a4a1d7d8fe033d62.s1.eu.hivemq.cloud";
            options.Port = 8883;
            options.UseTLS = true;
            options.UserName = "wx1mqttdemo2324jh1";
            options.Password = "4iJv8fiAdJ4mzcsbdw6i";

            Client = new HiveMQClient(options);
            await Client.ConnectAsync().ConfigureAwait(false);

            await Client.SubscribeAsync("teams/list");
            await Client.SubscribeAsync("teams/10/question/new");
            await Client.SubscribeAsync("teams/10/question/error");
            Client.OnMessageReceived += (sender, e) =>
            {
                OnMessageReceived(sender, e);
            };
            waitHandle.WaitOne();
            await Client.DisconnectAsync().ConfigureAwait(false);
        }
        public static async void OnMessageReceived(object Sender, OnMessageReceivedEventArgs args)
        {
            var output = JsonConvert.DeserializeObject<Response>(args.PublishMessage.PayloadAsString);
            Console.WriteLine(output.Question);
            await Client.PublishAsync(output.TopicToPublishAnswerTo, Console.ReadLine());
            waitHandle.Set();
        }

    }
    public class Response
    {
        public string Question { get; set; }
        public string TopicToPublishAnswerTo { get; set; }
    }
}