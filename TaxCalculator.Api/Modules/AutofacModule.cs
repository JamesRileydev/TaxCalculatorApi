using Autofac;
using TaxCalculator.Api.Services;

namespace TaxCalculator.Api.Modules
{
    // ReSharper disable once IdentifierTypo
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TaxCalculationService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
