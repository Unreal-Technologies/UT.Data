using UT.Data.IO;

namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnSequentialExecutionConfiguration(SequentialExecution se);
        public void OnClientConfiguration(Client client);
        public void OnServerConfiguration(Server server);
    }
}
