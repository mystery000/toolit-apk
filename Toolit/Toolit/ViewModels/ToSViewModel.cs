using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Resourses;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class ToSViewModel : BaseViewModel
    {
        public class TosItem : INotifyPropertyChangedBase
        {
            private string _sectionTitle;
            private string _sectionContent;

            public string SectionTitle
            {
                get => _sectionTitle;
                set
                {
                    _sectionTitle = value;
                    OnPropertyChanged();
                }
            }

            public string SectionContent
            {
                get => _sectionContent;
                set
                {
                    _sectionContent = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        
        private ObservableCollection<TosItem> _tosItems;
        
        public ObservableCollection<TosItem> TosItems
        {
            get => _tosItems;
            set
            {
                _tosItems = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; private set; }

        public ToSViewModel(ICallback view)
        {
            this.view = view;
            BackCommand = new Command(Back);

            TosItems = new ObservableCollection<TosItem>()
            {
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection1Title,
                    SectionContent = AppResources.TosSection1Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection2Title,
                    SectionContent = AppResources.TosSection2Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection3Title,
                    SectionContent = AppResources.TosSection3Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection4Title,
                    SectionContent = AppResources.TosSection4Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection5Title,
                    SectionContent = AppResources.TosSection5Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection6Title,
                    SectionContent = AppResources.TosSection6Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection7Title,
                    SectionContent = AppResources.TosSection7Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection8Title,
                    SectionContent = AppResources.TosSection8Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection9Title,
                    SectionContent = AppResources.TosSection9Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection10Title,
                    SectionContent = AppResources.TosSection10Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection11Title,
                    SectionContent = AppResources.TosSection11Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection12Title,
                    SectionContent = AppResources.TosSection12Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection13Title,
                    SectionContent = AppResources.TosSection13Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection14Title,
                    SectionContent = AppResources.TosSection14Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection15Title,
                    SectionContent = AppResources.TosSection15Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection16Title,
                    SectionContent = AppResources.TosSection16Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection17Title,
                    SectionContent = AppResources.TosSection17Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection18Title,
                    SectionContent = AppResources.TosSection18Content,
                },
                
                new TosItem()
                {
                    SectionTitle = AppResources.TosSection19Title,
                    SectionContent = AppResources.TosSection19Content,
                }
            };
        }

        public async void Back()
        {
            await view.Back();
        }
    }
}