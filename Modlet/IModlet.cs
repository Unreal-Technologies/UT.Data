using Microsoft.EntityFrameworkCore;
using System.Net;

namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnClientConfiguration(Form? form);
        public void OnGlobalServerAction(byte[]? stream, IPAddress ip);
        public byte[]? OnLocalServerAction(byte[]? stream, IPAddress ip);
        public void OnServerConfiguration(DbContext? context);
        public void OnServerInstallation(DbContext? context);
    }
}
