using System;

namespace Uoc.Chart.Property
{
    /// <summary>
    /// プロパティを表すクラス
    /// キー（PropertyKey）と値（PropertyValue）を持つ
    /// </summary>
    public class Property
    {
        private readonly PropertyKey key;
        private readonly PropertyValue value;

        public Property(PropertyKey key, PropertyValue value)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Property(string key, string value)
        {
            this.key = new PropertyKey(key);
            this.value = new PropertyValue(value);
        }

        /// <summary>
        /// プロパティキー
        /// </summary>
        public PropertyKey Key => key;

        /// <summary>
        /// プロパティ値
        /// </summary>
        public PropertyValue Value => value;

        /// <summary>
        /// プロパティ値を更新し、新しいインスタンスを返します。
        /// </summary>
        /// <param name="value">新規プロパティ値</param>
        /// <returns>プロパティ値が更新されたインスタンス</returns>
        public Property UpdateValue(PropertyValue value)
        {
            return new Property(key, value);
        }
    }
}
