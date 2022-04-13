using Microsoft.Extensions.DependencyInjection;
using Outrage.EventBus;
using WindowsForms.Example.Messages;

namespace WindowsForms.Example
{
    public partial class MessageForm : Form
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IClientEventBus clientEventBus;
        private ISubscriber? subscriber;
        public string Messages { get; set; } = "Enter a message to send it to other forms.";
        public string TextMessage { get; set; } = String.Empty;

        public MessageForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            this.clientEventBus = serviceProvider.GetRequiredService<IClientEventBus>();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.messagesBox.Text = Messages;

            // Subscribe to messages on the bus.
            // subscriber is held by the form, so the subscription goes out of scope
            // when the form is closed.
            this.subscriber = this.clientEventBus.Subscribe<TextMessage>((context, message) =>
            {
                this.Messages += "\r\n" + message.Message;
                // Marshal the UI update back onto the UI thread
                // The message is NOT received on the UI thread,
                // so not invoking like this will cause an error.
                this.Invoke(() =>
                {
                    messagesBox.Text = Messages;
                });

                return Task.CompletedTask;
            });
        }

        private void newFormButton_Click(object sender, EventArgs e)
        {
            var form = serviceProvider.GetRequiredService<MessageForm>();
            form.Show();
        }

        private async void sendChatButton_Click(object sender, EventArgs e)
        {
            var text = textMessageBox.Text;
            textMessageBox.Text = String.Empty;

            // Publish a message to the event bus, all forms listening to it will receive
            // every message published to the bus.
            if (!String.IsNullOrEmpty(text))
                await this.clientEventBus.PublishAsync(new TextMessage(text));
        }
    }
}