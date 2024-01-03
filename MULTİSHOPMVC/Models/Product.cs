namespace MULTİSHOPMVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string Inst { get; set; }
        public string Face { get; set; }
        public string Pin { get; set; }
        public string Tvit { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
       
        public List<ProductColor>? ProductColors { get; set; }
        
    }
}
