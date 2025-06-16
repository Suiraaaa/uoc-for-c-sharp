using System;
using System.Linq;
using System.Collections.Generic;
using Uoc.Analyze;
using Uoc.Chart.Notes;

namespace Uoc.Chart
{
    /// <summary>
    /// ノーツデータ
    /// </summary>
    public class NotesData
    {
        private readonly NoteDefCollection noteDefCollection;
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NotePlaybackProviderCollection notePlaybackProviderCollection;
        private readonly NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection;

        private NotesData(NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NotePlaybackProviderCollection notePlaybackProviderCollection, NoteGroupPlaybackProviderCollection noteGroupPlaybackProviderCollection)
        {
            this.noteDefCollection = noteDefCollection ?? throw new ArgumentNullException(nameof(noteDefCollection));
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.notePlaybackProviderCollection = notePlaybackProviderCollection ?? throw new ArgumentNullException(nameof(notePlaybackProviderCollection));
            this.noteGroupPlaybackProviderCollection = noteGroupPlaybackProviderCollection ?? throw new ArgumentNullException(nameof(noteGroupPlaybackProviderCollection));
        }

        internal static NotesData Create(NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfiles, NoteGroupProfileCollection noteGroupProfiles, AnalysisSetting analysisSetting, Tpb tpb)
        {
            var notePlaybackProviders = NotePlaybackProviderCollection.FormNoteProfileCollection(noteProfiles, analysisSetting, tpb);
            var noteGroupPlaybackProviders = NoteGroupPlaybackProviderCollection.FormNoteProfileCollection(notePlaybackProviders, noteGroupProfiles);
            return new NotesData(noteDefCollection, noteGroupDefCollection, notePlaybackProviders, noteGroupPlaybackProviders);
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
        /// ノート再生プロバイダのリスト
        /// 生成タイミングで昇順
        /// </summary>
        public IReadOnlyList<NotePlaybackProvider> NotePlaybackProviders
        {
            get
            {
                return notePlaybackProviderCollection.NotePlaybackProviders
                    .Where(x => !noteGroupDefCollection.BelongsToAnyGroup(x.NoteId))
                    .ToList();
            }
        }

        /// <summary>
        /// ノートグループ再生プロバイダのリスト
        /// 生成タイミングで昇順
        /// </summary>
        public IReadOnlyList<NoteGroupPlaybackProvider> NoteGroupPlaybackProviders => noteGroupPlaybackProviderCollection.NoteGroupPlaybackProvider;
    }
}
