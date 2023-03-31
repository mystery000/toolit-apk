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
    public partial class BidCostView : ContentView
    {
        public static readonly BindableProperty DisplayedBidProperty =
            BindableProperty.Create(
                nameof(DisplayedBid), 
                typeof(BidUiModel), 
                typeof(BidCostView), 
                default(BidUiModel),
                propertyChanged:DisplayedBidPropertyChanged);

        private static void DisplayedBidPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BidCostView bcv && newvalue is BidUiModel newMdl)
            {
                bcv.OnPropertyChanged(nameof(IsBidMadeByUser));
            }
        }

        public static readonly BindableProperty IsInEditModeProperty =
            BindableProperty.Create(
                nameof(IsInEditMode), 
                typeof(bool), 
                typeof(BidCostView), 
                false);
        
        public static readonly BindableProperty IsRotRutProperty =
            BindableProperty.Create(
                nameof(IsRotRut), 
                typeof(bool), 
                typeof(BidCostView), 
                false);
        
        public static readonly BindableProperty EditCommandProperty =
            BindableProperty.Create(
                nameof(EditCommand), 
                typeof(ICommand), 
                typeof(BidCostView));
        
        public static readonly BindableProperty MoveToToSCommandProperty =
            BindableProperty.Create(
                nameof(MoveToToSCommand), 
                typeof(ICommand), 
                typeof(BidCostView));
        
        public BidUiModel DisplayedBid
        {
            get => (BidUiModel)GetValue(DisplayedBidProperty);
            set => SetValue(DisplayedBidProperty, value);
        }

        public bool IsInEditMode
        {
            get => (bool)GetValue(IsInEditModeProperty);
            set => SetValue(IsInEditModeProperty, value);
        }
        
        public bool IsRotRut
        {
            get => (bool)GetValue(IsRotRutProperty);
            set => SetValue(IsRotRutProperty, value);
        }
        
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }
        
        public ICommand MoveToToSCommand
        {
            get => (ICommand)GetValue(MoveToToSCommandProperty);
            set => SetValue(MoveToToSCommandProperty, value);
        }

        // if there's no craftsman id, it's likely in process of creation, hence it's made by user
        public bool IsBidMadeByUser => DisplayedBid?.CraftsmanId?.Equals(DAO.Instance.ActiveUser.Id) ?? true;
        
        public BidCostView()
        {
            InitializeComponent();
            MasterContainer.BindingContext = this;
        }
    }
}