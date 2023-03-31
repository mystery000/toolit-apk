using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanInfoView : ContentView
    {
        public static readonly BindableProperty IsInEditModeProperty =
            BindableProperty.Create(
                nameof(IsInEditMode), 
                typeof(bool), 
                typeof(CraftsmanInfoView), 
                false);
        
        public bool IsInEditMode
        {
            get => (bool)GetValue(IsInEditModeProperty);
            set => SetValue(IsInEditModeProperty, value);
        }
        
        public CraftsmanInfoView()
        {
            InitializeComponent();
        }
    }
}