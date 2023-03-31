using System;
using System.Linq;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class UserTaskCompletedPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private readonly string _craftsmanId;
        private int _selectedRating;
        private string _ratingText;

        private Craftsman _bidCraftsman;
        private UserUiModel _bidCraftsmanUser;

        public UserUiModel BidCraftsmanUser
        {
            get => _bidCraftsmanUser;
            set
            {
                _bidCraftsmanUser = value;
                OnPropertyChanged();
            }
        }

        public int SelectedRating
        {
            get => _selectedRating;
            set
            {
                _selectedRating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSubmit));
            }
        }

        public bool CanSubmit => SelectedRating > 0;

        public string RatingText
        {
            get => _ratingText;
            set
            {
                _ratingText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitRatingCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public UserTaskCompletedPopupViewModel(ICallback view, string taskId, string craftsmanId)
        {
            this.view = view;
            _taskId = taskId;
            _craftsmanId = craftsmanId;

            SubmitRatingCommand = new AsyncCommand(SubmitRating);
            CloseCommand = new AsyncCommand(Close);
        }


        public override async void Navigated()
        {
            base.Navigated();
            
            try
            {
                userDialogs.ShowLoading();
                
                _bidCraftsman = (await dao.GetCraftsman(Settings.ActiveOffice, _craftsmanId));
                BidCraftsmanUser = (await dao.GetUser(_bidCraftsman.Id)).ToUserUiModel();
                
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
        }

        private async System.Threading.Tasks.Task SubmitRating()
        {
            try
            {
                if (CanSubmit)
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    var dt = DateTime.Now.Normalize();
                    var newRatingMdl = new Rating()
                    {
                        Id = string.Empty,
                        CraftsmanId = _bidCraftsman.Id,
                        OfficeId = _bidCraftsman.OfficeId,
                        TaskId = _taskId,
                        UserId = dao.ActiveUser.Id,

                        Amount = SelectedRating,
                        Header = string.Empty,
                        Text = RatingText,
                        Created = dt.ToRFC3339(),
                        Modified = dt,
                    };
                    
                    var result = await dao.Add(newRatingMdl);

                    await view.Close();
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        private async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
    }
}