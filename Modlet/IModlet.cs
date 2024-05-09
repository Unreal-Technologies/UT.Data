using Microsoft.EntityFrameworkCore;
using System.Net;
using UT.Data.Efc;

namespace UT.Data.Modlet
{
    public interface IModlet
    {
        public void OnClientConfiguration(Form? form, ModletClient client, Session session);
        public void OnGlobalServerAction(byte[]? stream, IPAddress ip);
        public byte[]? OnLocalServerAction(byte[]? stream, IPAddress ip);
        public void OnServerConfiguration(ServerContext? context);
        public void OnServerInstallation(ServerContext? context);
    }
}
