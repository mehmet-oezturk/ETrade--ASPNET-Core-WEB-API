using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Models
{
    public class PaymentRequestModel
    {
        [Required]
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
        public decimal TotalPrice { get; set; }
    }
}
