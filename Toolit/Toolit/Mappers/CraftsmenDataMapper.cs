using System;
using System.Collections.Generic;
using System.Linq;
using Toolit.Models.Ui;

namespace Toolit.Mappers
{
    public static class CraftsmenDataMapper
    {
        public static CraftsmanUiModel ToCraftsmanUiModel(this Craftsman mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new CraftsmanUiModel()
            {
                Id = mdl.Id,
                OfficeId = mdl.OfficeId,
                UserId = mdl.UserId,
                
                Crafts = new List<CraftUiModel>(mdl.Crafts?.Select(crft => 
                    crft.ToCraftUiModel()) ?? Array.Empty<CraftUiModel>()),
                Ratings = new List<RatingUiModel>(mdl.Ratings?.Select(rtng => 
                    rtng.ToRatingUiModel()) ?? Array.Empty<RatingUiModel>()),
                AboutHeader = mdl.AboutHeader,
                AboutText = mdl.AboutText,
                AccountNumber = mdl.AccountNumber,
                CompanyAddress = mdl.CompanyAddress,
                CompanyName = mdl.CompanyName,
                CompletedJobs = mdl.CompletedJobs,
                CraftsmanName = mdl.CraftsmanName,
                MemberSince = mdl.MemberSince.FromRFC3339(),
                OrgNumber = mdl.OrgNumber,
                WorkArea = mdl.WorkArea,
                FTax = mdl.FTax,
                IsDeleted = mdl.Deleted,
                IsFrozen = mdl.Frozen
            };
        }

        public static RatingUiModel ToRatingUiModel(this Rating mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new RatingUiModel()
            {
                Id = mdl.Id,
                CraftsmanId = mdl.CraftsmanId,
                OfficeId = mdl.OfficeId,
                TaskId = mdl.TaskId,
                UserId = mdl.UserId,

                Amount = mdl.Amount,
                Created = mdl.Created.FromRFC3339(),
                Header = mdl.Header,
                Text = mdl.Text
            };
        }

        public static CraftUiModel ToCraftUiModel(this Craft mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new CraftUiModel()
            {
                Id = mdl.Id,
                CertificateId = mdl.CertificateId,
                
                Status = mdl.Status,
                CraftType = mdl.CraftType
            };
        }
    }
}