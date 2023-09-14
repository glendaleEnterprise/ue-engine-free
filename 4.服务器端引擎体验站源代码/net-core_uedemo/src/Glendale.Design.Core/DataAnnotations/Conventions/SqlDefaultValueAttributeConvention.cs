using Microsoft.EntityFrameworkCore;
using System;

namespace Glendale.Design.DataAnnotations.Conventions
{
    public static class SqlDefaultValueAttributeConvention
    {
        public static void Apply(ModelBuilder builder)
        {
            ConventionBehaviors
                .SetSqlValueForPropertiesWithAttribute<SqlDefaultValueAttribute>(builder, (Func<SqlDefaultValueAttribute, string>)(x => x.DefaultValue));
        }
    }
}
