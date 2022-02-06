using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public interface ILockfileProvider
    {
        public Task<Lockfile> GetLockfile(CancellationToken token);
    }
}
