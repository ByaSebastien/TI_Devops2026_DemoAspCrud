namespace TI_Devops2026_DemoAspCrud.Models
{
    public record ProductIndexResponse(
        int Id,
        string Name,
        int Price,
        string CategoryName
    );
}
