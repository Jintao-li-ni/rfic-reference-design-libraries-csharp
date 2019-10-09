﻿using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxInstr
    {
        #region Type Definitions
        public struct InstrumentConfiguration
        {
            public LocalOscillatorConfiguration[] LoConfigurations;

            public static InstrumentConfiguration GetDefault()
            {
                return new InstrumentConfiguration
                {
                    LoConfigurations = new LocalOscillatorConfiguration[] { LocalOscillatorConfiguration.GetDefault() }       
                };
            }

            public static InstrumentConfiguration GetDefault(RFmxInstrMX sessionHandle)
            {
                InstrumentConfiguration instrConfig = GetDefault(); // covers case for sub6 instruments with a single configurable LO
                // lo configuration will now be overridden if the instrument has multiple configurable LOs
                sessionHandle.GetInstrumentModel("", out string instrumentModel);
                switch (instrumentModel)
                {
                    case "NI PXIe-5830":
                    case "NI PXIe-5831":
                        LocalOscillatorConfiguration lo1Config = LocalOscillatorConfiguration.GetDefault();
                        lo1Config.ChannelName = "LO1";
                        LocalOscillatorConfiguration lo2Config = LocalOscillatorConfiguration.GetDefault();
                        lo2Config.ChannelName = "LO2";
                        instrConfig.LoConfigurations = new LocalOscillatorConfiguration[] { lo1Config, lo2Config };
                        break;
                }
                return instrConfig;
            }
        }
        #endregion

        #region Instrument Configurations
        public static void ConfigureInstrument(RFmxInstrMX instrHandle, InstrumentConfiguration instrConfig)
        {
            foreach (LocalOscillatorConfiguration loConfig in instrConfig.LoConfigurations)
            {
                switch (loConfig.SharingMode)
                {
                    case LocalOscillatorSharingMode.None:
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, false);
                        instrHandle.SetLOSource(loConfig.ChannelName, RFmxInstrMXConstants.LOSourceOnboard);
                        break;
                    case LocalOscillatorSharingMode.Manual:
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Disabled);
                        instrHandle.SetLOExportEnabled(loConfig.ChannelName, loConfig.ExportEnabled);
                        instrHandle.SetLOSource(loConfig.ChannelName, loConfig.Source);
                        break;
                    default: // default to automatic case
                        instrHandle.SetAutomaticSGSASharedLO(loConfig.ChannelName, RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOExportEnabled);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.LOSource);
                        break;
                }

                switch (loConfig.OffsetMode)
                {
                    case LocalOscillatorOffsetMode.NoOffset:
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                        instrHandle.SetDownconverterFrequencyOffset(loConfig.ChannelName, 0.0);
                        break;
                    case LocalOscillatorOffsetMode.UserDefined:
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.False);
                        instrHandle.SetDownconverterFrequencyOffset(loConfig.ChannelName, loConfig.Offset_Hz);
                        break;
                    default: // default to automatic case
                        instrHandle.SetLOLeakageAvoidanceEnabled(loConfig.ChannelName, RFmxInstrMXLOLeakageAvoidanceEnabled.True);
                        instrHandle.ResetAttribute(loConfig.ChannelName, RFmxInstrMXPropertyId.DownconverterFrequencyOffset);
                        break;
                }
            }
        }
        #endregion
    }
}

