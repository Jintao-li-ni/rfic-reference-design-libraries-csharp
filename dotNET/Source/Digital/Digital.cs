﻿using System;
using System.Text;
using System.IO;
using NationalInstruments.ModularInstruments.NIDigital;


namespace NationalInstruments.ReferenceDesignLibraries
{
    public class Digital
    {
        #region Type Definitions
        public struct ProjectFiles
        {
            public string PinMapFile;
            public string[] SpecificationsFiles;
            public string[] PinLevelsFiles;
            public string[] TimingFiles;
            public string[] DigitalPatternFiles;
        }
        public struct SourcePinConfiguration
        {
            public string PinName;
            public PpmuOutputFunction PinOutputFunction;
            public double Voltage_V;
            public double Current_A;
        }
        public static SourcePinConfiguration GetDefaultSourcePinConfiguration()
        {
            return new SourcePinConfiguration
            {
                PinOutputFunction = PpmuOutputFunction.DCVoltage,
                Voltage_V = 1.8,
            };
        }
        public struct TriggerConfiguration
        {
            public TriggerType BurstTriggerType;
            public DigitalEdge DigitalEdgeType;
            public string DigitalEdgeSource;
        }
        public static TriggerConfiguration GetDefaultTriggerConfiguration()
        {
            return new TriggerConfiguration
            {
                BurstTriggerType = TriggerType.None,
                DigitalEdgeType = DigitalEdge.Rising,
                DigitalEdgeSource = "PXI_Trig0",
            };
        }
        #endregion
        #region Configuration
        public static void LoadProjectFiles(NIDigital nIDigital, ProjectFiles projectFiles)
        {
            if (string.IsNullOrEmpty(projectFiles.PinMapFile))
            {
                throw new System.ArgumentException("A pin map file is required by the instrument", "ProjectFiles.PinMapFile");
            }
            else nIDigital.LoadPinMap(projectFiles.PinMapFile);

            if (projectFiles.SpecificationsFiles.Length > 0)
            {
                nIDigital.LoadSpecifications(projectFiles.SpecificationsFiles);
            } //Specifications sheets are not required by the instrument

            if (projectFiles.PinLevelsFiles.Length > 0)
            {
                nIDigital.LoadLevels(projectFiles.PinLevelsFiles);
            }
            else throw new System.ArgumentException("At least one levels sheet must be supplied to the instrument", "PinLevelsFiles");

            if (projectFiles.TimingFiles.Length > 0)
            {
                nIDigital.LoadTiming(projectFiles.TimingFiles);
            }
            else throw new System.ArgumentException("At least one timing sheet must be supplied to the instrument", "TimingFiles");
            

            foreach (string path in projectFiles.DigitalPatternFiles) nIDigital.LoadPattern(path);

            if (projectFiles.PinLevelsFiles.Length == 1 && projectFiles.TimingFiles.Length == 1)
            {
                nIDigital.ApplyLevelsAndTiming("", projectFiles.PinLevelsFiles[0], projectFiles.TimingFiles[0]);
            }
        }
        public static void ConfigureAndSourcePin(NIDigital nIDigital, SourcePinConfiguration sourceConfig)
        {
            DigitalPinSet pinSet = nIDigital.PinAndChannelMap.GetPinSet(sourceConfig.PinName);
            pinSet.Ppmu.OutputFunction = sourceConfig.PinOutputFunction;
            switch (sourceConfig.PinOutputFunction)
            {
                case PpmuOutputFunction.DCCurrent:
                    pinSet.Ppmu.DCCurrent.CurrentLevel = sourceConfig.Current_A;
                    break;
                case PpmuOutputFunction.DCVoltage:
                    pinSet.Ppmu.DCVoltage.VoltageLevel = sourceConfig.Voltage_V;
                    break;
            }
            pinSet.Ppmu.Source();
        }
        public static void InitiatePatternGeneration(NIDigital nIDigital, string patternStartLabel, TriggerConfiguration triggerConfig)
        {
            switch (triggerConfig.BurstTriggerType)
            {
                case TriggerType.Software:
                    nIDigital.Trigger.StartTrigger.Software.Configure();

                    // This call to BurstPattern returns immediately because waitUntilDone is 'false.'
                    nIDigital.PatternControl.BurstPattern(string.Empty, patternStartLabel, true, false, TimeSpan.FromSeconds(10));
                    break;
                case TriggerType.DigitalEdge:
                    nIDigital.Trigger.StartTrigger.DigitalEdge.Configure(triggerConfig.DigitalEdgeSource, triggerConfig.DigitalEdgeType);

                    // This call to BurstPattern returns immediately because waitUntilDone is 'false.'
                    nIDigital.PatternControl.BurstPattern("", patternStartLabel, true, false, TimeSpan.FromSeconds(10));
                    break;
                case TriggerType.None:
                    nIDigital.PatternControl.BurstPattern(string.Empty, patternStartLabel, true, TimeSpan.FromSeconds(10));
                    break;
            }
        }
        public static void DisconnectAndClose(NIDigital nIDigital)
        {
            //Disconnect all pins before closing
            nIDigital.PinAndChannelMap.GetPinSet("").SelectedFunction = SelectedFunction.Disconnect;
            nIDigital.Close();
        }
        #endregion
        #region Results
        #endregion
        public class Utilities
        {
            public static ProjectFiles SearchForProjectFiles(string searchDirectory, bool recursiveSearch)
            {
                ProjectFiles results = new ProjectFiles();

                if (!Directory.Exists(searchDirectory))
                    throw new System.IO.DirectoryNotFoundException();

                //Setup search options for the file search
                SearchOption searchOpt;
                if (recursiveSearch) searchOpt = SearchOption.AllDirectories;
                else searchOpt = SearchOption.TopDirectoryOnly;

                string[] pinMapFiles = Directory.GetFiles(searchDirectory, "*.pinmap", searchOpt);
                if (pinMapFiles.Length > 1)
                {
                    throw new System.ArgumentOutOfRangeException("More than one Pin Map files were" +
                        "found in the specified search directory. The instrument can only load one at a time");
                }
                else if (pinMapFiles.Length == 1) results.PinMapFile = pinMapFiles[0];

                results.DigitalPatternFiles = Directory.GetFiles(searchDirectory, "*.digipat", searchOpt);
                results.PinLevelsFiles = Directory.GetFiles(searchDirectory, "*.digilevels", searchOpt);
                results.SpecificationsFiles = Directory.GetFiles(searchDirectory, "*.specs", searchOpt);
                results.TimingFiles = Directory.GetFiles(searchDirectory, "*.digitiming", searchOpt);

                return results;
            }
        }
    }
}
