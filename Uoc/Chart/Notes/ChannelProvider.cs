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

        public ChannelProvider(NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection)
        {
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.noteProfileCollection = noteProfileCollection ?? throw new ArgumentNullException(nameof(noteProfileCollection));
        }

        public Channel GetAvailableChannel(Position startPosition, Position endPosition, Layer layer)
        {
            var channelReservations = GetChannelReservations(layer);
            var reservatedReservations = channelReservations.Where(x => x.IsReserved(startPosition, endPosition)).ToList();
            var channel = reservatedReservations.Count != 0 ? reservatedReservations.Max(x => x.Channel.Value) + 1 : 0;
            return new Channel(channel);
        }

        private IReadOnlyList<ChannelReservation> GetChannelReservations(Layer layer)
        {
            var channelReservations = new List<ChannelReservation>();
            var notes = noteProfileCollection.NoteProfiles;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Layer != layer) continue;

                // ノートが始点として所属するノートグループを探索
                var startNote = notes[i];
                var noteGroupDef = noteGroupDefCollection.GetNoteGroupDefByStartNoteId(startNote.NoteDef.NoteId);
                if (noteGroupDef == null) continue;

                // 終点ノートを探索
                var endNote = (NoteProfile?)null;
                for (int j = i + 1; j < notes.Count; j++)
                {
                    if (notes[j].Layer != layer) continue;
                    if (notes[j].Channel == Channel.Empty || notes[j].Channel != startNote.Channel) continue;
                    if (notes[j].NoteDef.NoteId != noteGroupDef.EndNoteId) continue;
                    endNote = notes[j];
                }
                if (endNote == null) throw new Exception("グループの終点ノートが見つかりませんでした。");

                // 予約を追加
                channelReservations.Add(new ChannelReservation(startNote.Position, endNote.Position, startNote.Channel));
            }
            return channelReservations;
        }

        private readonly struct ChannelReservation
        {
            private readonly Position start;
            private readonly Position end;
            private readonly Channel channel;

            public ChannelReservation(Position start, Position end, Channel channel)
            {
                this.start = start;
                this.end = end;
                this.channel = channel;
            }

            public Channel Channel => channel;

            public bool IsReserved(Position startPosition, Position endPosition)
            {
                return start <= startPosition && startPosition <= end || start <= endPosition && endPosition <= end;
            }
        }
    }
}
