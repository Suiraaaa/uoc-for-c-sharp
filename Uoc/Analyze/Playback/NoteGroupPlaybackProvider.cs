using System;
using System.Collections.Generic;
using Uoc.Chart;
using Uoc.Chart.Notes;

namespace Uoc.Analyze.Playback
{
    /// <summary>
    /// ノートグループの再生に関する情報を提供するクラス
    /// </summary>
    public class NoteGroupPlaybackProvider
    {
        private readonly NoteGroupProfile noteGroupProfile;
        private readonly IReadOnlyList<NotePlaybackProvider> notePlaybackProviders;
        private readonly NoteGroupJudgmentCalculator judgmentCalculator;

        internal NoteGroupPlaybackProvider(NoteGroupProfile noteGroupProfile, IReadOnlyList<NotePlaybackProvider> notePlaybackProviders, NoteGroupJudgmentCalculator judgmentCalculator)
        {
            if (noteGroupProfile == null) throw new ArgumentNullException(nameof(noteGroupProfile));
            if (notePlaybackProviders == null) throw new ArgumentNullException(nameof(notePlaybackProviders));
            if (notePlaybackProviders.Count == 0) throw new ArgumentNullException("グループに所属するノートが含まれていません。");

            this.noteGroupProfile = noteGroupProfile;
            this.notePlaybackProviders = notePlaybackProviders;
            this.judgmentCalculator = judgmentCalculator ?? throw new ArgumentNullException(nameof(judgmentCalculator));
        }

        /// <summary>
        /// ノートグループID
        /// </summary>
        public NoteGroupId NoteGroupId => noteGroupProfile.NoteGroupId;

        /// <summary>
        /// グループに所属するノートのリスト
        /// </summary>
        public IReadOnlyList<NotePlaybackProvider> BelongsNotes => notePlaybackProviders;

        /// <summary>
        /// グループ始点の生成タイミング
        /// </summary>
        public long FirstInstantiateTiming => notePlaybackProviders[0].InstantiateTiming;

        public IReadOnlyList<long> GetJudgmentTimings(int noteDivision, Func<NotePlaybackProvider, bool>? excludeAtNote = null)
        {
            if (noteDivision <= 0) throw new ArgumentOutOfRangeException(nameof(noteDivision));
            return GetJudgmentTimings(_ => noteDivision, excludeAtNote);
        }

        public IReadOnlyList<long> GetJudgmentTimings(Func<Bpm, int> noteDivisionSelector, Func<NotePlaybackProvider, bool>? excludeAtNote = null)
        {
            if (noteDivisionSelector == null) throw new ArgumentNullException(nameof(noteDivisionSelector));
            var excludedNoteGuids = new HashSet<Guid>();
            if (excludeAtNote != null)
            {
                foreach (var note in notePlaybackProviders)
                {
                    if (excludeAtNote(note)) excludedNoteGuids.Add(note.Guid);
                }
            }
            return judgmentCalculator.Calculate(noteGroupProfile, noteDivisionSelector, excludedNoteGuids);
        }
    }
}
