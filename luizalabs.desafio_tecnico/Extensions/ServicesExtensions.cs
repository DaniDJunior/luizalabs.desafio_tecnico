using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Managers;
using luizalabs.desafio_tecnico.Services;

namespace luizalabs.desafio_tecnico.Extensions
{
    public static class ServicesExtensions
    {
        public static void ApplyServices(this IServiceCollection services)
        {
            services.AddHostedService<LegacyFileKafkaService>();
            services.AddHostedService<LegacyLineKafkaService>();
            services.AddHostedService<LegacyDataKafkaService>();
            services.AddHostedService<LegacyFinalKafkaService>();
        }
    }
}
