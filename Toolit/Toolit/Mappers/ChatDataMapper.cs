using System;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Mappers
{
    public static class ChatDataMapper
    {
        
        public static ChatUiModel ToChatUiModel(this Chat mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new ChatUiModel()
            {
                Id = mdl.Id,
                OfficeId = mdl.OfficeId,
                TaskId = mdl.TaskId,
                BidId = mdl.BidId,
                
                Modified = mdl.Modified
            };
        }
        
        public static Chat ToChatApiModel(this ChatUiModel mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new Chat()
            {
                Id = mdl.Id,
                OfficeId = mdl.OfficeId,
                TaskId = mdl.TaskId,
                BidId = mdl.BidId,
                
                Modified = mdl.Modified
            };
        }
        
        public static MessageUiModel ToMessageUiModel(this Message mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new MessageUiModel()
            {
                Id = mdl.Id,
                OfficeId = mdl.OfficeId,
                TaskId = mdl.TaskId,
                ChatId = mdl.ChatId,
                UserId = mdl.UserId,
                
                Sent = mdl.Sent.ToLocalTime(),
                Text = mdl.Text,
                ImageUrl = (mdl.ImageSource as UriImageSource)?.Uri?.ToString() ?? string.Empty,
                IsRead = mdl.IsRead,
                Status = mdl.IsRead ? MessageStatus.Read : MessageStatus.Sent
            };
        }
    }
}