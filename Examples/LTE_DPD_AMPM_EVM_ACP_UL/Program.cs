﻿using System;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class Program
    {
        /// <summary>
        /// This example illustrates how to use the RFmxLTE and RFSG drivers to perform AMPM, EVM and ACP measurements with or without digital predistortion (DPD) of the LTE uplink waveform input to the DUT.
        /// Before executing this application, please check and ensure you have the right configurations in the InitializeParameters() function.
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Example Application LTE_DPD_AMPM_EVM_ACP_UL\n");
            LTE_DPD_AMPM_EVM_ACP_UL lteDpdAmPmEvmAcpUl = new LTE_DPD_AMPM_EVM_ACP_UL();
            try
            {
                lteDpdAmPmEvmAcpUl.Run();              
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