using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 譜面のプロパティグループ
    /// </summary>
    public class ChartPropertyGroup
    {
        // 仕様で定義されているキーの定数
        private const string KEY_GAME_ID = "GameID";
        private const string KEY_TICKS_PER_BEAT = "TicksPerBeat";
        private const string KEY_TITLE = "Title";

        private readonly PropertyGroup propertyGroup;

        public ChartPropertyGroup(PropertyGroup propertyGroup)
        {
            this.propertyGroup = propertyGroup ?? throw new ArgumentNullException(nameof(propertyGroup));
        }

        /// <summary>
        /// 指定された番号に対応するプロパティ
        /// </summary>
        /// <param name="index">プロパティ番号</param>
        /// <returns>プロパティ</returns>
        public Property this[int index] => propertyGroup[index];

        /// <summary>
        /// プロパティの数
        /// </summary>
        public int Count => propertyGroup.Count;

        public PropertyGroup PropertyGroup => propertyGroup;

        /// <summary>
        /// 指定されたキーに対応するプロパティを取得します。
        /// </summary>
        /// <param name="key">プロパティのキー</param>
        /// <returns>指定されたキーに対応するプロパティ</returns>
        public Property GetPropertyByKey(string key)
        {
            return propertyGroup.GetPropertyByKey(key);
        }

        /// <summary>
        /// ゲームIDを取得します。
        /// </summary>
        /// <returns>ゲームID</returns>
        public string GetGameId()
        {
            var value = GetPropertyByKey(KEY_GAME_ID).Value;
            return value.AsString();
        }

        /// <summary>
        /// TPBを取得します。
        /// </summary>
        /// <returns>TPB</returns>
        public Tpb GetTpb()
        {
            var value = GetPropertyByKey(KEY_TICKS_PER_BEAT).Value;
            return new Tpb(value.AsInt());
        }

        /// <summary>
        /// タイトルを取得します。
        /// </summary>
        /// <returns>タイトル</returns>
        public string GetTitle()
        {
            var value = GetPropertyByKey(KEY_TITLE).Value;
            return value.AsString();
        }
    }
}
