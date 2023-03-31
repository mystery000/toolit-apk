using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CachedAvatarView : ContentView
    {
        public static readonly BindableProperty ImageUrlProperty =
            BindableProperty.Create(nameof(ImageUrl),
                typeof(string),
                typeof(CachedAvatarView),
                string.Empty);
        
        public static readonly BindableProperty UserInitialsProperty =
            BindableProperty.Create(nameof(UserInitials),
                typeof(string),
                typeof(CachedAvatarView),
                string.Empty);
        
        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }
        
        public string UserInitials
        {
            get => (string)GetValue(UserInitialsProperty);
            set => SetValue(UserInitialsProperty, value);
        }

        public CachedAvatarView()
        {
            InitializeComponent();
        }
    }
}