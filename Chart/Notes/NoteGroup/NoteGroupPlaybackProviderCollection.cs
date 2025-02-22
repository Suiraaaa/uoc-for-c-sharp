using System;
using System.Collections.Generic;
using System.Linq;

namespace UOC.Chart.Notes
{
    internal class NoteGroupPlaybackProviderCollection
    {
        private readonly IReadOnlyList<NoteGroupPlaybackProvider> noteGroupPlaybackProviders;

        public NoteGroupPlaybackProviderCollection(IReadOnlyList<NoteGroupPlaybackProvider> noteGroupPlaybackProviders)
        {
            if (noteGroupPlaybackProviders == null) throw new ArgumentNullException(nameof(noteGroupPlaybackProviders));
            this.noteGroupPlaybackProviders = noteGroupPlaybackProviders.OrderBy(x => x.FirstInstantiateTiming).ToList(); // 生成タイミングで昇順にソート
        }

        public static NoteGroupPlaybackProviderCollection FormNoteProfileCollection(NotePlaybackProviderCollection notePlaybackProviderCollection, NoteGroupProfileCollection noteGroupProfileCollection)
        {
            var noteGroupPlaybackProviders = new List<NoteGroupPlaybackProvider>();
            foreach (var noteGroupProfile in noteGroupProfileCollection.NoteGroupProfiles)
            {
                var belongsNotePlaybackProviders = new List<NotePlaybackProvider>();
                foreach (var note in noteGroupProfile.BelongsNotes)
                {
                    var belongsNotePlaybackProvider = notePlaybackProviderCollection.NotePlaybackProviders.FirstOrDefault(x => note.Guid.Value == x.Guid);
                    if (belongsNotePlaybackProvider == null) throw new KeyNotFoundException($"Guidが\"{note.Guid.Value}\"のノートは見つかりませんでした。");
                    belongsNotePlaybackProviders.Add(belongsNotePlaybackProvider);
                }
                var noteGroupPlaybackProvider = new NoteGroupPlaybackProvider(noteGroupProfile.NoteGroupDef.NoteGroupId, belongsNotePlaybackProviders);
                noteGroupPlaybackProviders.Add(noteGroupPlaybackProvider);
            }
            return new NoteGroupPlaybackProviderCollection(noteGroupPlaybackProviders);
        }

        public IReadOnlyList<NoteGroupPlaybackProvider> NoteGroupPlaybackProvider => noteGroupPlaybackProviders;
    }
}
