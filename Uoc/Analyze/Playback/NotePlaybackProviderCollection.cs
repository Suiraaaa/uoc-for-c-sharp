using System;
using System.Collections.Generic;
using System.Linq;
using Uoc.Chart.Event;
using Uoc.Chart.Notes;

namespace Uoc.Analyze.Playback
{
    internal class NotePlaybackProviderCollection
    {
        private readonly IReadOnlyList<NotePlaybackProvider> notePlaybackProviders;

        private NotePlaybackProviderCollection(IReadOnlyList<NotePlaybackProvider> notePlaybackProviders)
        {
            if (notePlaybackProviders == null) throw new ArgumentNullException(nameof(notePlaybackProviders));
            this.notePlaybackProviders = notePlaybackProviders.OrderBy(x => x.InstantiateTiming).ToList(); // 生成タイミングで昇順にソート
        }

        public static NotePlaybackProviderCollection FormNoteProfileCollection(NoteProfileCollection noteProfileCollection, EventProviders eventsProvider, PlaybackTimingCalculator timingCalculator, AnalysisSetting analysisSetting)
        {
            var maxMeasureIndex = noteProfileCollection.GetMaxMeasureIndex();
            var layers = noteProfileCollection.NoteProfiles.Select(x => x.Layer).Distinct().ToList();
            var playbackCalculator = new NotePlaybackCalculator(eventsProvider.SpeedMultiplierProvider, timingCalculator, analysisSetting, maxMeasureIndex, layers);
            try
            {
                var notePlaybackProviders = noteProfileCollection.NoteProfiles.Select(x => new NotePlaybackProvider(x, playbackCalculator)).ToList();
                return new NotePlaybackProviderCollection(notePlaybackProviders);
            }
            finally
            {
                playbackCalculator.ReleaseInstantiationCache();
            }
        }

        public IReadOnlyList<NotePlaybackProvider> NotePlaybackProviders => notePlaybackProviders;

        public NotePlaybackProvider GetNotePlaybackProviderByGuid(Guid guid)
        {
            return notePlaybackProviders.FirstOrDefault(x => x.Guid == guid) ?? throw new KeyNotFoundException($"Guidが\"{guid}\"のノートは見つかりませんでした。");
        }
    }
}
