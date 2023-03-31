using Toolit.Models.Layouts;
using Xamarin.Forms;

namespace Toolit.Helpers.Ui
{
    public class OtherJobTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TaskTemplate { get; set; }
        public DataTemplate AdTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is OtherJobLayoutModel job)
            {
                return job.IsAnAd ? AdTemplate : TaskTemplate;
            }

            return null;
        }
    }
}