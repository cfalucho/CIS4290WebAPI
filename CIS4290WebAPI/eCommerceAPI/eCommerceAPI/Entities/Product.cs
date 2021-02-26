using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCommerceAPI.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int SubCategoryID { get; set; }
        public string Featured { get; set; }
        public int MainCategoryID { get; set; }
    }
}
