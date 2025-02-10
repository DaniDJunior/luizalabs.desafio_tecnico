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

        public static Models.Legacy.LegacyRequest SampleAddLine(Models.Legacy.LegacyRequest request)
        {
            Models.Legacy.LegacyRequestLine requestLine = new Models.Legacy.LegacyRequestLine();

            requestLine.request_id = request.request_id;
            requestLine.request = request;
            requestLine.product_value = ComunSample.RandomFloat();
            requestLine.line_number = ComunSample.RandomInt();
            requestLine.product_id = ComunSample.RandomInt();
            requestLine.order_date = DateTime.Now;
            requestLine.order_id = ComunSample.RandomInt();
            requestLine.user_id = ComunSample.RandomInt();
            requestLine.user_name = ComunSample.RandomString(45);
            requestLine.request_line_id = Guid.NewGuid();

            request.lines.Add(requestLine);

            return request;
        }
    }
}
