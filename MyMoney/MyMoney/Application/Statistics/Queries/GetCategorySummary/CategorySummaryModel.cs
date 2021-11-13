﻿using System.Collections.Generic;

namespace MyMoney.Application.Statistics.Queries.GetCategorySummary
{
    public class CategorySummaryModel
    {
        public CategorySummaryModel(decimal totalEarned,
                                    decimal totalSpent,
                                    List<CategoryOverviewItem> categoryOverviewItems)
        {
            TotalEarned = totalEarned;
            TotalSpent = totalSpent;
            CategoryOverviewItems = categoryOverviewItems;
        }

        public int CategoryId { get; set; }

        public decimal TotalEarned { get; set; }

        public decimal TotalSpent { get; set; }

        public List<CategoryOverviewItem> CategoryOverviewItems { get; set; }
    }
}
