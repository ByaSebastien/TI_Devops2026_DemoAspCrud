namespace TI_Devops2026_DemoAspCrud.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
