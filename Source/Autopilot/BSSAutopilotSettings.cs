using System.Reflection;
using KSP.Localization;

namespace BetterSandboxSpecializations.Autopilot
{
    public class BSSAutopilotSettings : GameParameters.CustomParameterNode
    {
        private readonly string _title = Localizer.Format("#BSS_LOC_APSAS_title");      // "Autopilot / SAS"
        private readonly string _displaySection = Localizer.Format("#BSS_LOC_section"); // "Better Sandbox Specializations"
        private const string _section = "BetterSandboxSpecializations";
        private const int _order = 1;
        private const bool _hasPresets = true;
        private const GameParameters.GameMode gmNM = GameParameters.GameMode.CAREER | GameParameters.GameMode.SCIENCE | GameParameters.GameMode.SANDBOX;
        private const GameParameters.GameMode gmSS = GameParameters.GameMode.SCIENCE | GameParameters.GameMode.SANDBOX;
        private const GameParameters.GameMode gmCrr = GameParameters.GameMode.CAREER;
        private const GameParameters.GameMode gmSci = GameParameters.GameMode.SCIENCE;
        private const GameParameters.GameMode gmSnd = GameParameters.GameMode.SANDBOX;

        #region Properties
        public override string Title
        {
            get { return _title; }
        }
        public override string DisplaySection
        {
            get { return _displaySection; }
        }
        public override string Section
        {
            get { return _section; }
        }
        public override int SectionOrder
        {
            get { return _order; }
        }
        public override bool HasPresets
        {
            get { return _hasPresets; }
        }
        public override GameParameters.GameMode GameMode
        {
            get { return gmNM; }
        }
        #endregion

        #region UI Elements
        // #BSS_LOC_APSAS_gm = "The game mode is"
        [GameParameters.CustomStringParameterUI("#BSS_LOC_APSAS_gm", autoPersistance = false, gameMode = gmCrr)]
        public string gmDispCrr = Localizer.Format("#autoLOC_190722"); // #autoLOC_190722 = Career
        [GameParameters.CustomStringParameterUI("#BSS_LOC_APSAS_gm", autoPersistance = false, gameMode = gmSci)]
        public string gmDispSci = Localizer.Format("#autoLOC_190714"); // #autoLOC_190714 = Science
        [GameParameters.CustomStringParameterUI("#BSS_LOC_APSAS_gm", autoPersistance = false, gameMode = gmSnd)]
        public string gmDispSnd = Localizer.Format("#autoLOC_190706"); // #autoLOC_190706 = Sandbox

        // #autoLOC_140970 = Enable Kerbal Experience
        [GameParameters.CustomStringParameterUI("#autoLOC_140970", autoPersistance = false)]
        public string useXPon = Localizer.Format("#BSS_LOC_on");
        [GameParameters.CustomStringParameterUI("#autoLOC_140970", autoPersistance = false)]
        public string useXPoff = Localizer.Format("#BSS_LOC_off");

        // #autoLOC_6010001 = All SAS Modes on all probes
        [GameParameters.CustomStringParameterUI("#autoLOC_6010001", autoPersistance = false)] // not gmSS, in case of doctored career saves
        public string freeSASon = Localizer.Format("#BSS_LOC_on");
        [GameParameters.CustomStringParameterUI("#autoLOC_6010001", autoPersistance = false, gameMode = gmSS)]
        public string freeSASoff = Localizer.Format("#BSS_LOC_off");

        [GameParameters.CustomParameterUI(
            "#BSS_LOC_APSAS_freeSAS_title",                 // "SAS always available"
            toolTip = "#BSS_LOC_APSAS_freeSAS_tooltip",     // "If on, SAS will always be available."
            gameMode = gmSS
        )]
        public bool enableFullSASInSandbox = false;

        [GameParameters.CustomParameterUI(
            "#BSS_LOC_APSAS_reqpilot_title",                // "Require pilot for SAS"
            toolTip = "#BSS_LOC_APSAS_reqpilot_tooltip",    // "If off, any crewmember can provide SAS.\nIf on, a pilot will be required."
            gameMode = gmNM
        )]
        public bool requirePilotForSAS = true;

        [GameParameters.CustomStringParameterUI("", autoPersistance = false)]
        public string behaviorStock = Localizer.Format("#BSS_LOC_APSAS_stock_behavior");    // "Stock behavior is in use:"
        [GameParameters.CustomStringParameterUI("", autoPersistance = false)]
        public string behaviorCustom = Localizer.Format("#BSS_LOC_APSAS_custom_behavior");  // "Custom behavior is in use:"

        [GameParameters.CustomStringParameterUI("", autoPersistance = false, lines=4)]
        public string useXPdesc = Localizer.Format("#BSS_LOC_APSAS_useXP_on_desc");
        [GameParameters.CustomStringParameterUI("", autoPersistance = false, lines=2)]
        public string freeSASdesc = Localizer.Format("#BSS_LOC_APSAS_freeSAS_on_desc");
        [GameParameters.CustomStringParameterUI("", autoPersistance = false, lines=4)]
        public string reqPilotOn = Localizer.Format("#BSS_LOC_APSAS_reqPilot_on_desc");
        [GameParameters.CustomStringParameterUI("", autoPersistance = false, lines=4)]
        public string reqPilotOff = Localizer.Format("#BSS_LOC_APSAS_reqPilot_off_desc");
        #endregion

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "requirePilotForSAS") return true;
            if (member.Name.StartsWith("gmDisp")) return true;
            bool useXP = parameters.EnableKerbalExperience();
            if (member.Name == "useXPon" || member.Name == "useXPdesc") return useXP;
            if (member.Name == "useXPoff") return !useXP;

            bool freeSAS = parameters.EnableFullSASInSandbox();
            if (BSSAutopilot.KSP_1_6_plus)
            {
                if (member.Name == "freeSASon") return !useXP && freeSAS;
                if (member.Name == "freeSASoff") return !useXP && !freeSAS;
                if (member.Name == "enableFullSASInSandbox") return false;
            }
            else
            {
                if (member.Name == "freeSASon" || member.Name == "freeSASoff") return false;
                if (member.Name == "enableFullSASInSandbox") return true;
            }

            if (member.Name == "behaviorStock") return useXP || freeSAS;
            if (member.Name == "behaviorCustom") return !useXP && !freeSAS;
            if (member.Name == "freeSASdesc") return !useXP && freeSAS;

            bool reqPilot = parameters.RequirePilotForSAS();
            if (member.Name == "reqPilotOn") return !useXP && !freeSAS && reqPilot;
            if (member.Name == "reqPilotOff") return !useXP && !freeSAS && !reqPilot;
            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "enableFullSASInSandbox")
                return !parameters.EnableKerbalExperience();
            if (member.Name == "requirePilotForSAS")
                return
                    !parameters.EnableKerbalExperience() &&
                    !parameters.EnableFullSASInSandbox();
            return true;
        }

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    requirePilotForSAS = false;
                    break;
                case GameParameters.Preset.Normal:
                case GameParameters.Preset.Moderate:
                case GameParameters.Preset.Hard:
                    requirePilotForSAS = true;
                    break;
            }
        }
    }
}
