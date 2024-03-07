using Microsoft.EntityFrameworkCore;

namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnSequentialExecutionConfiguration(SequentialExecution se);
        public void OnClientConfiguration(Form? form);
        public void OnGlobalServerAction(byte[]? stream);
        public byte[]? OnLocalServerAction(byte[]? stream);
        public void OnServerConfiguration(DbContext? context, ref Dictionary<string, object?> configuration);
        public void OnServerInstallation(DbContext? context);
    }
}
