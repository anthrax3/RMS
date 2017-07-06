﻿namespace RMS.Model
{
    //--Ataur-->
    public class AdvanceSaleAmount:CommonModel
    {
        public int SaleAdvanceId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerUsername { get; set; }
        public byte TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
