using Toolit.Models.Misc;
using Xamarin.Forms;

namespace Toolit.Helpers.Ui
{
    public class TaskMediaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PhotoTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is MediaFile media)
            {
                return media.IsVideo ? VideoTemplate : PhotoTemplate;
            }

            return null;
        }
    }
}