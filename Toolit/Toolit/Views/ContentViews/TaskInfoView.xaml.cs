using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ICommand = System.Windows.Input.ICommand;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskInfoView : ContentView
    {
        public static readonly BindableProperty IsInEditModeProperty =
            BindableProperty.Create(
                nameof(IsInEditMode), 
                typeof(bool), 
                typeof(TaskInfoView), 
                false);
        
        public static readonly BindableProperty OpenFullImageCommandProperty =
            BindableProperty.Create(
                nameof(OpenFullImageCommand), 
                typeof(ICommand), 
                typeof(TaskInfoView), 
                default(ICommand));
        
        public static readonly BindableProperty OpenFullVideoCommandProperty =
            BindableProperty.Create(
                nameof(OpenFullVideoCommand), 
                typeof(ICommand), 
                typeof(TaskInfoView), 
                default(ICommand));
        
        public bool IsInEditMode
        {
            get => (bool)GetValue(IsInEditModeProperty);
            set => SetValue(IsInEditModeProperty, value);
        }
        
        public ICommand OpenFullImageCommand
        {
            get => (ICommand)GetValue(OpenFullImageCommandProperty);
            set => SetValue(OpenFullImageCommandProperty, value);
        }
        
        public ICommand OpenFullVideoCommand
        {
            get => (ICommand)GetValue(OpenFullVideoCommandProperty);
            set => SetValue(OpenFullVideoCommandProperty, value);
        }
        
        public TaskInfoView()
        {
            InitializeComponent();
        }
    }
}