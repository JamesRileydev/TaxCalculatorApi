using Autofac;
using TaxCalculator.Api.Data;
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

            builder.RegisterType<MockRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<OrderCreationService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
