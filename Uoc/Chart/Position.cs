using System;
using System.Collections.Generic;
using System.Numerics;
using Uoc.Chart.Event;

namespace Uoc.Chart
{
    /// <summary>
    /// 譜面の位置情報
    /// -----------------------------
    /// 例) データが「0010」の場合
    /// sectionCount: 4
    /// activeIndex  : 2（0始まり）
    /// -----------------------------
    /// </summary>
    public class Position : IEquatable<Position>, IComparable<Position>
    {
        private readonly MeasureIndex measureIndex;
        private readonly int sectionCount;
        private readonly int activeIndex;

        /// <param name="measureIndex">小節番号</param>
        /// <param name="sectionCount">小節のセクション数</param>
        /// <param name="activeIndex">ノートの有効セクション位置（0始まり）</param>
        public Position(MeasureIndex measureIndex, int sectionCount, int activeIndex)
        {
            if (sectionCount < 1) throw new ArgumentOutOfRangeException(nameof(sectionCount));
            if (activeIndex < 0) throw new ArgumentOutOfRangeException(nameof(activeIndex));
            if (activeIndex >= sectionCount) throw new ArgumentOutOfRangeException(nameof(activeIndex));

            this.measureIndex = measureIndex ?? throw new ArgumentNullException(nameof(measureIndex));

            // 位置を約分して設定
            int gcd = GCD(activeIndex, sectionCount);
            this.sectionCount = sectionCount / gcd;
            this.activeIndex = activeIndex / gcd;
        }

        /// <summary>
        /// 譜面の始点を表す位置
        /// </summary>
        public static Position ChartStart => new(new MeasureIndex(0), 1, 0);

        /// <summary>
        /// 指定された小節内の始点を表す位置
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節内の始点を表す位置</retwAurns>
        public static Position MeasureStart(MeasureIndex measureIndex) => new(measureIndex, 1, 0);

        /// <summary>
        /// 小節番号
        /// </summary>
        public MeasureIndex MeasureIndex => measureIndex;

        /// <summary>
        /// 小節内での位置を 0~1 で表した値
        /// </summary>
        public float Position01 => (float)activeIndex / sectionCount;

        /// <summary>
        /// 小節のセクション数
        /// </summary>
        public int SectionCount => sectionCount;

        /// <summary>
        /// 有効セクション位置（0始まり）
        /// </summary>
        public int ActiveIndex => activeIndex;


        /// <summary>
        /// 譜面位置までの四分音符の数からPositionを作成します。
        /// </summary>
        /// <param name="quarterNoteCount">譜面位置までの四分音符の数</param>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>Positionインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException">quarterNoteCountが有限の非負値でない場合、または位置をintの範囲内の分数で表せない場合にスローされます。</exception>
        public static Position CreateFromQuarterNotesCount(float quarterNoteCount, MeasureLengthProvider measureLengthProvider)
        {
            if (float.IsNaN(quarterNoteCount) || float.IsInfinity(quarterNoteCount) || quarterNoteCount < 0) throw new ArgumentOutOfRangeException(nameof(quarterNoteCount));
            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));

            try
            {
                return CreateFromQuarterNoteFraction(Fraction.FromSingle(quarterNoteCount), measureLengthProvider);
            }
            catch (ArgumentOutOfRangeException exception) when (exception.ParamName == nameof(quarterNoteCount))
            {
                // 近似した整数比がPositionの範囲を超える場合は、2進数の正確な整数比でも確認する。
                return CreateFromQuarterNoteFraction(Fraction.FromSingleExact(quarterNoteCount), measureLengthProvider);
            }
        }

        /// <summary>
        /// 現在位置より後にある最初のスナップ位置を、整数比で計算します。
        /// スナップ間隔は四分音符4個をsnapBeatsで分割した長さです。
        /// </summary>
        public Position GetNextSnapPosition(int snapBeats, MeasureLengthProvider measureLengthProvider)
        {
            if (snapBeats < 1) throw new ArgumentOutOfRangeException(nameof(snapBeats));
            var snapIndex = GetTotalQuarterNoteFraction(measureLengthProvider) * new Fraction(snapBeats, 4);
            var nextIndex = snapIndex.Numerator / snapIndex.Denominator + 1;
            return CreateFromQuarterNoteFraction(new Fraction(nextIndex * 4, snapBeats), measureLengthProvider);
        }

        /// <summary>
        /// 現在位置より前にある最後のスナップ位置を、整数比で計算します。
        /// 譜面の始点ではnullを返します。
        /// </summary>
        public Position? GetPreviousSnapPosition(int snapBeats, MeasureLengthProvider measureLengthProvider)
        {
            if (snapBeats < 1) throw new ArgumentOutOfRangeException(nameof(snapBeats));
            var snapIndex = GetTotalQuarterNoteFraction(measureLengthProvider) * new Fraction(snapBeats, 4);
            var previousIndex = (snapIndex.Numerator + snapIndex.Denominator - 1) / snapIndex.Denominator - 1;
            if (previousIndex.Sign < 0) return null;
            return CreateFromQuarterNoteFraction(new Fraction(previousIndex * 4, snapBeats), measureLengthProvider);
        }

        private static Position CreateFromQuarterNoteFraction(Fraction quarterNoteCount, MeasureLengthProvider measureLengthProvider)
        {
            if (quarterNoteCount.Numerator.Sign < 0) throw new ArgumentOutOfRangeException(nameof(quarterNoteCount));
            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));

            var changes = measureLengthProvider.MeasureLengthChangeEvents;
            var remaining = quarterNoteCount;
            for (var i = 0; i < changes.Count; i++)
            {
                var change = changes[i];
                var measureQuarterNotes = GetMeasureQuarterNoteFraction(change.MeasureLength);
                var startMeasure = change.MeasureIndex.Value;
                if (i + 1 < changes.Count)
                {
                    var span = measureQuarterNotes * new Fraction(changes[i + 1].MeasureIndex.Value - startMeasure, 1);
                    if (remaining.CompareTo(span) >= 0)
                    {
                        remaining -= span;
                        continue;
                    }
                }

                var measures = remaining / measureQuarterNotes;
                var wholeMeasures = System.Numerics.BigInteger.DivRem(measures.Numerator, measures.Denominator, out var remainder);
                var targetMeasure = startMeasure + wholeMeasures;
                if (targetMeasure > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(quarterNoteCount));
                var position = new Fraction(remainder, measures.Denominator);
                if (position.Denominator > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(quarterNoteCount), "譜面位置をintの範囲内の分数で表すことができません。");
                }
                return new Position(new MeasureIndex((int)targetMeasure), (int)position.Denominator, (int)position.Numerator);
            }
            throw new InvalidOperationException("小節長が見つかりません。");
        }

        /// <summary>
        /// 位置が小節の始点である場合にtrueを返します。
        /// </summary>
        /// <returns>位置が小節の始点であるかどうか</returns>
        public bool IsMeasureStart()
        {
            return activeIndex == 0;
        }

        /// <summary>
        /// 位置が譜面の始点である場合にtrueを返します。
        /// </summary>
        /// <returns>位置が譜面の始点であるかどうか</returns>
        public bool IsChartStart()
        {
            return activeIndex == 0 && measureIndex.Value == 0;
        }

        /// <summary>
        /// 位置のティックを計算します。
        /// </summary>
        /// <param name="measureLength">小節長</param>
        /// <param name="tpb">TPB</param>
        /// <returns>計算されたティック</returns>
        public Tick CalculateTick(MeasureLength measureLength, Tpb tpb)
        {
            var tick = tpb.Value * measureLength.GetBeatCount() * Position01;
            return new Tick(tick);
        }

        /// <summary>
        /// 位置のティックをint型で計算します。
        /// </summary>
        /// <param name="measureLength">小節長</param>
        /// <param name="tpb">TPB</param>
        /// <returns>計算されたティック（int）</returns>
        public int CalculateTickInt(MeasureLength measureLength, Tpb tpb)
        {
            var tick = tpb.Value * measureLength.GetBeatCount() * Position01;
            return (int)Math.Floor(tick); // 小数点以下切り捨て
        }

        /// <summary>
        /// 指定された距離をこの位置に加算した新しいPositionを返します。
        /// </summary>
        /// <param name="distance">距離</param>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>距離が加算されたPosition</returns>
        public Position AddDistance(Distance distance, MeasureLengthProvider measureLengthProvider)
        {
            if (distance == null) throw new ArgumentNullException(nameof(distance));
            if (distance.QuarterNoteFraction is Fraction fraction)
            {
                return CreateFromQuarterNoteFraction(GetTotalQuarterNoteFraction(measureLengthProvider) + fraction, measureLengthProvider);
            }
            var totalQuarterNotescount = GetTotalQuarterNoteCount(measureLengthProvider);
            totalQuarterNotescount += distance.QuarterNoteCount;
            return CreateFromQuarterNotesCount(totalQuarterNotescount, measureLengthProvider);
        }

        /// <summary>
        /// 小節長に変更があった際に、絶対的な位置が変動しないようPositionを再計算します。
        /// </summary>
        /// <param name="oldMeasureLengthProvider">変更前の小節長プロバイダ</param>
        /// <param name="newMeasureLengthProvider">変更後の小節長プロバイダ</param>
        /// <returns>再計算されたPosition</returns>
        public Position RecalculatePosition(MeasureLengthProvider oldMeasureLengthProvider, MeasureLengthProvider newMeasureLengthProvider)
        {
            var totalQuarterNoteCount = GetTotalQuarterNoteFraction(oldMeasureLengthProvider);
            return CreateFromQuarterNoteFraction(totalQuarterNoteCount, newMeasureLengthProvider);
        }

        internal Fraction GetTotalQuarterNoteFraction(MeasureLengthProvider measureLengthProvider)
        {
            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            var total = Fraction.Zero;
            var changes = measureLengthProvider.MeasureLengthChangeEvents;
            for (var i = 0; i < changes.Count; i++)
            {
                var change = changes[i];
                if (change.MeasureIndex.Value > measureIndex.Value) break;
                var endMeasure = i + 1 < changes.Count
                    ? Math.Min(changes[i + 1].MeasureIndex.Value, measureIndex.Value)
                    : measureIndex.Value;
                var length = GetMeasureQuarterNoteFraction(change.MeasureLength);
                total += length * new Fraction(endMeasure - change.MeasureIndex.Value, 1);
                if (i + 1 == changes.Count || changes[i + 1].MeasureIndex.Value > measureIndex.Value)
                {
                    return total + length * new Fraction(activeIndex, sectionCount);
                }
            }
            throw new InvalidOperationException("小節長が見つかりません。");
        }

        private static Fraction GetMeasureQuarterNoteFraction(MeasureLength measureLength)
        {
            return new Fraction((BigInteger)measureLength.Numerator * 4, measureLength.Denominator);
        }

        /// <summary>
        /// 譜面位置までの四分音符の数を求めます。
        /// </summary>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>譜面位置までの四分音符の数</returns>
        public float GetTotalQuarterNoteCount(MeasureLengthProvider measureLengthProvider)
        {
            var totalQuarterNoteCount = 0f;
            for (int i = 0; i < measureIndex.Value; i++)
            {
                totalQuarterNoteCount += measureLengthProvider.GetMeasureLengthAt(i).GetQuarterNoteCount();
            }
            totalQuarterNoteCount += Position01 * measureLengthProvider.GetMeasureLengthAt(measureIndex.Value).GetQuarterNoteCount();
            return totalQuarterNoteCount;
        }


        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public bool Equals(Position? other)
        {
            return other is not null &&
                   sectionCount == other.sectionCount &&
                   activeIndex == other.activeIndex &&
                   measureIndex == other.measureIndex;
        }

        public int CompareTo(Position? other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var measureComparison = measureIndex.Value.CompareTo(other.measureIndex.Value);
            if (measureComparison != 0)
            {
                return measureComparison;
            }

            var scaledActiveIndex = (long)activeIndex * other.sectionCount;
            var otherScaledActiveIndex = (long)other.activeIndex * sectionCount;
            return scaledActiveIndex.CompareTo(otherScaledActiveIndex);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(measureIndex, sectionCount, activeIndex);
        }

        public static bool operator ==(Position? left, Position? right)
        {
            return EqualityComparer<Position?>.Default.Equals(left, right);
        }

        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }

        public static bool operator >(Position left, Position right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(Position left, Position right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(Position left, Position right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(Position left, Position right)
        {
            return left.CompareTo(right) <= 0;
        }
    }
}
