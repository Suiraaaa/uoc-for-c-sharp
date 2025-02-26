using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Uoc.Chart;
using Uoc.Chart.Notes;

namespace Uoc.Analyze
{
    /// <summary>
    /// CHARTセクションに所属する行
    /// </summary>
    internal class ChartSectionLine
    {
        private static readonly Regex chartLineRegex = new(@"^\s*([A-Z0-9]{3})([A-Z0-9]{2})([A-Z0-9]{2})?\s*:\s*(\d+)(?:\s*,\s*(.*))*?$", RegexOptions.Compiled);

        private readonly IReadOnlyList<Position> positions;
        private readonly IReadOnlyList<string> propertyValues;
        private readonly Layer layer;
        private readonly Channel channel;
        private readonly NoteDefIndex noteDefIndex;

        private ChartSectionLine(IReadOnlyList<Position> positions, IReadOnlyList<string> propertyValues, Layer layer, Channel channel, NoteDefIndex noteDefIndex)
        {
            this.positions = positions ?? throw new ArgumentNullException(nameof(positions));
            this.layer = layer ?? throw new ArgumentNullException(nameof(layer));
            this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this.propertyValues = propertyValues ?? throw new ArgumentException(nameof(propertyValues));
            this.noteDefIndex = noteDefIndex ?? throw new ArgumentNullException(nameof(noteDefIndex));

            if (positions.Count == 0) throw new ArgumentException(nameof(positions));
        }

        public static ChartSectionLine ParseUocLine(UocLine line, Layer layer)
        {
            try
            {
                /*
                * 入力形式例
                * "0004 : 0100, 3, 4"
                * "002501 : 0100, 3, 4"
                */
                var match = chartLineRegex.Match(line.LineText);
                if (!match.Success)
                {
                    throw new FormatException("正規表現によるパースに失敗しました。");
                }
                var measureIndex = new MeasureIndex(Base36.Decode(match.Groups[1].Value));
                var noteDefIndex = new NoteDefIndex(Base36.Decode(match.Groups[2].Value));
                var channel = match.Groups[3].Success ? new Channel(Base36.Decode(match.Groups[3].Value)) : Channel.Empty;
                var positionsString = match.Groups[4].Value;
                var propertyValues = string.IsNullOrWhiteSpace(match.Groups[5].Value) ? new string[0] : match.Groups[5].Value.Replace(" ", "").Split(',');

                // すべての位置を求める（positionsString内の各文字について '1' の位置をPositionとして記録）
                var positions = new List<Position>();
                for (int i = 0; i < positionsString.Length; i++)
                {
                    if (positionsString[i] != '1') continue;
                    var position = new Position(measureIndex, positionsString.Length, i);
                    positions.Add(position);
                }

                var chartSectionLine = new ChartSectionLine(positions, propertyValues, layer, channel, noteDefIndex);
                return chartSectionLine;
            }
            catch (Exception e)
            {
                throw new Exception($"行{line}をCHARTセクション行にパースできませんでした。", e);
            }
        }

        public IReadOnlyList<NoteProfile> ToNoteProfiles(NoteDefCollection noteDefCollection)
        {
            if (noteDefCollection == null) throw new ArgumentNullException(nameof(noteDefCollection));
            var noteProfiles = new List<NoteProfile>();
            var noteDef = noteDefCollection.GetNoteDefByIndex(noteDefIndex);
            foreach (var position in positions)
            {
                var noteProfile = new NoteProfile(noteDef, position, propertyValues, layer, channel);
                noteProfiles.Add(noteProfile);
            }
            return noteProfiles;
        }
    }
}
