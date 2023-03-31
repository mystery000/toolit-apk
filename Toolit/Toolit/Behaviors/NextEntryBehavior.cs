using System;
using Xamarin.Forms;

namespace Toolit.Behaviors
{
    /// <summary>
    /// Custom behavior for handling the 'Next' return key tap
    /// </summary>
    public class NextEntryBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty NextEntryProperty = BindableProperty.Create(
            nameof(NextEntry), typeof(Entry), typeof(Entry), defaultBindingMode: BindingMode.OneTime, defaultValue: null);

        public Entry NextEntry
        {
            get => (Entry)GetValue(NextEntryProperty);
            set => SetValue(NextEntryProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.Completed += Bindable_Completed;
            bindable.ReturnType = ReturnType.Next;

            base.OnAttachedTo(bindable);
        }

        private void Bindable_Completed(object sender, EventArgs e)
        {
            NextEntry?.Focus();
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= Bindable_Completed;
            base.OnDetachingFrom(bindable);
        }
    }
}
