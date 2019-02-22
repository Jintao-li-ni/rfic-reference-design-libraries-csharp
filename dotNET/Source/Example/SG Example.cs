﻿using System;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    class SGExample
    {
        static void Main()
        {
            string resourceName = "VST2";
            string filePath = @"C:\Users\SiLabs\Desktop\11ac_MCS9_BW_40M_IQ_80M.tdms";

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            InstrumentConfiguration instrConfig = new InstrumentConfiguration();
            instrConfig.SetDefaults();

            ConfigureInstrument(ref nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(ref nIRfsg, filePath);

            DownloadWaveform(ref nIRfsg, ref waveform);
            
            WaveformGenerationTiming timing = new WaveformGenerationTiming
            {
                DutyCycle_Percent = 20,
                PFIPortMode = WaveformGenerationTiming.PFIMode.Static,
                PreBurstTime_s = 1e-06,
                PostBurstTime_s = 1e-06,
            };

            CreatedAndDownloadScript(ref nIRfsg, ref waveform, timing, out _, out _);

            waveform = GetWaveformParametersByName(ref nIRfsg, waveform.WaveformName);

            TogglePFILine(ref nIRfsg, RfsgMarkerEventToggleInitialState.DigitalLow);
            CloseInstrument(ref nIRfsg);
        }
    }
}
