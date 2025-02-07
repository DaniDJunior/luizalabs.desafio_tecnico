using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;

namespace luizalabs.desafio_tecnico.Extensions
{
    public static class InjectionExtensions
    {
        public static void ApplyInjections(this IServiceCollection services)
        {
            services.AddTransient<IKafkaManager, KafkaManager>();

            services.AddTransient<IAuthManager, AuthManager>();

            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ITokenAdapter, TokenAdapter>();

            services.AddTransient<ILegacyManager, LegacyManager>();
            services.AddTransient<ILegacyKafkaManager, LegacyKafkaManager>();
            services.AddTransient<ILegacyAdapter, LegacyAdapter>();
        }
    }
}
