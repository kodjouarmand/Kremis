using Kremis.Utility.Options;
using Microsoft.Extensions.Options;

namespace Kremis.Api.Initializer
{
    public interface IDbInitializer
    {
        void Initialize(bool ensureDeleted = false);
    }
}