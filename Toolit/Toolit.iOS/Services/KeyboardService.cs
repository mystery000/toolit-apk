using System;
using DentMeApp.Shared.Contracts.Services;
using Toolit.iOS.Services;
using Toolit.Models.Misc;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(KeyboardService))]
namespace Toolit.iOS.Services
{
    public class KeyboardService : IKeyboardService
    {
        public event EventHandler<KeyboardEventArgs> KeyboardWillShow;
        public event EventHandler<KeyboardEventArgs> KeyboardWillHide;
        public event EventHandler<KeyboardEventArgs> KeyboardShown;
        public event EventHandler<KeyboardEventArgs> KeyboardHidden;

        public KeyboardService()
        {
            UIKeyboard.Notifications.ObserveDidShow(OnKeyboardShown);
            UIKeyboard.Notifications.ObserveDidHide(OnKeyboardHidden);
            UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
        }

        private void OnKeyboardShown(object sender, UIKeyboardEventArgs e)
        {
            KeyboardShown?.Invoke(this, new KeyboardEventArgs()
            {
                KeyboardHeight = e.FrameEnd.Height
            });
        }
        
        private void OnKeyboardHidden(object sender, UIKeyboardEventArgs e)
        {
            KeyboardHidden?.Invoke(this, new KeyboardEventArgs()
            {
                KeyboardHeight = e.FrameEnd.Height
            });
        }

        private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            KeyboardWillShow?.Invoke(this, new KeyboardEventArgs()
            {
                KeyboardHeight = e.FrameEnd.Height
            });
        }

        private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            KeyboardWillHide?.Invoke(this, new KeyboardEventArgs()
            {
                KeyboardHeight = e.FrameEnd.Height
            });
        }
    }
}