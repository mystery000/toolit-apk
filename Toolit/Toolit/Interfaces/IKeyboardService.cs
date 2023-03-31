using System;
using Toolit.Models.Misc;

namespace DentMeApp.Shared.Contracts.Services
{
    public interface IKeyboardService
    {
        event EventHandler<KeyboardEventArgs> KeyboardWillShow;
        event EventHandler<KeyboardEventArgs> KeyboardWillHide;
        event EventHandler<KeyboardEventArgs> KeyboardShown;
        event EventHandler<KeyboardEventArgs> KeyboardHidden;
    }
}