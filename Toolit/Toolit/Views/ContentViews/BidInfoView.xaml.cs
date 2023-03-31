using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolit.Models.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BidInfoView : ContentView
    {
        public static readonly BindableProperty DisplayedBidProperty =
            BindableProperty.Create(
                nameof(DisplayedBid), 
                typeof(BidUiModel), 
                typeof(BidInfoView), 
                default(BidUiModel));
        
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command), 
                typeof(ICommand), 
                typeof(BidInfoView), 
                default(ICommand));
        
        public BidUiModel DisplayedBid
        {
            get => (BidUiModel)GetValue(DisplayedBidProperty);
            set => SetValue(DisplayedBidProperty, value);
        }
        
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        
        public BidInfoView()
        {
            InitializeComponent();
            MasterContainer.BindingContext = this;
        }
    }
}