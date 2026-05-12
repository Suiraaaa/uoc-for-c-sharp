using System;
using System.Collections.Generic;
using System.Linq;
using Uoc.Chart.Notes.Definition;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートグループのチャンネルを提供するクラス
    /// </summary>
    public class ChannelProvider
    {
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NoteProfileCollection noteProfileCollection;
        private readonly List<ChannelReservation> addedReservations;

        public ChannelProvider(NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection)
        {
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.noteProfileCollection = noteProfileCollection ?? throw new ArgumentNullException(nameof(noteProfileCollection));
            addedReservations = new();
        }

        public Channel GetAvailableChannel(Position startPosition, Position endPosition, Layer layer)
        {
            var reservedReservations = GetChannelReservations(layer)
                .Concat(addedReservations.Where(x => x.Layer == layer))
                .Where(x => x.IsReserved(startPosition, endPosition));

            return GetMinAvailableChannel(reservedReservations);
        }

        public Channel GetAvailableChannelAndAddReservation(Position startPosition, Position endPosition, Layer layer)
        {
            var reservedReservations = GetChannelReservations(layer)
                .Concat(addedReservations.Where(x => x.Layer == layer))
                .Where(x => x.IsReserved(startPosition, endPosition));

            var channel = GetMinAvailableChannel(reservedReservations);

            addedReservations.Add(new ChannelReservation(startPosition, endPosition, layer, channel));

            return channel;
        }

        public void ClearAddedReservations()
        {
            addedReservations.Clear();
        }

        private IReadOnlyList<ChannelReservation> GetChannelReservations(Layer layer)
        {
            var reservations = new List<ChannelReservation>();
            var notes = noteProfileCollection.NoteProfiles;
            for (var i = 0; i < notes.Count; i++)
            {
                var startNote = notes[i];

                if (startNote.Layer != layer) continue;
                if (startNote.Channel.IsEmpty) continue;
                if (!noteGroupDefCollection.IsStartNoteInAnyGroup(startNote.NoteDef.NoteId.Value)) continue;

                var noteGroupDef = noteGroupDefCollection.GetNoteGroupDefByStartNoteId(startNote.NoteDef.NoteId);
                if (noteGroupDef == null) continue;

                var endNote = FindEndNote(notes, i + 1, layer, startNote.Channel, noteGroupDef.EndNoteId);
                if (endNote == null)
                {
                    throw new InvalidOperationException("グループの終点ノートが見つかりませんでした。");
                }

                reservations.Add(new ChannelReservation(startNote.Position, endNote.Position, layer, startNote.Channel));
            }
            return reservations;
        }

        private static Channel GetMinAvailableChannel(IEnumerable<ChannelReservation> reservations)
        {
            var usedChannels = reservations.Select(x => x.Channel.Value).ToHashSet();
            var channel = 0;
            while (usedChannels.Contains(channel))
            {
                channel++;
            }
            return new Channel(channel);
        }

        private static NoteProfile? FindEndNote(IReadOnlyList<NoteProfile> notes, int startIndex, Layer layer, Channel channel, NoteId endNoteId)
        {
            for (var index = startIndex; index < notes.Count; index++)
            {
                var candidate = notes[index];

                if (candidate.Layer != layer) continue;
                if (candidate.Channel.IsEmpty) continue;
                if (candidate.Channel != channel) continue;
                if (candidate.NoteDef.NoteId != endNoteId) continue;

                return candidate;
            }
            return null;
        }

        private readonly struct ChannelReservation
        {
            private readonly Position start;
            private readonly Position end;
            private readonly Layer layer;
            private readonly Channel channel;

            public ChannelReservation(Position start, Position end, Layer layer, Channel channel)
            {
                this.start = start;
                this.end = end;
                this.layer = layer;
                this.channel = channel;
            }

            public Layer Layer => layer;

            public Channel Channel => channel;

            public bool IsReserved(Position startPosition, Position endPosition)
            {
                return start <= endPosition && startPosition <= end;
            }
        }
    }
}
