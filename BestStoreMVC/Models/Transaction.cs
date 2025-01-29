using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankTransaction.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {  get; set; }

        [Column(TypeName = "nvarchar(12)")]
        [DisplayName("Account Number")]
        [Required(ErrorMessage ="This field is required.")]
        [MaxLength(12,ErrorMessage ="Maximum 12 characters only")]
        public string AccountNumber { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Beneficiary Name")]
        [Required(ErrorMessage = "This field is required.")]

        public string BeneficiaryName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Bank Name")]
        [Required(ErrorMessage = "This field is required.")]

        public string BankName { get; set; }

        
        [Column(TypeName = "nvarchar(11)")]
        [DisplayName("Swift Code")]
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(11, ErrorMessage = "Maximum 11 characters only")]
        public string SwiftCode { get; set; }

        [Column(TypeName ="nvarchar(100)")]
        [DisplayName("Tags")]
        [Required(ErrorMessage ="This field is requiered")]
        public String Tags { get; set; }    

        [Required(ErrorMessage = "This field is required.")]
        public int Amount { get; set; }

        [DisplayFormat(DataFormatString ="{0:MMM-dd-yy}")]
        public DateTime Date {  get; set; }


    }
}
