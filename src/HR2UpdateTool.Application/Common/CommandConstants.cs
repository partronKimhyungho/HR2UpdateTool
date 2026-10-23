using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Common
{
    public static class CommandConstants
    {
        public const byte STX1 = 0xAA;
        public const byte STX2 = 0x55;

        public const byte READ_FW_VERSION = 0x10;
        public const byte SET_TEST_MODE = 0x20;
        public const byte SET_RF_SOC_CHANNEL = 0x30;
        public const byte SET_RF_SOC_TRX = 0x31;
        public const byte READ_RF_SOC_RESULT = 0x32;
        public const byte READ_CONNECTION_STATE = 0x60;
        public const byte READ_SETTING_DATA = 0x70;
        public const byte WRITE_SETTING_DATA = 0x80;
        public const byte WRITE_COMPANY_ID = 0x81;
        public const byte WRITE_ARTIST_ID = 0x82;
        public const byte READ_ARTIST_ID = 0x92;
        public const byte WRITE_DMX_CHANNEL = 0x83;
        public const byte WRITE_CRYPTO_FLAG = 0x84;
        public const byte WRITE_CONCERT_ID = 0x85;
        public const byte READ_BTN_INFO = 0x90;
        public const byte WRITE_BTN_INFO = 0x91;
        public const byte SAVE_BTN_INFO = 0x92;
        public const byte READ_BTN_RGB = 0x93;
        public const byte WRITE_BTN_RGB = 0x94;

        public const byte ENTER_CONTROLLER_BOOT = 0XA0;
        public const byte ENTER_RF_SOC1_BOOT = 0XA1;
        public const byte ENTER_RF_SOC2_BOOT = 0xA2;

        public const byte FW_WRITE = 0xF0;
        public const byte FW_DOWNLOAD_FINISH = 0xF1;

    }
}
