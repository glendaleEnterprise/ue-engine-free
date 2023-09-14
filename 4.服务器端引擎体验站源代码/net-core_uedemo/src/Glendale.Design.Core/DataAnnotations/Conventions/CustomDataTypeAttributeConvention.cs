using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.DataAnnotations.Conventions
{
    public static class CustomDataTypeAttributeConvention
    {
        public static void Apply(ModelBuilder builder)
        {
            ConventionBehaviors
                .SetTypeForPropertiesWithAttribute<DataTypeAttribute>(builder,
                    x => x.CustomDataType);
        }
    }
}
