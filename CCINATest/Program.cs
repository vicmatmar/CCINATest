using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using FTD2XX_NET;
using CINALib;

namespace CCINATest
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] currentTesterAddresses = new byte[] { 0x40, 0x41, 0x44, 0x45 };

            FTDI ftdi = new FTDI();
            int count = 10;
            FTDI.FT_DEVICE_INFO_NODE[] devlist = new FTDI.FT_DEVICE_INFO_NODE[count];
            FTDI.FT_STATUS status = ftdi.GetDeviceList(devlist);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Problem getting FTDI device list");
            }

            for (int i = 0; i < count; i++)
            {
                FTDI.FT_DEVICE_INFO_NODE devinfo = devlist[i];
                if (devinfo != null)
                {
                    if (devinfo.Type == FTDI.FT_DEVICE.FT_DEVICE_232H)
                    {
                        string msg = string.Format("{0}. {1}", i+1, devinfo.Description);
                        Console.WriteLine(msg);

                        FTI2C currentController = new FTI2C();
                        currentController.Init(i);

                        Cina219 cina = new Cina219(currentController);
                        foreach (var currentTesterAddress in currentTesterAddresses)
                        {
                            var cina219 = new Cina219(currentController, currentTesterAddress);

                            try
                            {
                                cina219.Init();
                                Console.WriteLine(string.Format("0x{0:X} => Sensor found", currentTesterAddress));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("0x{0:X} => {1}", currentTesterAddress, ex.Message));
                                continue;
                            };
                        }
                        Console.WriteLine();
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
