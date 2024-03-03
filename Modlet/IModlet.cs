using UT.Data.DBE;

namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnSequentialExecutionConfiguration(SequentialExecution se);
        public void OnClientConfiguration(ModletClient client);
        public void OnGlobalServerAction(byte[]? stream);
        public byte[]? OnLocalServerAction(byte[]? stream);
        public void OnServerConfiguration(IDatabaseConnection? dbc, ref Dictionary<string, object?> configuration);
        public void OnServerInstallation(IDatabaseConnection? dbc);
    }
}
