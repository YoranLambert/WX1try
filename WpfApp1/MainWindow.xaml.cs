using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HiveMQClient Client;
        public string AnswerTopic;
        public bool NeedAnswer;

        public void OnMessageReceived(object Sender, OnMessageReceivedEventArgs args)
        {
            var output = JsonConvert.DeserializeObject<Response>(args.PublishMessage.PayloadAsString);
            Question.Content = output.Question;
            AnswerTopic = output.TopicToPublishAnswerTo;
            NeedAnswer = true;
        }


        public MainWindow()
        {
            InitializeComponent();
            Connect();
        }
        private async void Connect()
        {
            var options = new HiveMQClientOptions();
            options.Host = "ce6b74a7cccc45cb8db551fc2d7394bb.s2.eu.hivemq.cloud";
            options.Port = 8883;
            options.UseTLS = true;
            options.UserName = "wx1mqttdemo2324jh1";
            options.Password = "4iJv8fiAdJ4mzcsbdw6i";

            Client = new HiveMQClient(options);
            await Client.ConnectAsync().ConfigureAwait(false);

            await Client.SubscribeAsync("teams/10/question/new");
            await Client.SubscribeAsync("teams/10/question/error");
            Client.OnMessageReceived += (sender, e) =>
            {
                OnMessageReceived(sender, e);
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NeedAnswer)
            {
                await Client.PublishAsync(AnswerTopic, Answer.Text);
                await Client.DisconnectAsync().ConfigureAwait(false);
            }
        }
    }
    public class Response
    {
        public string Question { get; set; }
        public string TopicToPublishAnswerTo { get; set; }
    }
}
