// Models/Product.cs
using System.ComponentModel.DataAnnotations;

namespace ApiPraPostgreSQL.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; }

        public string? name { get; set; }

        public decimal? price { get; set; }
    }

    public class ProductIdRequest
    {
        public int Id { get; set; }
    }
}