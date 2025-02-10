using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Sample
{
    internal class LegacyRequestSample
    {
        public static Models.Legacy.LegacyRequest Sample()
        {
            Models.Legacy.LegacyRequest request = new Models.Legacy.LegacyRequest();

            request.request_id = Guid.NewGuid();
            request.file_name = ComunSample.RandomString(45);
            request.total_lines = ComunSample.RandomInt();
            request.status = Enuns.LegacyFileStatus.RECEIVED;

            return request;
        }
    }
}
