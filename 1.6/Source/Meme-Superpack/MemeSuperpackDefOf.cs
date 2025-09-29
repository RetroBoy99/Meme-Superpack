using RimWorld;
using Verse;

namespace MSS.MemeSuperpack
{
	[DefOf]
	public class MemeSuperPackDefOf
	{
		[MayRequireIdeology]
		public static ThoughtDef MSSMeme_RimRim_Watched_Good;

		[MayRequireIdeology]
		public static ThoughtDef MSSMeme_RimRim_Watched_Meh;

		[MayRequireIdeology]
		public static ThoughtDef MSSMeme_RimRim_Watched_Poor;

		[MayRequireIdeology]
		public static ThoughtDef MSSMeme_RimRim_Missed;

		[MayRequireIdeology]
		public static PreceptDef MSSMeme_RimRim_Demanded;

		[MayRequireIdeology]
		public static PreceptDef MSSMeme_IdeoRole_Waifu;

		[MayRequireIdeology]
		public static PreceptDef MSSMeme_RimRim_Ritual;

		[MayRequireIdeology]
		public static HistoryEventDef MSSMeme_WaifuDied;

		public static PawnKindDef MSSMeme_Stickbug;

		public static ThoughtDef MSSMeme_AwakeThought;

		public static SongDef MSSMeme_BuckoDrinkMusic;

		public static HediffDef MSSMeme_YChromosomalAdam;

		public static ThingDef MSSMeme_StickbugIncoming;

		public static readonly XenotypeDef MSSMeme_Taff;
		public static readonly FactionDef MSSMeme_TaffsFaction;

		public static readonly ThoughtDef MSSMeme_Marked;

		[MayRequireIdeology]
		public static TaleDef MSSMeme_WatchedRimRim;

		public static ThingDef MSSMeme_Balloon;
		public static ThingDef MSSMeme_PawnFlyer_Balloon;
		public static JobDef MSSMeme_ExtractTarget;

		static MemeSuperPackDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(MemeSuperPackDefOf));
		}
	}
}
