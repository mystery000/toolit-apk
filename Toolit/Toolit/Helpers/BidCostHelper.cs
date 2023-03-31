using System;
using Toolit.Models.Ui;
using Toolit.Resourses;

namespace Toolit.Helpers
{
    public static class BidCostHelper
    {
        public static BidUiModel ConstructNewBidModel(decimal laborCost, decimal materialCost, bool isRotRut, decimal brokerageFee)
        {
            laborCost = Math.Round(laborCost, 2, MidpointRounding.ToEven);
            materialCost = Math.Round(materialCost, 2, MidpointRounding.ToEven);

            var newBidMdl = new BidUiModel()
            {  
                LabourCost = laborCost,
                MaterialCost = materialCost,
            };

            var laborCostVat = Math.Round(laborCost * AppConstants.VatRate, 2, MidpointRounding.ToEven);
            var laborCostIncVat = laborCost + laborCostVat;

            var rotRate = isRotRut ? AppConstants.BaseRootDeductible : Decimal.Zero;
            var deduction = Math.Round(laborCostIncVat * rotRate, 2, MidpointRounding.ToEven);

            var materialCostVat = Math.Round(materialCost * AppConstants.VatRate, 2, MidpointRounding.ToEven);
            var materialCostIncVat = materialCost + materialCostVat;

            var vat = (laborCostVat - (laborCostVat * rotRate)) + materialCostVat;
            var total = laborCostIncVat + materialCostIncVat - deduction;

            newBidMdl.RootDeduction = deduction;
            newBidMdl.Vat = vat;
            newBidMdl.FinalBid = total;
            newBidMdl.BrokerageFee = brokerageFee;

            return newBidMdl;
        }
        
    }
}