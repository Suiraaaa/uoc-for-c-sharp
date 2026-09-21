using System;
using System.Collections.Generic;
using Uoc.Chart;
using Uoc.Chart.Event;
using Uoc.Chart.Notes;

namespace Uoc.Analyze.Playback
{
    // 小節基準の音符グリッドからグループ内の周期判定位置を選ぶ。
    internal class NoteGroupJudgmentCalculator
    {
        private readonly BpmProvider bpmProvider;
        private readonly MeasureLengthProvider measureLengthProvider;
        private readonly PlaybackTimingCalculator timingCalculator;

        public NoteGroupJudgmentCalculator(BpmProvider bpmProvider, MeasureLengthProvider measureLengthProvider, PlaybackTimingCalculator timingCalculator)
        {
            this.bpmProvider = bpmProvider ?? throw new ArgumentNullException(nameof(bpmProvider));
            this.measureLengthProvider = measureLengthProvider ?? throw new ArgumentNullException(nameof(measureLengthProvider));
            this.timingCalculator = timingCalculator ?? throw new ArgumentNullException(nameof(timingCalculator));
        }

        public IReadOnlyList<long> Calculate(NoteGroupProfile noteGroupProfile, Func<Bpm, int> noteDivisionSelector, ISet<Guid> excludedNoteGuids)
        {
            var notes = noteGroupProfile.BelongsNotes;
            var start = notes[0].Position;
            var end = notes[^1].Position;
            var excludedPositions = new HashSet<Position>();
            foreach (var note in notes)
            {
                if (excludedNoteGuids.Contains(note.Guid)) excludedPositions.Add(note.Position);
            }

            var timings = new List<long>();
            for (var measure = start.MeasureIndex.Value; ; measure++)
            {
                var measureIndex = new MeasureIndex(measure);
                var measureLength = measureLengthProvider.GetMeasureLengthAt(measure);
                var measureTicks = timingCalculator.CalculateMeasureMaxTick(measure);
                if (measureTicks <= 0) throw new InvalidOperationException("小節のtick数を正の整数で表現できません。");
                var segmentStart = measure == start.MeasureIndex.Value ? start : Position.MeasureStart(measureIndex);
                var bpm = bpmProvider.GetMeasureStartBpm(measure);
                var changes = bpmProvider.GetBpmChangeEventsAt(measure);

                for (var i = 0; i < changes.Count; i++)
                {
                    var change = changes[i];
                    // 同一tickの変更は、既存のイベント順で最後に有効になるBPMへまとめる。
                    while (i + 1 < changes.Count && changes[i + 1].Tick == change.Tick)
                    {
                        change = changes[++i];
                    }

                    // 小節末尾へ丸められたイベントは次小節の始点BPMとして適用される。
                    if (change.Tick.Value >= measureTicks) break;
                    var boundary = new Position(measureIndex, measureTicks, change.Tick.Value);
                    if (boundary > end) break;
                    if (boundary <= segmentStart)
                    {
                        bpm = change.Bpm;
                        continue;
                    }

                    AppendSegment(timings, segmentStart, boundary, end, measureLength, bpm, noteDivisionSelector, excludedPositions);
                    segmentStart = boundary;
                    bpm = change.Bpm;
                }

                AppendSegment(timings, segmentStart, null, end, measureLength, bpm, noteDivisionSelector, excludedPositions);
                if (measure == end.MeasureIndex.Value) break;
            }
            return timings.AsReadOnly();
        }

        private void AppendSegment(List<long> timings, Position start, Position? endExclusive, Position groupEnd, MeasureLength measureLength, Bpm bpm, Func<Bpm, int> noteDivisionSelector, HashSet<Position> excludedPositions)
        {
            var division = noteDivisionSelector(bpm);
            if (division <= 0) throw new ArgumentOutOfRangeException(nameof(noteDivisionSelector), division, "音符間隔は正の整数で指定してください。");

            // n分音符の小節内間隔 = 拍子の分母 / (拍子の分子 × n)。
            // 先に約分し、Positionで正確に表現できる範囲を確認する。
            var stepNumerator = (long)measureLength.Denominator;
            var stepDenominator = (long)measureLength.Numerator * division;
            var gcd = GCD(stepNumerator, stepDenominator);
            stepNumerator /= gcd;
            stepDenominator /= gcd;
            if (stepDenominator > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(noteDivisionSelector), division, "音符グリッドをPositionの範囲内で表現できません。");

            var scaledStart = (long)start.ActiveIndex * stepDenominator;
            var scaledStep = (long)start.SectionCount * stepNumerator;
            var firstIndex = scaledStart / scaledStep + (scaledStart % scaledStep == 0 ? 0 : 1);
            for (var index = firstIndex; index * stepNumerator < stepDenominator; index++)
            {
                var position = new Position(start.MeasureIndex, (int)stepDenominator, (int)(index * stepNumerator));
                if (position > groupEnd || (endExclusive != null && position >= endExclusive)) break;
                if (excludedPositions.Contains(position)) continue;

                // ミリ秒に丸める前の位置で除外する。同一時刻に丸められた別の候補は残す。
                timings.Add(timingCalculator.CalculateTimingFromPosition(position));
            }
        }

        private static long GCD(long a, long b)
        {
            while (b != 0)
            {
                var remainder = a % b;
                a = b;
                b = remainder;
            }
            return a;
        }
    }
}
