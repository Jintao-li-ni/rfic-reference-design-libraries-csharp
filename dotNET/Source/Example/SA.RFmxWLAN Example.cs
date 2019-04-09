﻿using System;
using System.IO;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{    static class RFmxWLANExample
    {
        static void Main()
        {
            #region Configure Generation
            string resourceName = "VST2";
            string filePath = Path.GetFullPath(@"Support Files\80211a_20M_48Mbps.tdms");

            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            InstrumentConfiguration instrConfig = new InstrumentConfiguration();
            instrConfig.SetDefaults();
            instrConfig.CarrierFrequency_Hz = 2.412e9;

            ConfigureInstrument(ref nIRfsg, instrConfig);
            Waveform waveform = LoadWaveformFromTDMS(ref nIRfsg, filePath);

            DownloadWaveform(ref nIRfsg, ref waveform);

            WaveformTimingConfiguration timing = new WaveformTimingConfiguration
            {
                DutyCycle_Percent = 60,
                PreBurstTime_s = 1e-9,
                PostBurstTime_s = 1e-9,
            };

            PAENConfiguration paenConfig = new PAENConfiguration
            {
                PAEnableMode = PAENMode.Dynamic,
                PAEnableTriggerExportTerminal = "PFI0",
                PAEnableTriggerMode = RfsgMarkerEventOutputBehaviour.Toggle
            };

            ConfigureWaveformTimingAndPAControl(ref nIRfsg, ref waveform, timing, paenConfig, out double period, out _);
            nIRfsg.Initiate();
            #endregion

            RFmxInstrMX instr = new RFmxInstrMX("VST2", "");
            RFmxWlanMX wlan = instr.GetWlanSignalConfiguration();
            instr.GetWlanSignalConfiguration();


            CommonConfiguration commonConfiguration = new CommonConfiguration();
            commonConfiguration.SetDefaults();
            commonConfiguration.CenterFrequency_Hz = 2.412e9;

            AutoLevelConfiguration autoLevel = new AutoLevelConfiguration
            {
                //AutoLevelMeasureTime_s = period,
                AutoLevelReferenceLevel = true
            };

            SA.RFmxWLAN.ConfigureCommon(ref instr, ref wlan, commonConfiguration, autoLevel);

            SignalConfiguration signal = new SignalConfiguration();
            signal.SetDefaults();
            signal.AutoDetectSignal = false;
            signal.ChannelBandwidth_Hz = 80e6;
            signal.Standard = RFmxWlanMXStandard.Standard802_11ac;

            SA.RFmxWLAN.ConfigureSignal(ref wlan, signal);

            TxPConfiguration txpConfig = new TxPConfiguration
            {
                AveragingCount = 10,
                MaximumMeasurementInterval_s = waveform.BurstLength_s,
                AveragingEnabled = RFmxWlanMXTxpAveragingEnabled.True
            };

            SA.RFmxWLAN.ConfigureTxP(ref wlan, txpConfig);

            OFDMModAccConfiguration modAccConfig = new OFDMModAccConfiguration();
            modAccConfig.SetDefaults();
            modAccConfig.AveragingEnabled = RFmxWlanMXOfdmModAccAveragingEnabled.True;

            SA.RFmxWLAN.ConfigureOFDMModAcc(ref wlan, modAccConfig);

            TxPServoConfiguration servoConfig = new TxPServoConfiguration();
            servoConfig.SetDefaults();
            servoConfig.TargetTxPPower_dBm = 20.5;

            SA.RFmxWLAN.TxPServoPower(ref wlan, ref nIRfsg, servoConfig, autoLevel);

            wlan.Initiate("", "");

            TxPResults txpRes = SA.RFmxWLAN.FetchTxP(ref wlan);
            OFDMModAccResults modAccResults = SA.RFmxWLAN.FetchOFDMModAcc(ref wlan);
            Console.WriteLine("TXP Avg Power: {0:N}", txpRes.AveragePowerMean_dBm);
            Console.WriteLine("Composite RMS EVM (dB): {0:N}", modAccResults.CompositeRMSEVMMean_dB);
            Console.ReadKey();

            wlan.Dispose();
            instr.Close();

            TogglePFILine(ref nIRfsg, RfsgMarkerEventToggleInitialState.DigitalLow);
            CloseInstrument(ref nIRfsg);

        }
    }

}
