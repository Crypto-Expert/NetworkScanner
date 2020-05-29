using System.Threading.Tasks;
using NBitcoin.Protocol;

namespace NetworkScanner.CLI.Chains
{
    public interface IChain
    {
        Task Connect();
        void LoadChainData();
        void SaveChainData();
        void Stop();
    }
}
