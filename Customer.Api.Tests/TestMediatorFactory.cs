using Customer.Api.Controllers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Api.Tests
{
    public static class TestMediatorFactory
    {
        public static IMediator BuildMediator(CustomerContext context)
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(CustomersController));
            services.AddScoped<CustomerContext>(x => context);

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IMediator>();
        }
    }
}