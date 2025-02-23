using System;
using System.Collections.Generic;
using Uoc.Chart.Notes;

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
        /// <returns>指定された小節内の始点を表す位置</returns>
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

        public Position AddDistance(Distance distance, MeasureLengthProvider measureLengthProvider)
        {
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
            var totalBeatsCount = GetTotalQuarterNoteCount(oldMeasureLengthProvider);
            return CreateFromQuarterNotesCount(totalBeatsCount, newMeasureLengthProvider);
        }

        /// <summary>
        /// 譜面位置までの四分音符の数を求めます。
        /// </summary>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>譜面位置までの四分音符の数</returns>
        public float GetTotalQuarterNoteCount(MeasureLengthProvider measureLengthProvider)
        {
            var totalBeatsCount = 0f;
            for (int i = 0; i < measureIndex.Value; i++)
            {
                totalBeatsCount += measureLengthProvider.GetMeasureLengthAt(i).GetQuarterNoteCount();
            }
            totalBeatsCount += Position01 * measureLengthProvider.GetMeasureLengthAt(measureIndex.Value).GetQuarterNoteCount();
            return totalBeatsCount;
        }

        /// <summary>
        /// 譜面位置までの四分音符の数からPositionを作成します。
        /// </summary>
        /// <param name="beatsCount">譜面位置までの四分音符の数</param>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>Positionインスタンス</returns>
        private static Position CreateFromQuarterNotesCount(float beatsCount, MeasureLengthProvider measureLengthProvider)
        {
            var remainingQuarterNotesCount = beatsCount;
            var measureIndex = 0;

            // どの小節に属するかを判定
            while (remainingQuarterNotesCount >= measureLengthProvider.GetMeasureLengthAt(measureIndex).GetQuarterNoteCount())
            {
                remainingQuarterNotesCount -= measureLengthProvider.GetMeasureLengthAt(measureIndex).GetQuarterNoteCount();
                measureIndex++;
            }

            var numerator = remainingQuarterNotesCount / measureLengthProvider.GetMeasureLengthAt(measureIndex).GetQuarterNoteCount();
            var denominator = 1;

            // 小数がなくなるまで10倍する（Positionのコンストラクタ内で約分されます）
            while (numerator % 1 != 0)
            {
                numerator *= 10;
                denominator *= 10;
            }
            return new Position(new MeasureIndex(measureIndex), denominator, (int)numerator);

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

            // FIXME: 比較する位置の小節長が異なる場合は正確な比較ができない
            var scale = measureIndex.Value + Position01;
            var otherScale = other.measureIndex.Value + other.Position01;
            return scale.CompareTo(otherScale);
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
