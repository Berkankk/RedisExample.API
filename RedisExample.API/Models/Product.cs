namespace RedisExample.API.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string? Name { get; set; } //Null olma ihtimalini ortadan kaldırdık
        public decimal Price { get; set; }
    }
}
