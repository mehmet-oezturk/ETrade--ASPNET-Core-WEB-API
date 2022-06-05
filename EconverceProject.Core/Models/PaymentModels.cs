using System;
using System.ComponentModel.DataAnnotations;

namespace EconverceProject.Core.Models
{   public class PaymentModel
    {
     
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
    
        public string Type { get; set; }
       
        public string InvoiceAddress { get; set; }
       
        public string ShippeAdress { get; set; }
        public bool IsCompleted { get; set; }

        public int? CartId { get; set; }
        public int? AccountId { get; set; }
    }
    public class PayModel
    {   [Required]
        [CreditCard]

        public string CartNumber { get; set; }
        [Required]
        [StringLength(40)]
        public string CardName { get; set; }

        //04/23
        [Required]
        [StringLength(5)]
        [RegularExpression(@"^\d{2}\/\d{2}$")]//2 karakter sayı / 2 karakter sayı beklidiğmizi belirtiyoruz 04/23 gibi
        public string ExpireDate { get; set; }
        [Required]
        [StringLength(3)]// 3 karakter sayı
        [RegularExpression(@"^\d{3}$")]//
        public string CVV { get; set; }
        public decimal? TotalPriceOverride { get; set; }
        [StringLength(25)]
        public string Type { get; set; }
        [Required]
        [StringLength(160)]
        public string InvoiceAddress { get; set; }
        [Required]
        [StringLength(160)]
        public string ShippeAdress { get; set; }
    }
}
