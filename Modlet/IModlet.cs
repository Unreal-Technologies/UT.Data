namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnSequentialExecutionConfiguration(SequentialExecution se);
        public void OnClientConfiguration(ModletClient client);
        public void OnGlobalServerAction(byte[]? stream);
        public byte[]? OnLocalServerAction(byte[]? stream);
        public void OnServerConfiguration(ref Dictionary<string, object?> configuration);
    }
}
