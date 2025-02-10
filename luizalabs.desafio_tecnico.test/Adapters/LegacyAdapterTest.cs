using luizalabs.desafio_tecnico.Adapters;
using luizalabs.desafio_tecnico.Models.Legacy;
using luizalabs.desafio_tecnico.Models.Order;
using luizalabs.desafio_tecnico.test.LocalMock.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.Adapters
{
    [TestClass]
    public class LegacyAdapterTest
    {
        private LegacyAdapter LegacyAdapter;

        [TestInitialize]
        public void TestInitialize()
        {
            LegacyAdapter = new LegacyAdapter();
        }

        [TestMethod]
        public void ToView()
        {
            LegacyRequest request = LegacyRequestSample.Sample();
            LegacyView requestVies = LegacyAdapter.ToView(request);

            Assert.AreEqual(request.request_id, requestVies.request_id);
            Assert.AreEqual(request.file_name, requestVies.file_name);
        }

        [TestMethod]
        public void ToListView()
        {
            int sampleSize = 5;

            List<LegacyRequest> requests = new List<LegacyRequest>();

            for (int i = 0; i < sampleSize; i++)
            {
                requests.Add(LegacyRequestSample.Sample());
            }

            List<LegacyView> requestsVies = LegacyAdapter.ToListView(requests);

            Assert.AreEqual(requests.Count, requestsVies.Count);

            for (int i = 0; i < sampleSize; i++)
            {
                Assert.AreEqual(requests[i].request_id, requestsVies[i].request_id);
                Assert.AreEqual(requests[i].file_name, requestsVies[i].file_name);
            }
        }
    }
}
