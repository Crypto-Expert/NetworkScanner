using System.Threading.Tasks;

namespace NetworkScanner.CLI.Chains
{
    public interface IChain
    {
        Task Connect();
        void LoadChainData();
        void SaveChainData();
        Task StopAsync();
    }
}
