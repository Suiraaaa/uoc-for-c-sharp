using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートを構成する情報を持つクラス
    /// </summary>
    public class NoteProfile
    {
        private readonly NoteDef noteDef;
        private readonly Position position;
        private readonly PropertyGroup propertyGroup;
        private readonly Layer layer;
        private readonly Channel channel;
        private readonly NoteGuid guid;

        public NoteProfile(NoteDef noteDef, Position position, IReadOnlyList<string> propertyValues, Layer layer, Channel channel, NoteGuid guid)
        {
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            this.noteDef = noteDef ?? throw new ArgumentNullException(nameof(noteDef));
            this.position = position ?? throw new ArgumentNullException(nameof(position));
            this.layer = layer ?? throw new ArgumentNullException(nameof(layer));
            this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this.guid = guid ?? throw new ArgumentNullException(nameof(guid));
            propertyGroup = PropertyGroup.MergeKeysAndValues(noteDef.PropertyNames, propertyValues);
        }

        public NoteProfile(NoteDef noteDef, Position position, IReadOnlyList<string> propertyValues, Layer layer, Channel channel)
        {
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            this.noteDef = noteDef ?? throw new ArgumentNullException(nameof(noteDef));
            this.position = position ?? throw new ArgumentNullException(nameof(position));
            this.layer = layer ?? throw new ArgumentNullException(nameof(layer));
            this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
            propertyGroup = PropertyGroup.MergeKeysAndValues(noteDef.PropertyNames, propertyValues);
            guid = new NoteGuid();
        }

        /// <summary>
        /// ノート定義
        /// </summary>
        public NoteDef NoteDef => noteDef;

        /// <summary>
        /// ノートの位置
        /// </summary>
        public Position Position => position;

        /// <summary>
        /// プロパティグループ
        /// </summary>
        public PropertyGroup PropertyGroup => propertyGroup;

        /// <summary>
        /// レイヤー
        /// </summary>
        public Layer Layer => layer;

        /// <summary>
        /// チャンネル
        /// </summary>
        public Channel Channel => channel;

        /// <summary>
        /// ノートのGuid
        /// </summary>
        public NoteGuid Guid => guid;

        /// <summary>
        /// 譜面位置を更新します。
        /// </summary>
        /// <param name="position">譜面位置</param>
        /// <returns>譜面位置が更新されたNoteProfile</returns>
        public NoteProfile UpdatePosition(Position position)
        {
            return new NoteProfile(noteDef, position, propertyGroup.GetPropertyValueList(), layer, channel, guid);
        }

        /// <summary>
        /// プロパティグループを更新します。
        /// </summary>
        /// <param name="propertyGroup">プロパティグループ</param>
        /// <returns>プロパティグループが更新されたNoteProfile</returns>
        public NoteProfile UpdatePropertyGroup(PropertyGroup propertyGroup)
        {
            return new NoteProfile(noteDef, position, propertyGroup.GetPropertyValueList(), layer, channel, guid);
        }

        /// <summary>
        /// チャンネルを更新します。
        /// </summary>
        /// <param name="channel">チャンネル</param>
        /// <returns>チャンネルが更新されたNoteProfile</returns>
        public NoteProfile UpdateChannel(Channel channel)
        {
            return new NoteProfile(noteDef, position, propertyGroup.GetPropertyValueList(), layer, channel, guid);
        }

        public bool IsBpmChangeNote()
        {
            return noteDef.NoteId == NoteDef.BpmChange.NoteId;
        }

        public bool IsSpeedChangeNote()
        {
            return noteDef.NoteId == NoteDef.SpeedChange.NoteId;
        }

        public bool IsMeasureLengthChangeNote()
        {
            return noteDef.NoteId == NoteDef.MeasureLengthChange.NoteId;
        }

        internal BpmChangeEvent ToBpmChangeEvent(MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            if (!IsBpmChangeNote()) throw new InvalidOperationException("ノートがBPMChangeNoteではないため、BPMChangeEventに変換できません。");
            var bpm = new Bpm(propertyGroup.GetPropertyByKey("bpm").Value.AsInt());
            return new BpmChangeEvent(position, bpm, measureLengthProvider, tpb);
        }

        internal SpeedChangeEvent ToSpeedChangeEvent(MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            if (!IsSpeedChangeNote()) throw new InvalidOperationException("ノートがSpeedChangeNoteではないため、SpeedChangeEventに変換できません。");
            var speedMultiplier = new SpeedMultiplier(propertyGroup.GetPropertyByKey("speed").Value.AsInt());
            return new SpeedChangeEvent(position, layer, speedMultiplier, measureLengthProvider, tpb);
        }

        internal MeasureLengthChangeEvent ToMeasureLengthChangeEvent()
        {
            if (!IsMeasureLengthChangeNote()) throw new InvalidOperationException("ノートがMeasureLengthChangeNoteではないため、MeasureLengthChangeEventに変換できません。");
            int numerator = propertyGroup.GetPropertyByKey("numerator").Value.AsInt();
            int denominator = propertyGroup.GetPropertyByKey("denominator").Value.AsInt();
            var measureLength = new MeasureLength(numerator, denominator);
            return new MeasureLengthChangeEvent(position, measureLength);
        }
    }
}
