using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Domain.Repositories
{
    public interface IFirmwareUpdateRepository
    {
        Task FirmwareUpdateAsync(string filePath, IProgress<int>? progress = null, CancellationToken cancellationToken = default);
    }
}
