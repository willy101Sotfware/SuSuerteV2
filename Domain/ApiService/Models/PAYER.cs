﻿namespace SuSuerteV2.Domain.ApiService.Models
{
    public partial class PAYER
    {
        public int PAYER_ID { get; set; }
        public string IDENTIFICATION { get; set; }
        public string NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<decimal> PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public bool STATE { get; set; }
        public string TYPE_PAYER { get; set; }
        public string TYPE_IDENTIFICATION { get; set; }
    }
}
