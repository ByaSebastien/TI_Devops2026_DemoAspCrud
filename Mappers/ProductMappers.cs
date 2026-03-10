using TI_Devops2026_DemoAspCrud.Entities;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Mappers
{
    public static class ProductMappers
    {
        public static ProductIndexResponse ToProductIndexResponse(this Product p)
        {
            return new ProductIndexResponse
            (
                p.Id,
                p.Name,
                p.Price,
                p.Category.Name
            );
        }
    }
}
