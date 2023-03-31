using System;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;

namespace Toolit.Extensions
{
    public static class PricingExtension
    {
        public static decimal CalculateVat(this Bid bid)
        {
            return bid.ToBidUiModel().CalculateVat();
        }
        
        public static decimal CalculateRootDeduction(this Bid bid)
        {
            return bid.ToBidUiModel().CalculateRootDeduction();
        }
        
        public static decimal CalculateVat(this BidUiModel bid)
        {
            if (bid != null)
            {
                return (bid.MaterialCost + bid.LabourCost) * AppConstants.VatRate;
            }
            return decimal.Zero;
        }
        
        public static decimal CalculateRootDeduction(this BidUiModel bid)
        {
            if (bid != null)
            {
                return bid.LabourCost * (1 + AppConstants.VatRate) * AppConstants.BaseRootDeductible;
            }
            return decimal.Zero;
        }
    }
}