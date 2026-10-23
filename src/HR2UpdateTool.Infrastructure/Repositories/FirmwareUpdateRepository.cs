using HR2UpdateTool.Application.Common;
using HR2UpdateTool.Application.Interface;
using HR2UpdateTool.Domain.Repositories;
using HR2UpdateTool.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Infrastructure.Repositories
{
    public class FirmwareUpdateRepository : IFirmwareUpdateRepository
    {
        private readonly ISerialMessenger _serialMessenger;

        public FirmwareUpdateRepository(ISerialMessenger serialMessenger)
        {
            _serialMessenger = serialMessenger;
        }

        public async Task FirmwareUpdateAsync(string filePath, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("file path is required");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("firmware file is not exist", filePath);

            byte[] bootCmd =
                [0x35, 0x76, 0x82, 0xF0, 0x12, 0xBC, 0x2E, 0x61, 0x36, 0x4D,
                 0xBE, 0xAB, 0xEE, 0x1A, 0x38, 0x6A, 0xC8, 0xBB, 0x44, 0x3F,
                 0x83, 0x29, 0x9C, 0x1F, 0x4B, 0xD7, 0x44, 0xD7, 0x2D, 0x7E,
                 0xDE, 0x97, 0x79, 0xAA, 0x3B, 0x07, 0x27, 0x41, 0x8C, 0x37,
                 0x8F, 0x38, 0xDE, 0xDB, 0x51, 0xFF, 0x83, 0xDA, 0x55, 0xE2,
                 0xB9, 0x4A, 0xB6, 0x2E, 0xF6, 0xE8, 0x9D, 0x6A, 0xCA, 0xCF,
                 0xB8, 0x17, 0xA6, 0xD3, 0x65, 0x41, 0xAE, 0x1D, 0xEB, 0xD4
                ];

            await _serialMessenger.SendRawDataAndAwaitResponseAsync(bootCmd, buffer=> buffer[5] == 1);
            //await Task.Delay(500);

            byte[] firmware = await File.ReadAllBytesAsync(filePath, cancellationToken);

            if (firmware.Length < 2048) throw new InvalidDataException("firmware file is invalid");

            byte[] trimmedFirmware = firmware.Skip(2048).ToArray();

            const int chunkSize = 512;
            int totalSize = trimmedFirmware.Length;
            int sentBytes = 0;

            for (int offset = 0; offset < totalSize; offset += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int remaining = totalSize - offset;
                int currentChunkSize = Math.Min(remaining, chunkSize);

                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(trimmedFirmware, offset, chunk, 0, currentChunkSize);

                var packet = await _serialMessenger.SendCommandAndAwaitResponseAsync(CommandConstants.FW_WRITE, chunk);
                var payload = packet.Payload;
                var isSuccess = payload[0];

                if (isSuccess != 0x01) throw new InvalidDataException($"status : {isSuccess}");
                sentBytes += currentChunkSize;

                if (progress != null)
                {
                    int percent = (int)((double)sentBytes / totalSize * 100);
                    progress.Report(percent);
                }
            }

            await _serialMessenger.SendCommandAndAwaitResponseAsync(CommandConstants.FW_DOWNLOAD_FINISH);
        }
    }
}
