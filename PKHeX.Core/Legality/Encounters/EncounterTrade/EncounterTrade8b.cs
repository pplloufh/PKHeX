﻿namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Trade Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterTrade"/>
    public sealed record EncounterTrade8b : EncounterTrade, IContestStats, IScaledSize, IFixedOTFriendship
    {
        public override int Generation => 8;
        public override int Location => Locations.LinkTrade6NPC;

        public EncounterTrade8b(GameVersion game) : base(game) => EggLocation = unchecked((ushort)Locations.Default8bNone);
        public byte CNT_Cool => BaseContest;
        public byte CNT_Beauty => BaseContest;
        public byte CNT_Cute => BaseContest;
        public byte CNT_Smart => BaseContest;
        public byte CNT_Tough => BaseContest;
        public byte CNT_Sheen => 0;
        public byte HeightScalar { get; set; }
        public byte WeightScalar { get; set; }
        public byte OT_Friendship => Species == (int)Core.Species.Chatot ? (byte)35 : (byte)50;
        private byte BaseContest => Species == (int)Core.Species.Chatot ? (byte)20 : (byte)0;
        public uint PID { get; init; }
        public uint EncryptionConstant { get; init; }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm.EncryptionConstant != EncryptionConstant)
                return false;
            if (pkm.PID != PID)
                return false;
            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;
            if (pkm is IScaledSize h && h.HeightScalar != HeightScalar)
                return false;
            if (pkm is IScaledSize w && w.WeightScalar != WeightScalar)
                return false;
            return base.IsMatchExact(pkm, evo);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pb8 = (PB8)pk;
            pb8.EncryptionConstant = EncryptionConstant;
            pb8.PID = PID;

            // Has German Language ID for all except German origin, which is Japanese
            if (Species == (int)Core.Species.Magikarp)
                pb8.Language = (int)(pb8.Language == (int)LanguageID.German ? LanguageID.Japanese : LanguageID.German);

            this.CopyContestStatsTo(pb8);
            pb8.HT_Language = sav.Language;
            pb8.HeightScalar = HeightScalar;
            pb8.WeightScalar = WeightScalar;
        }
    }
}
