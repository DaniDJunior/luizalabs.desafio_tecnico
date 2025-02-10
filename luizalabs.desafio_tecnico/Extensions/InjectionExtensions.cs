using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Communications;
using luizalabs.desafio_tecnico.Data;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Logics;
using luizalabs.desafio_tecnico.Managers;

namespace luizalabs.desafio_tecnico.Extensions
{
    public static class InjectionExtensions
    {
        public static void ApplyInjections(this IServiceCollection services)
        {
            services.AddTransient<IKafkaCommunication, KafkaCommunication>();

            services.AddTransient<IAuthManager, AuthManager>();

            services.AddTransient<ITokenLogic, TokenLogic>();
            services.AddTransient<ITokenAdapter, TokenAdapter>();

            services.AddTransient<ILegacyData, LegacyData>();
            services.AddTransient<ILegacyLogic, LegacyLogic>();
            services.AddTransient<ILegacyAdapter, LegacyAdapter>();

            services.AddTransient<IUserData, UserData>();
            services.AddTransient<IUserAdapter, UserAdapter>();

            services.AddTransient<IOrderData, OrderData>();
            services.AddTransient<IOrderAdapter, OrderAdapter>();
        }
    }
}
