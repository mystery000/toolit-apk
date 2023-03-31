using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ImageSource = Xamarin.Forms.ImageSource;

namespace Toolit.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingBar : ContentView
    {
        public const int NumberOfStars = 5;
        
        public static readonly BindableProperty RatingProperty =
            BindableProperty.Create(
                nameof(Rating), 
                typeof(double), 
                typeof(RatingBar), 
                0.0d,
                propertyChanged: RatingPropertyChanged);
        
        public static readonly BindableProperty IsInDisplayModeProperty =
            BindableProperty.Create(
                nameof(IsInDisplayMode), 
                typeof(bool), 
                typeof(RatingBar), 
                false,
                propertyChanged: IsInDisplayModePropertyChanged);

        public double Rating
        {
            get => (double)GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }
        
        public bool IsInDisplayMode
        {
            get => (bool)GetValue(IsInDisplayModeProperty);
            set => SetValue(IsInDisplayModeProperty, value);
        }

        public RatingBar()
        {
            InitializeComponent();
            RefreshStars();
        }

        private Grid CreateStar(double amountFilled)
        {
            if (amountFilled == 0.0d)
            {
                return new Grid()
                {
                    Children = { new Image()
                    {
                        Source = ImageSource.FromFile("star_empty")
                    }}
                };
            }

            if (Math.Abs(amountFilled - 1.0f) < double.Epsilon)
            {
                return new Grid()
                {
                    Children = { new Image()
                    {
                        Source = ImageSource.FromFile("star")
                    }}
                };
            }
            // add two overlapping stars, with the filled star cropped to reflect the rating
            var emptyImage = new CachedImage()
            {
                Source = ImageSource.FromFile("star_empty")
            };
            var filledImage = new CachedImage()
            {
                Source = ImageSource.FromFile("star"),
                HorizontalOptions = LayoutOptions.Start
            };

            filledImage.Transformations = new List<ITransformation>()
            {
                new CropTransformation(
                    1.0, 
                    amountFilled >= 0.5 ? -amountFilled : (-1 + amountFilled) * (1 / (amountFilled*2)), 
                    0.0, 
                    amountFilled, 
                    1.0)
            };

            var grid = new Grid();
            
            grid.Children.Add(emptyImage);
            grid.Children.Add(filledImage);

            return grid;
        }

        public void RefreshStars()
        {
            StarContainer.Children.Clear();

            var fullStars = (int) Rating;
            var fraction = Rating - fullStars;
            var emptyStars = fraction > 0.0d ? NumberOfStars - fullStars - 1 : NumberOfStars - fullStars;

            for (int i = 0; i < fullStars; i++)
            {
                StarContainer.Children.Add(CreateStar(1.0d));
            }
            if (fraction > 0.0d)
            {
                StarContainer.Children.Add(CreateStar(fraction));
            }
            for (int i = 0; i < emptyStars; i++)
            {
                StarContainer.Children.Add(CreateStar(0.0d));
            }

            if (!IsInDisplayMode)
            {
                for (int i = 0; i < StarContainer.Children.Count; i++)
                {
                    StarContainer.Children[i].GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new AsyncCommand<int>(SetRating),
                        CommandParameter = i + 1
                    });
                }
            }
        }

        private async System.Threading.Tasks.Task SetRating(int newRating)
        {
            Rating = newRating;
        }

        private static void RatingPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is RatingBar ratingBar)
            {
                if (newvalue is double newRating &&
                    oldvalue is double oldRating)
                {
                    // rating can't exceed the number of stars or be negative
                    if (newRating > RatingBar.NumberOfStars ||
                        newRating < 0.0d)
                    {
                        ratingBar.Rating = oldRating;
                    }
                    else
                    {
                        ratingBar.RefreshStars();
                    }
                }
            }
        }
        
        private static void IsInDisplayModePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            
        }

    }
}