using System.IO;
using BattleServer.Networking;
using BattleServer.Storage;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;

namespace BattleServer
{
    public class ApplicationServer : ApplicationBase
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new UnityClient(initRequest, UserManager.Init(Path.Combine(BinaryPath, "data")));
        }

        protected override void Setup()
        {
            var file = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }
        }

        protected override void TearDown()
        {
        }
    }
}