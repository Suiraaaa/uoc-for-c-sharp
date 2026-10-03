using System;
using Uoc.Chart.Notes;
using Uoc.Chart.Property;

namespace Uoc.Analyze.Playback
{
    /// <summary>
    /// 単体ノートの再生に関する情報を提供するクラス
    /// </summary>
    public class NotePlaybackProvider
    {
        private readonly NoteProfile noteProfile;
        private readonly NotePlaybackCalculator playbackCalculator;
        private readonly long instantiateTiming;
        private readonly long enabledTiming;

        internal NotePlaybackProvider(NoteProfile noteProfile, NotePlaybackCalculator playbackCalculator)
        {
            this.noteProfile = noteProfile ?? throw new ArgumentNullException(nameof(noteProfile));
            this.playbackCalculator = playbackCalculator ?? throw new ArgumentNullException(nameof(playbackCalculator));
            enabledTiming = playbackCalculator.CalculateEnabledTiming(noteProfile.Position);
            instantiateTiming = playbackCalculator.CalculateInstantiateTiming(noteProfile.Layer, enabledTiming);
        }

        /// <summary>
        /// ノートID
        /// </summary>
        public NoteId NoteId => noteProfile.NoteDef.NoteId;

        /// <summary>
        /// ノートが持つプロパティ
        /// </summary>
        public PropertyGroup NoteProperties => noteProfile.PropertyGroup;

        /// <summary>
        /// ノートの生成タイミング
        /// </summary>
        public long InstantiateTiming => instantiateTiming;

        /// <summary>
        /// ノートの有効タイミング
        /// </summary>
        public long EnabledTiming => enabledTiming;

        /// <summary>
        /// ノートのGuid
        /// </summary>
        public Guid Guid => noteProfile.Guid;

        /// <summary>
        /// タイミングからノートの位置を求めます。
        /// ノート生成位置を1、判定位置を0とします。
        /// 速度倍率が正の場合、判定位置を通過した後は負の値をとります。
        /// </summary>
        /// <param name="timing">タイミング</param>
        /// <returns>ノートの位置</returns>
        public float CalculateNotePosition(long timing)
        {
            return playbackCalculator.CalculateNotePosition(noteProfile.Layer, enabledTiming, timing);
        }
    }
}
