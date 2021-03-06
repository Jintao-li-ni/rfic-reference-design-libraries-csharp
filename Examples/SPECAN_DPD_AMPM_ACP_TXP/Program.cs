﻿using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application SPECAN_DPD_AMPM_ACP_TXP\n");
            SPECAN_DPD_AMPM_ACP_TXP specan_DPD_AMPM_ACP_TXP = new SPECAN_DPD_AMPM_ACP_TXP();
            try
            {
                specan_DPD_AMPM_ACP_TXP.Run();
              
            }
            catch (Exception e)
            {

                DisplayError(e);
            }
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
        static void DisplayError(Exception e)
        {
            Console.WriteLine("ERROR:\n" + e.GetType() + ": " + e.Message);
        }
    }
}