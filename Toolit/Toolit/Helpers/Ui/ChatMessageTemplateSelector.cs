using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Helpers.Ui
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SentMessageTemplate { get; set; }
        public DataTemplate ReceivedMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is MessageUiModel message)
            {
                return message.UserId.Equals(message.ChatRecipientId) ? ReceivedMessageTemplate : SentMessageTemplate;
            }

            return null;
        }
    }
}