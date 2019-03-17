namespace AzureFunctionsUpdates.Models
{
    public class UpdateMessage
    {
        public UpdateMessage(string topic, string content)
        {
            Topic = topic;
            Content = content;
        }

        public string Topic { get; set; }

        public string Content { get; set; }
    }
}
