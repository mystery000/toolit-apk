using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace Toolit.Controls
{
    public enum ImagePosition
    {
        Left,
        Right,
        None
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PancakeButton : PancakeView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text),
                typeof(string),
                typeof(PancakeButton),
                string.Empty);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor),
                typeof(Color),
                typeof(PancakeButton),
                default(Color));

        public static readonly BindableProperty TextAlignmentProperty =
            BindableProperty.Create(nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(PancakeButton),
                TextAlignment.Center);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily),
                typeof(string),
                typeof(PancakeButton));

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize),
                typeof(double),
                typeof(PancakeButton),
                default(double));

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource),
                typeof(ImageSource),
                typeof(PancakeButton),
                null);

        public static readonly BindableProperty ImagePositionProperty =
            BindableProperty.Create(nameof(ImagePosition),
                typeof(ImagePosition),
                typeof(PancakeButton),
                ImagePosition.Left);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command),
                typeof(ICommand),
                typeof(PancakeButton),
                null,
                BindingMode.TwoWay);
        
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter),
                typeof(object),
                typeof(PancakeButton),
                null,
                BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty).ToString();
            set => SetValue(TextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public ImagePosition ImagePosition
        {
            get => (ImagePosition)GetValue(ImagePositionProperty);
            set => SetValue(ImagePositionProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public PancakeButton()
        {
            InitializeComponent();

            foreach (var ctrl in ControlContainer.Children)
            {
                ctrl.BindingContext = this;
            }
        }
    }
}