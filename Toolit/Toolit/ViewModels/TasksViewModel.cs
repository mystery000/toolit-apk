using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using Toolit.Extensions;
using Toolit.Mappers;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToMyTask(string taskId);
            System.Threading.Tasks.Task MoveToOtherTask(string taskId);
            System.Threading.Tasks.Task MoveToAd(AdUiModel ad);
        }

        private readonly ICallback view;
        
        private List<TaskUiModel> _taskCache;
        private List<BidUiModel> _bidCache;
        private List<AdUiModel> _adCache;

        private CraftsmanUiModel _userCraftsman;

        private readonly object _otherTaskListLock = new object();
        
        public ICommand ExecuteSearchQueryCommand { get; private set; }
        public ICommand MoveToMyTaskCommand { get; private set; }
        public ICommand MoveToOtherTaskCommand { get; private set; }
        public ICommand MoveToAdCommand { get; private set; }
        public ICommand ToggleExpandCategoryCommand { get; private set; }
        public ICommand ToggleExpandFilterCommand { get; private set; }
        
        public ICommand CraftTappedCommand { get; private set; }
        public ICommand SelectAllCraftFiltersCommand { get; private set; }

        public string UserName => dao?.ActiveUser?.FirstName;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        private bool _isCategoryFilterExpanded;
        public bool IsCategoryFilterExpanded
        {
            get { return _isCategoryFilterExpanded; }
            set
            {
                _isCategoryFilterExpanded = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isDistanceFilterExpanded;
        public bool IsDistanceFilterExpanded
        {
            get { return _isDistanceFilterExpanded; }
            set
            {
                _isDistanceFilterExpanded = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TaskUiModel> _myTaskList;
        public ObservableCollection<TaskUiModel> MyTaskList
        {
            get => _myTaskList;
            set
            {
                _myTaskList = value;
                OnPropertyChanged();
            }
        }
        
        private ObservableCollection<OtherJobLayoutModel> _otherTaskList;
        public ObservableCollection<OtherJobLayoutModel> OtherTaskList
        {
            get => _otherTaskList;
            set
            {
                _otherTaskList = value;
                OnPropertyChanged();
            }
        }
        
        
        private ObservableCollection<CraftLayoutModel> _craftList;
        private string _searchQuery;

        public ObservableCollection<CraftLayoutModel> CraftList
        {
            get => _craftList;
            set
            {
                _craftList = value;
                OnPropertyChanged();
            }
        }

        public bool AreAllFiltersSelected => CraftList.All(crft => crft.IsSelected);


        public TasksViewModel(ICallback view)
        {
            IsATab = true;
            this.view = view;
            IsCategoryFilterExpanded = false;
            IsDistanceFilterExpanded = false;
            
            ExecuteSearchQueryCommand = new AsyncCommand(ExecuteSearchQuery);
            MoveToMyTaskCommand = new AsyncCommand<TaskUiModel>(MoveToMyTask);
            MoveToOtherTaskCommand = new AsyncCommand<OtherJobLayoutModel>(MoveToOtherTask);
            ToggleExpandCategoryCommand = new Command(ToggleExpandCategory);
            ToggleExpandFilterCommand = new Command(ToggleExpandFilter);
            CraftTappedCommand = new AsyncCommand<CraftLayoutModel>(CraftTapped);
            SelectAllCraftFiltersCommand = new AsyncCommand(SelectAllCraftFilters);
            
            MyTaskList = new ObservableCollection<TaskUiModel>();
            OtherTaskList = new ObservableCollection<OtherJobLayoutModel>();

            CraftList = new ObservableCollection<CraftLayoutModel>(AppConstants.CraftModels);
        }

        public override async void Navigated()
        {
            base.Navigated();

            await dao.IsSignedIn();

            try
            {
                _userCraftsman = (await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id)).ToCraftsmanUiModel();
            }
            catch (Exception ex)
            {
                // user's not a craftsman, carry on
            }

            if (!IsSubscribedToDao)
            {
                dao.Subscribe(HandleTaskUpdate, HandleError);
                dao.Subscribe(HandleBidUpdate, HandleError);
                dao.Subscribe(HandleAdSuccess, HandleError);
                
                IsSubscribedToDao = true;
            }
            
            SelectAll(true);
            OnPropertyChanged(nameof(UserName));
            
            userDialogs.HideLoading();
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();

            SearchQuery = string.Empty;

            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandleTaskUpdate, HandleError);
                dao.Unsubscribe(HandleBidUpdate, HandleError);
                dao.Unsubscribe(HandleAdSuccess, HandleError);
                IsSubscribedToDao = false;
            }
        }

        private async void HandleTaskUpdate(Task[] data, string nonce, DateTime updated)
        {
            try
            {
                if (data.Length == 0)
                {
                    Crashes.TrackError(new DataException($"No tasks received in the subscription handler for user {dao.ActiveUser.Id}"));
                }
                
                _taskCache = data.Where(tsk => !tsk.Deleted)
                    .Select(tsk => tsk.ToTaskUiModel()).ToList();

                UpdateFilter();
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        private void HandleBidUpdate(Bid[] data, string nonce, DateTime updated)
        {
            try
            {
                _bidCache = data.Where(b => !b.Deleted).Select(b => b.ToBidUiModel()).ToList();
                
                UpdateFilter();
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        private void HandleAdSuccess(Ad[] data, string nonce, DateTime updated)
        {
            _adCache = data.Where(a => !a.Deleted).Select(a => a.ToAdUiModel()).ToList();
            
            UpdateFilter();
        }
        
        private async System.Threading.Tasks.Task ExecuteSearchQuery()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length >= 3)
            {
                UpdateFilter();
            }
        }
        
        public async System.Threading.Tasks.Task MoveToMyTask(TaskUiModel task)
        {
            // showing loading during navigation sometimes freezes the app on android
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
            }
            
            if (task.HasUsersBid)
            {
                await view.MoveToOtherTask(task.Id);
            }
            else
            {
                await view.MoveToMyTask(task.Id);
            }
        }
        
        public async System.Threading.Tasks.Task MoveToOtherTask(OtherJobLayoutModel job)
        {
            if (job.IsAnAd)
            {
                await view.MoveToAd(job.Ad);
            }
            else
            {
                if (!job.Task.IsBiddable)
                {
                    return;
                }

                // showing loading during navigation sometimes freezes the app on android
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);
                }
                await view.MoveToOtherTask(job.Task.Id);
            }
        }
        
        public void ToggleExpandCategory()
        {
            IsCategoryFilterExpanded = !IsCategoryFilterExpanded;
        }
        public void ToggleExpandFilter()
        {
            IsDistanceFilterExpanded = !IsDistanceFilterExpanded;
        }
        private async System.Threading.Tasks.Task CraftTapped(CraftLayoutModel craft)
        {
            if (AreAllFiltersSelected)
            {
                foreach (var craftMdl in CraftList)
                {
                    craftMdl.IsSelected = (craftMdl == craft);
                }
            }
            else
            {
                craft.IsSelected = !craft.IsSelected;
            }
            UpdateFilter();
        }

        private async System.Threading.Tasks.Task SelectAllCraftFilters()
        {
            SelectAll();
        }

        private void SelectAll(bool enableAllOverride = false)
        {
            var prevValue = AreAllFiltersSelected;
            
            foreach (var craft in CraftList)
            {
                if (!enableAllOverride)
                {
                    // select all if not all were selected
                    // select none if all were selected
                    craft.IsSelected = !prevValue;
                }
                else
                {
                    craft.IsSelected = true;
                }
            }
            
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            if (_taskCache == null || _bidCache == null || dao.ActiveUser?.Id == null) 
            {
                return;
            }

            foreach (var task in _taskCache)
            {
                task.HasUsersBid = _bidCache
                    .Any(b => b.CraftsmanId.Equals(dao.ActiveUser.Id) && b.TaskId.Equals(task.Id));
            }

            var myTaskList = _taskCache
                .Where(tsk => (tsk?.UserId.Equals(dao.ActiveUser.Id) ?? false) || (tsk?.HasUsersBid ?? false));

            // search bar filter
            if (!string.IsNullOrWhiteSpace(SearchQuery) && SearchQuery.Length >= 3)
            {
                myTaskList = myTaskList
                    .Where(tsk => tsk.DoesMatchQuery(SearchQuery));
            }

            MyTaskList = new ObservableCollection<TaskUiModel>(myTaskList
                .OrderByDescending(tsk => tsk.Modified));
            
            var selectedFilters = CraftList
                .Where(crft => crft.IsSelected)
                .Select(c => c.ServerId);
            
            var filteredTasks = _taskCache.Where(tsk =>
                tsk.Crafts.Any(crft => selectedFilters.Contains(crft)))
                .ToList();
            
            // only other tasks are being filtered
            var newOtherTaskList = filteredTasks
                .Where(tsk => (!tsk?.UserId.Equals(dao.ActiveUser.Id) ?? false) 
                              && !tsk.HasUsersBid 
                              && !tsk.Finished)
                .ToList();

            if (_userCraftsman?.Crafts != null && !_userCraftsman.IsDeleted)
            {
                foreach (var task in newOtherTaskList)
                {
                    if (_userCraftsman.IsFrozen)
                    {
                        // frozen craftsmen can't bid on anything
                        task.IsBiddable = false;
                        continue;
                    }
                    
                    task.IsBiddable = _userCraftsman.Crafts.Any(crft => 
                        crft.Status == CraftStatus.Approved && task.Crafts.Contains(crft.CraftType));
                }
            }
            else
            {
                // allow users to view tasks, they can't bid anyway
                foreach (var task in newOtherTaskList)
                {
                    task.IsBiddable = true;
                }
            }

            // search bar filter
            if (!string.IsNullOrWhiteSpace(SearchQuery) && SearchQuery.Length >= 3)
            {
                newOtherTaskList = newOtherTaskList
                    .Where(tsk => tsk.DoesMatchQuery(SearchQuery))
                    .ToList();
            }
            
            var otherTaskList = newOtherTaskList
                .Select(task => new OtherJobLayoutModel() {Task = task, OrderDate = task.Modified}).ToList();
            
            if (_adCache != null)
            {
                otherTaskList.AddRange(_adCache
                    .Select(ad => new OtherJobLayoutModel() {Ad = ad, OrderDate = ad.Modified}));
            }

            // this method can be called by several subscription handlers at once, so it needs some protection
            lock (_otherTaskListLock)
            {
                OtherTaskList =
                    new ObservableCollection<OtherJobLayoutModel>(
                        otherTaskList.OrderByDescending(tsk => tsk.OrderDate));
            }

            OnPropertyChanged(nameof(AreAllFiltersSelected));
            OnPropertyChanged(nameof(OtherTaskList));
        }
    }
}
