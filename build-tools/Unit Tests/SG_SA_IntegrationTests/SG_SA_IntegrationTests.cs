﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.ReferenceDesignLibraries;
using NationalInstruments.ReferenceDesignLibraries.SA;
using FluentAssertions;
using FluentAssertions.Execution;

namespace SG_SA_IntegrationTests
{
    [TestClass]
    public class SG_SA_IntegrationTests
    {
        static Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>> deviceSessions = new Dictionary<string, Tuple<NIRfsg, RFmxInstrMX>>();

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            var mis = new ModularInstrumentsSystem(typeof(NIRfsg));
            for (int i = 0; i < mis.DeviceCollection.Count; i++)
            {
                var device = mis.DeviceCollection[i];
                switch (device.Model)
                {
                    //case "NI PXIe-5646R":
                    //case "NI PXIe-5820":
                    case "NI PXIe-5840":
                    case "NI PXIe-5841":
                    case "NI PXIe-5830":
                    case "NI PXIe-5831":
                        if (!deviceSessions.ContainsKey(device.Model))
                        {
                            var rfsg = new NIRfsg(mis.DeviceCollection[i].Name, false, true);
                            var instr = new RFmxInstrMX(mis.DeviceCollection[i].Name, "");
                            deviceSessions.Add(device.Model, Tuple.Create(rfsg, instr));
                        }
                        break;
                }
            }
        }

        [TestMethod()]
        public void Test5840AutomaticLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-5840", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);
            rfsg.FrequencyReference.Source = RfsgFrequencyReferenceSource.OnboardClock;
            rfsg.Utility.Commit();

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);
            instrmx.SetFrequencyReferenceSource("", RFmxInstrMXConstants.OnboardClock);
            var specAn = instrmx.GetSpecAnSignalConfiguration();
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);

            using (new AssertionScope())
            {
                Action specAnCommit = () => specAn.Commit("");
                specAnCommit.Should().NotThrow<RFmxException>("exception will be thrown if LO resource is reserved.");

                instrmx.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instrmx.GetLOSource("", out string rfmxLOSource);
                instrmx.GetLOExportEnabled("", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceLOIn);
                rfmxLOExportEnabled.Should().BeFalse();

                rfsg.RF.LOOutExportConfigureFromRfsa.Should().Be(RfsgLOOutExportConfigureFromRfsa.Enabled);
                rfsg.RF.LocalOscillator.Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
            }
        }

        [TestMethod()]
        public void Test5841AutomaticLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-5841", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);
            rfsg.FrequencyReference.Source = RfsgFrequencyReferenceSource.OnboardClock;
            rfsg.Utility.Commit();

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);
            instrmx.SetFrequencyReferenceSource("", RFmxInstrMXConstants.OnboardClock);
            var specAn = instrmx.GetSpecAnSignalConfiguration();
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);

            using (new AssertionScope())
            {
                Action specAnCommit = () => specAn.Commit("");
                specAnCommit.Should().NotThrow<RFmxException>("exception will be thrown if LO resource is reserved.");

                instrmx.GetAutomaticSGSASharedLO("", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instrmx.GetLOSource("", out string rfmxLOSource);
                instrmx.GetLOExportEnabled("", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceLOIn);
                rfmxLOExportEnabled.Should().BeFalse();

                rfsg.RF.LOOutExportConfigureFromRfsa.Should().Be(RfsgLOOutExportConfigureFromRfsa.Enabled);
                rfsg.RF.LocalOscillator.Source.Should().Be(RfsgLocalOscillatorSource.Onboard);
            }
        }

        [TestMethod()]
        public void Test5830AutomaticLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-5830", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);
            rfsg.FrequencyReference.Source = RfsgFrequencyReferenceSource.OnboardClock;
            rfsg.Utility.Commit();

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);
            instrmx.SetFrequencyReferenceSource("", RFmxInstrMXConstants.OnboardClock);
            var specAn = instrmx.GetSpecAnSignalConfiguration();
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);

            using (new AssertionScope())
            {
                Action specAnCommit = () => specAn.Commit("");
                specAnCommit.Should().NotThrow<RFmxException>("exception will be thrown if LO resource is reserved.");

                instrmx.GetAutomaticSGSASharedLO("LO2", out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                instrmx.GetLOSource("LO2", out string rfmxLOSource);
                instrmx.GetLOExportEnabled("LO2", out bool rfmxLOExportEnabled);

                rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                rfmxLOExportEnabled.Should().BeFalse();

                rfsg.RF.LocalOscillator["LO2"].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                rfsg.RF.LocalOscillator["LO2"].LOOutEnabled.Should().BeFalse();
            }
        }

        [TestMethod()]
        public void Test5831AutomaticLOSharing()
        {
            if (!deviceSessions.TryGetValue("NI PXIe-5831", out Tuple<NIRfsg, RFmxInstrMX> instrSessions))
                Assert.Inconclusive("No instrument present.");

            var rfsg = instrSessions.Item1;
            var sgConfig = SG.InstrumentConfiguration.GetDefault(rfsg);
            SG.ConfigureInstrument(rfsg, sgConfig);
            rfsg.FrequencyReference.Source = RfsgFrequencyReferenceSource.OnboardClock;
            rfsg.Utility.Commit();

            var instrmx = instrSessions.Item2;
            var instrmxConfig = RFmxInstr.InstrumentConfiguration.GetDefault(instrmx);
            RFmxInstr.ConfigureInstrument(instrmx, instrmxConfig);
            instrmx.SetFrequencyReferenceSource("", RFmxInstrMXConstants.OnboardClock);
            var specAn = instrmx.GetSpecAnSignalConfiguration();
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);

            using (new AssertionScope())
            {
                Action specAnCommit = () => specAn.Commit("");
                specAnCommit.Should().NotThrow<RFmxException>("exception will be thrown if LO resource is reserved.");

                foreach (string channelName in new string[] { "LO1", "LO2" })
                {
                    instrmx.GetAutomaticSGSASharedLO(channelName, out RFmxInstrMXAutomaticSGSASharedLO rfmxAutomaticSGSASharedLO);
                    instrmx.GetLOSource(channelName, out string rfmxLOSource);
                    instrmx.GetLOExportEnabled(channelName, out bool rfmxLOExportEnabled);

                    rfmxAutomaticSGSASharedLO.Should().Be(RFmxInstrMXAutomaticSGSASharedLO.Enabled);
                    rfmxLOSource.Should().Be(RFmxInstrMXConstants.LOSourceSGSAShared);
                    rfmxLOExportEnabled.Should().BeFalse();

                    rfsg.RF.LocalOscillator[channelName].Source.Should().Be(RfsgLocalOscillatorSource.SGSAShared);
                    rfsg.RF.LocalOscillator[channelName].LOOutEnabled.Should().BeFalse();
                }
            }
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            foreach (var keyValue in deviceSessions)
            {
                keyValue.Value.Item1.Close();
                keyValue.Value.Item2.Close();
            }
        }
    }
}