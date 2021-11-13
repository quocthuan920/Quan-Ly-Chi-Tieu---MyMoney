﻿using System;

namespace MyMoney.ViewModels.Statistics
{
    public class PaymentsForCategoryMessage
    {
        public PaymentsForCategoryMessage(int categoryId, DateTime startdate, DateTime enddate)
        {
            CategoryId = categoryId;
            StartDate = startdate;
            EndDate = enddate;
        }

        public int CategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
