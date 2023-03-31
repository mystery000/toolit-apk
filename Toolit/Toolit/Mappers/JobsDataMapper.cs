using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Mappers
{
    public static class JobsDataMapper
    {
        public static TaskUiModel ToTaskUiModel(this Task mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            var newMdl = new TaskUiModel()
            {
                Id = mdl.Id,
                OfficeId = mdl.OfficeId,
                UserId = mdl.UserId,
                PaymentId = mdl.PaymentId,

                Title = mdl.Title,
                Description = mdl.Description,
                DateDone = mdl.DateDone.FromRFC3339(),
                Modified = mdl.Modified,
                Address = mdl.Address,
                City = mdl.City,
                Postcode = mdl.Postcode,
                Price = mdl.Price,
                Crafts = new List<string>(mdl.Crafts ?? new string[] {}),
                Tags = new List<string>(mdl.Tags ?? new string[] {}),
                ImageUrls = mdl.ImageSources?.Select(src => (src as UriImageSource)?.Uri.ToString()).ToList() ?? new List<string>(),
                Finished = mdl.Finished ?? false,
                Rated = mdl.Rated,
                AcceptedBid = mdl.AcceptedBid,
                PropertyDesignation = mdl.PropertyDesignation,
                RealEstateUnion = mdl.RealestateUnion,
                ApartmentNumber = mdl.ApartmentNumber,
                UseRotRut = mdl.UseRotRut,
                ShowPublicly = mdl.ShowPublicly,
                VideoUrls = mdl.VideoSources?.Select(src => (src as UriImageSource)?.Uri.ToString()).ToList() ?? new List<string>()
            };

            // title image rules: if there are videos attached, then the first image in imageUrls is first video's thumbnail
            // if there are no other images attached, use that thumbnail, otherwise use first image after thumbnail
            // either way, the thumbnail should be removed from imageUrls once the title image has been assigned
            if (newMdl.VideoUrls.Any())
            {
                if (newMdl.ImageUrls.Any())
                {
                    newMdl.TitleImageUrl = newMdl.ImageUrls.Count > 1 ? newMdl.ImageUrls[1] : newMdl.ImageUrls[0];
                    newMdl.ImageUrls.RemoveAt(0);
                }
            }
            else
            {
                newMdl.TitleImageUrl = newMdl.ImageUrls.FirstOrDefault();
            }

            return newMdl;
        }

        public static AdUiModel ToAdUiModel(this Ad mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new AdUiModel()
            {
                Id = mdl.Id,
                
                Company = mdl.Company,
                Title = mdl.Title,
                Text = mdl.Text,
                Url = mdl.Url,
                ImageUrl = (mdl.ImageSource as UriImageSource)?.Uri.ToString() ?? string.Empty,
                Modified = mdl.Modified
            };
        }

        public static BidUiModel ToBidUiModel(this Bid mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new BidUiModel()
            {
                Id = mdl.Id,
                TaskId = mdl.TaskId,
                OfficeId = mdl.OfficeId,
                CraftsmanId = mdl.CraftsmanId,

                BidMessage = mdl.BidMessage,
                FinalBid = mdl.FinalBid,
                LabourCost = mdl.LabourCost,
                MaterialCost = mdl.MaterialCost,
                Vat = mdl.Vat,
                RootDeduction = mdl.RootDeduction
            };
        }
    }
}