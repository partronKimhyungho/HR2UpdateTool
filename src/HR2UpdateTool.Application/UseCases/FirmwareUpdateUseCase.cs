using HR2UpdateTool.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.UseCases
{
    public class FirmwareUpdateUseCase : BaseUseCase<FirmwareUpdateUseCase.Params, bool>
    {
        private readonly IFirmwareUpdateRepository _firmwareUpdateRepository;

        public FirmwareUpdateUseCase(IFirmwareUpdateRepository firmwareUpdateRepository)
        {
            _firmwareUpdateRepository = firmwareUpdateRepository;
        }

        public record Params(string FilePath, IProgress<int>? Progress = null);

        protected override async Task<bool> BuildAsync(Params parameters, CancellationToken cancellationToken)
        {
            await _firmwareUpdateRepository.FirmwareUpdateAsync(
                parameters.FilePath,
                parameters.Progress,
                cancellationToken
                );

            return true;
        }
    }
}
