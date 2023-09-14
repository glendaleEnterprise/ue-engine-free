using Microsoft.EntityFrameworkCore;

namespace Glendale.Design.DataAnnotations.Conventions
{
    public static class DecimalPrecisionAttributeConvention
    {
        public static void Apply(ModelBuilder builder)
        {
            ConventionBehaviors.SetTypeForPropertiesWithAttribute<DecimalPrecisionAttribute>(builder, x => $"decimal({x.Precision}, {x.Scale})");
        }
    }
}
