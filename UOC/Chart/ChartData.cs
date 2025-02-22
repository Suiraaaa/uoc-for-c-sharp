using System;
using System.Collections.Generic;
using UOC.Analyze;
using UOC.Chart.Notes;

namespace UOC.Chart
{
    /// <summary>
    /// 譜面データ
    /// </summary>
    public class ChartData
    {
        private readonly NotePlaybackProviderCollection notePlaybackProviderCollection;
        private readonly NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection;

        private ChartData(NotePlaybackProviderCollection notePlaybackProviderCollection, NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection)
        {
            this.notePlaybackProviderCollection = notePlaybackProviderCollection ?? throw new ArgumentNullException(nameof(notePlaybackProviderCollection));
            this.noteGroupPlaybackProviderCollection = noteGroupPlaybackProviderCollection ?? throw new ArgumentNullException(nameof(noteGroupPlaybackProviderCollection));
        }

        public static ChartData Create(NoteProfileCollection noteProfiles, NoteGroupProfileCollection noteGroupProfiles, AnalysisSetting analysisSetting, TPB tpb)
        {
            var notePlaybackProviders = NotePlaybackProviderCollection.FormNoteProfileCollection(noteProfiles, analysisSetting, tpb);
            var noteGroupPlaybackProviders = NoteGroupPlaybackProviderCollection.FormNoteProfileCollection(notePlaybackProviders, noteGroupProfiles);
            return new ChartData(notePlaybackProviders, noteGroupPlaybackProviders);
        }

        /// <summary>
        /// ノート再生プロバイダのリスト
        /// 生成タイミングで昇順
        /// </summary>
        public IReadOnlyList<NotePlaybackProvider> NotePlaybackProviders => notePlaybackProviderCollection.NotePlaybackProviders;

        /// <summary>
        /// ノートグループ再生プロバイダのリスト
        /// 生成タイミングで昇順
        /// </summary>
        public IReadOnlyList<NoteGroupPlaybackProvider> NoteGroupPlaybackProviders => noteGroupPlaybackProviderCollection.NoteGroupPlaybackProvider;
    }
}
