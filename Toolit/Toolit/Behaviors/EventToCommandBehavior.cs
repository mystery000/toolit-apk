using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace Toolit.Behaviors
{
	// This is a modified version of https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/
    public class EventToCommandBehavior : Behavior<View>
    {
        /// <summary>
        /// Bindable property for Name of the event that will be forwarded to 
        /// <see cref="EventToCommandBehavior.Command"/>
        /// </summary>
        public static readonly BindableProperty EventNameProperty =
            BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior));

        /// <summary>
        /// Bindable property for Command to execute
        /// </summary>
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior));

        /// <summary>
        /// Bindable property for Argument sent to <see cref="ICommand.Execute(object)"/>
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior));

        /// <summary>
        /// Bindable property to set instance of <see cref="IValueConverter" /> to convert the <see cref="EventArgs" /> for <see cref="EventName" />
        /// </summary>
        public static readonly BindableProperty EventArgsConverterProperty =
            BindableProperty.Create(nameof(EventArgsConverter), typeof(IValueConverter),
                typeof(EventToCommandBehavior));

        /// <summary>
        /// Bindable property to set Argument passed as parameter to <see cref="IValueConverter.Convert" />
        /// </summary>
        public static readonly BindableProperty EventArgsConverterParameterProperty =
            BindableProperty.Create(nameof(EventArgsConverterParameter), typeof(object),
                typeof(EventToCommandBehavior));

        /// <summary>
        /// Bindable property to set Parameter path to extract property from <see cref="EventArgs"/> instance to pass to <see cref="ICommand.Execute"/>
        /// </summary>
        public static readonly BindableProperty EventArgsParameterPathProperty =
            BindableProperty.Create(
                nameof(EventArgsParameterPath),
                typeof(string),
                typeof(EventToCommandBehavior));

        /// <summary>
        /// <see cref="EventInfo"/>
        /// </summary>
        protected EventInfo _eventInfo;

        /// <summary>
        /// Delegate to Invoke when event is raised
        /// </summary>
        protected Delegate _handler;

        /// <summary>
        /// Parameter path to extract property from <see cref="EventArgs"/> instance to pass to <see cref="ICommand.Execute"/>
        /// </summary>
        public string EventArgsParameterPath
        {
            get { return (string) GetValue(EventArgsParameterPathProperty); }
            set { SetValue(EventArgsParameterPathProperty, value); }
        }

        /// <summary>
        /// Name of the event that will be forwared to <see cref="Command" />
        /// </summary>
        /// <remarks>
        /// An event that is invalid for the attached <see cref="View" /> will result in <see cref="ArgumentException" /> thrown.
        /// </remarks>
        public string EventName
        {
            get { return (string) GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// The command to execute
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Argument sent to <see cref="ICommand.Execute" />
        /// </summary>
        /// <para>
        /// If <see cref="EventArgsConverter" /> and <see cref="EventArgsConverterParameter" /> is set then the result of the
        /// conversion
        /// will be sent.
        /// </para>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Instance of <see cref="IValueConverter" /> to convert the <see cref="EventArgs" /> for <see cref="EventName" />
        /// </summary>
        public IValueConverter EventArgsConverter
        {
            get { return (IValueConverter) GetValue(EventArgsConverterProperty); }
            set { SetValue(EventArgsConverterProperty, value); }
        }

        /// <summary>
        /// Argument passed as parameter to <see cref="IValueConverter.Convert" />
        /// </summary>
        public object EventArgsConverterParameter
        {
            get { return GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        #region Base

        /// <summary>
        /// The Object associated with the Behavior
        /// </summary>
        public BindableObject AssociatedObject { get; private set; }

        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        /// <inheritDoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        #endregion
        
        
        /// <summary>
        /// Subscribes to the event <see cref="BindableObject"/> object
        /// </summary>
        /// <param name="bindable">Bindable object that is source of event to Attach</param>
        /// <exception cref="ArgumentException">Thrown if no matching event exists on 
        /// <see cref="BindableObject"/></exception>
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;

            if (bindable.BindingContext != null)
            {
                BindingContext = bindable.BindingContext;
            }

            bindable.BindingContextChanged += OnBindingContextChanged;

            _eventInfo = AssociatedObject
                .GetType()
                .GetRuntimeEvent(EventName);
            if (_eventInfo == null)
            {
                throw new ArgumentException(
                    $"No matching event '{EventName}' on attached type '{bindable.GetType().Name}'");
            }

            AddEventHandler(_eventInfo, AssociatedObject, OnEventRaised);
        }

        /// <summary>
        /// Unsubscribes from the event on <paramref name="bindable"/>
        /// </summary>
        /// <param name="bindable"><see cref="BindableObject"/> that is source of event</param>
        protected override void OnDetachingFrom(BindableObject bindable)
        {
            if (_handler != null)
            {
                _eventInfo.RemoveEventHandler(AssociatedObject, _handler);
            }

            _handler = null;
            _eventInfo = null;
            
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedObject = null;
        }

        private void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
        {
            var eventParameters = eventInfo.EventHandlerType
                .GetRuntimeMethods().First(m => m.Name == "Invoke")
                .GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType))
                .ToArray();

            var actionInvoke = action.GetType()
                .GetRuntimeMethods().First(m => m.Name == "Invoke");

            _handler = Expression.Lambda(
                    eventInfo.EventHandlerType,
                    Expression.Call(Expression.Constant(action), actionInvoke, eventParameters[0], eventParameters[1]),
                    eventParameters)
                .Compile();

            eventInfo.AddEventHandler(item, _handler);
        }

        /// <summary>
        /// Method called when event is raised
        /// </summary>
        /// <param name="sender">Source of that raised the event</param>
        /// <param name="eventArgs">Arguments of the raised event</param>
        protected virtual void OnEventRaised(object sender, EventArgs eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            var parameter = CommandParameter;

            if (parameter == null && !string.IsNullOrEmpty(EventArgsParameterPath))
            {
                //Walk the ParameterPath for nested properties.
                var propertyPathParts = EventArgsParameterPath.Split('.');
                object propertyValue = eventArgs;
                foreach (var propertyPathPart in propertyPathParts)
                {
                    var propInfo = propertyValue.GetType().GetRuntimeProperty(propertyPathPart);
                    if (propInfo == null)
                        throw new MissingMemberException($"Unable to find {EventArgsParameterPath}");

                    propertyValue = propInfo.GetValue(propertyValue);
                    if (propertyValue == null)
                    {
                        break;
                    }
                }

                parameter = propertyValue;
            }

            if (parameter == null && eventArgs != null && eventArgs != EventArgs.Empty && EventArgsConverter != null)
            {
                parameter = EventArgsConverter.Convert(eventArgs, typeof(object), EventArgsConverterParameter,
                    CultureInfo.CurrentUICulture);
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }
    }
}