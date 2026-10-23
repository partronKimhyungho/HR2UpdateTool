using HR2UpdateTool.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.UseCases
{
    public abstract class BaseUseCase<Params, T>
    {
        public async Task<Result<T>> ExecuteAsync(Params parameters, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await BuildAsync(parameters, cancellationToken);
                return Result<T>.Success(data);
            }
            catch (TimeoutException ex)
            {
                return Result<T>.Error($"장비 응답 시간 초과: {ex.Message}", ex);
            }
            catch (OperationCanceledException ex)
            {
                return Result<T>.Error("작업이 취소되었습니다.", ex);
            }
            catch (Exception ex)
            {
                return Result<T>.Error($"통신 오류 발생: {ex.Message}", ex);
            }
        }

        protected abstract Task<T> BuildAsync(Params parameters, CancellationToken cancellationToken);
    }
}
