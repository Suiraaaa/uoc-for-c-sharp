using System;
using System.Collections.Generic;
using System.Linq;
using Uoc.Chart;
using Uoc.Chart.Notes;
using Uoc.Chart.Notes.Definition;

namespace Uoc.Analyze.Playback
{
    /// <summary>
    /// 譜面再生データ
    /// </summary>
    public class ChartPlaybackData
    {
        private readonly NoteDefCollection noteDefCollection;
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NotePlaybackProviderCollection notePlaybackProviderCollection;
        private readonly NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection;

        private ChartPlaybackData(NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NotePlaybackProviderCollection notePlaybackProviderCollection, NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection)
        {
            this.noteDefCollection = noteDefCollection ?? throw new ArgumentNullException(nameof(noteDefCollection));
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.notePlaybackProviderCollection = notePlaybackProviderCollection ?? throw new ArgumentNullException(nameof(notePlaybackProviderCollection));
            this.noteGroupPlaybackProviderCollection = noteGroupPlaybackProviderCollection ?? throw new ArgumentNullException(nameof(noteGroupPlaybackProviderCollection));
        }

        internal static ChartPlaybackData Create(NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfiles, NoteGroupProfileCollection noteGroupProfiles, AnalysisSetting analysisSetting, Tpb tpb)
        {
            var notePlaybackProviders = NotePlaybackProviderCollection.FormNoteProfileCollection(noteProfiles, analysisSetting, tpb);
            var noteGroupPlaybackProviders = NoteGroupPlaybackProviderCollection.FormNoteProfileCollection(notePlaybackProviders, noteGroupProfiles);
            return new ChartPlaybackData(noteDefCollection, noteGroupDefCollection, notePlaybackProviders, noteGroupPlaybackProviders);
        }

        /// <summary>
        /// ノート定義コレクション
        /// </summary>
        public NoteDefCollection NoteDefCollection => noteDefCollection;

        /// <summary>
        /// ノートグループ定義コレクション
        /// </summary>
        public NoteGroupDefCollection NoteGroupDefCollection => noteGroupDefCollection;

        /// <summary>
        /// 単体ノートの再生プロバイダのリストを取得します。
        /// グループに所属するノートは含まれません。
        /// 生成タイミングで昇順
        /// </summary>
        /// <returns>単体ノートの再生プロバイダのリスト</returns>
        public IReadOnlyList<NotePlaybackProvider> GetSingleNotePlaybackProviders()
        {
            return notePlaybackProviderCollection.NotePlaybackProviders
                .Where(x => !noteGroupDefCollection.BelongsToAnyGroup(x.NoteId))
                .ToList();
        }

        /// <summary>
        /// すべてのノートグループの再生プロバイダのリストを取得します。
        /// 生成タイミングで昇順
        /// </summary>
        /// <returns>すべてのノートグループの再生プロバイダのリスト</returns>
        public IReadOnlyList<NoteGroupPlaybackProvider> GetNoteGroupPlaybackProviders()
        {
            return noteGroupPlaybackProviderCollection.NoteGroupPlaybackProvider;
        }
    }
}
