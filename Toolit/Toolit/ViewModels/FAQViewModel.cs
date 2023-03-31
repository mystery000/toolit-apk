using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class FAQViewModel : BaseViewModel
    {

        public class FaqItem : INotifyPropertyChangedBase
        {
            private string _question;
            private string _answer;
            private bool _isExpanded;

            public string Question
            {
                get => _question;
                set
                {
                    _question = value;
                    OnPropertyChanged();
                }
            }

            public string Answer
            {
                get => _answer;
                set
                {
                    _answer = value;
                    OnPropertyChanged();
                }
            }

            public bool IsExpanded
            {
                get => _isExpanded;
                set
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        private ObservableCollection<FaqItem> _faqItems;
        public ICommand BackCommand { get; private set; }
        public ICommand ToggleExpandAnswerCommand { get; private set; }

        public ObservableCollection<FaqItem> FaqItems
        {
            get => _faqItems;
            set
            {
                _faqItems = value;
                OnPropertyChanged();
            }
        }

        public FAQViewModel(ICallback view)
        {
            this.view = view;
            
            BackCommand = new Command(Back);
            ToggleExpandAnswerCommand = new AsyncCommand<FaqItem>(ToggleExpandAnswer);

            FaqItems = new ObservableCollection<FaqItem>()
            {
                new FaqItem()
                {
                    Question = AppResources.FaqItem1Question,
                    Answer = AppResources.FaqItem1Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem2Question,
                    Answer = AppResources.FaqItem2Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem3Question,
                    Answer = AppResources.FaqItem3Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem4Question,
                    Answer = AppResources.FaqItem4Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem5Question,
                    Answer = AppResources.FaqItem5Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem6Question,
                    Answer = AppResources.FaqItem6Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem7Question,
                    Answer = AppResources.FaqItem7Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem8Question,
                    Answer = AppResources.FaqItem8Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem9Question,
                    Answer = AppResources.FaqItem9Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem10Question,
                    Answer = AppResources.FaqItem10Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem11Question,
                    Answer = AppResources.FaqItem11Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem12Question,
                    Answer = AppResources.FaqItem12Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem13Question,
                    Answer = AppResources.FaqItem13Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem14Question,
                    Answer = AppResources.FaqItem14Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem15Question,
                    Answer = AppResources.FaqItem15Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem16Question,
                    Answer = AppResources.FaqItem16Answer
                },
                
                new FaqItem()
                {
                    Question = AppResources.FaqItem17Question,
                    Answer = AppResources.FaqItem17Answer
                },
            };
        }

        public async void Back()
        {
            await view.Back();
        }

        public async System.Threading.Tasks.Task ToggleExpandAnswer(FaqItem item)
        {
            item.IsExpanded = !item.IsExpanded;
        }
    }
}