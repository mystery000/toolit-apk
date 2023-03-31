using Toolit.ViewModels;
using Xamarin.Forms;

namespace Toolit.Models.Layouts
{
    class WelcomeCarouselItemSelector : DataTemplateSelector
    {
        public DataTemplate WelcomeTemplate { get; set; }
        public DataTemplate OpenBankIdTemplate { get; set; }
        public DataTemplate LoginTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is WelcomeViewModel.CarouselItem welcomeItem)
            {
                switch (welcomeItem.Type)
                {
                    case WelcomeViewModel.WelcomeItemType.OnBoarding:
                        return WelcomeTemplate;
                    case WelcomeViewModel.WelcomeItemType.BankId:
                        return OpenBankIdTemplate;
                    case WelcomeViewModel.WelcomeItemType.Login:
                        return LoginTemplate;
                }
            }

            return WelcomeTemplate;
        }
    }
}
