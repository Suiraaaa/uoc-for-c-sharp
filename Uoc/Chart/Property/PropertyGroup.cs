using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart
{
    /// <summary>
    /// プロパティグループ
    /// </summary>
    public class PropertyGroup
    {
        private readonly IReadOnlyList<Property> properties;

        public PropertyGroup()
        {
            properties = new List<Property>();
        }

        public PropertyGroup(IReadOnlyList<Property> properties)
        {
            this.properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// キー配列と値配列からプロパティグループを作成します。
        /// それぞれの配列の番号同士が対応します。
        /// </summary>
        /// <returns>作成されたプロパティグループインスタンス</returns>
        public static PropertyGroup MergeKeysAndValues(IReadOnlyList<string> keys, IReadOnlyList<string> values)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (keys.Count != values.Count) throw new ArgumentException("プロパティ名とプロパティ値の数が一致しません。");

            var properties = new List<Property>();
            for (int i = 0; i < keys.Count; i++)
            {
                var propertyKey = new PropertyKey(keys[i]);
                var propertyValue = new PropertyValue(values[i]);
                var property = new Property(propertyKey, propertyValue);
                properties.Add(property);
            }
            return new PropertyGroup(properties);
        }

        /// <summary>
        /// キー名の配列から、値を持たないプロパティグループを作成します。
        /// </summary>
        /// <param name="propertyNames">キー名の配列</param>
        /// <returns>作成されたプロパティグループインスタンス</returns>
        public static PropertyGroup CreateFromPropertyNames(IReadOnlyList<string> propertyNames)
        {
            List<Property> properties = new();
            foreach (string propertyName in propertyNames)
            {
                var propertyKey = new PropertyKey(propertyName);
                var propertyValue = PropertyValue.Empty;
                var property = new Property(propertyKey, propertyValue);
                properties.Add(property);
            }
            return new PropertyGroup(properties);
        }

        /// <summary>
        /// 指定された番号に対応するプロパティ
        /// </summary>
        /// <param name="index">プロパティ番号</param>
        /// <returns>プロパティ</returns>
        public Property this[int index] => properties[index];

        /// <summary>
        /// プロパティの数
        /// </summary>
        public int Count => properties.Count;

        /// <summary>
        /// 指定されたキーに対応するプロパティを取得します。
        /// </summary>
        /// <param name="key">取得するプロパティのキー文字列</param>
        /// <returns>キーに対応するプロパティ</returns>
        public Property GetPropertyByKey(string key)
        {
            return GetPropertyByKey(new PropertyKey(key));
        }

        /// <summary>
        /// 指定されたキーに対応するプロパティを取得します。
        /// </summary>
        /// <param name="key">取得するプロパティのキー</param>
        /// <returns>キーに対応するプロパティ</returns>
        public Property GetPropertyByKey(PropertyKey key)
        {
            var property = properties.FirstOrDefault(x => x.Key == key);
            return property ?? throw new KeyNotFoundException($"キーが\"{key.Value}\"のプロパティは見つかりませんでした。");
        }

        /// <summary>
        /// 指定されたキーを持つプロパティが存在する場合にtrueを返します。
        /// </summary>
        /// <param name="key">検証対象キー文字列</param>
        /// <returns>指定されたキーを持つプロパティが存在するかどうか</returns>
        public bool HasKey(string key)
        {
            return HasKey(new PropertyKey(key));
        }

        /// <summary>
        /// 指定されたキーを持つプロパティが存在する場合にtrueを返します。
        /// </summary>
        /// <param name="key">検証対象キー</param>
        /// <returns>指定されたキーを持つプロパティが存在するかどうか</returns>
        public bool HasKey(PropertyKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return properties.Any(x => x.Key == key);
        }

        /// <summary>
        /// すべてのプロパティが値を持つ場合にtrueを返します。
        /// </summary>
        /// <returns>すべてのプロパティが値を持つかどうか</returns>
        public bool AllPropertiesHasValue()
        {
            foreach (var property in properties)
            {
                if (!property.Value.HasValue()) return false;
            }
            return true;
        }

        /// <summary>
        /// プロパティを追加または更新し、新しいインスタンスを返します。
        /// 同一キーを持つプロパティが存在しない場合は、プロパティを追加します。
        /// 同一キーを持つプロパティがすでに存在している場合は、そのプロパティの値を更新します。
        /// </summary>
        /// <param name="property">新規プロパティ</param>
        /// <returns>プロパティが更新されたプロパティグループインスタンス</returns>
        public PropertyGroup AddOrUpdateProperty(Property property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var properties = new List<Property>(this.properties);
            if (!HasKey(property.Key))
            {
                properties.Add(property);
            }
            else
            {
                var index = properties.FindIndex(x => x.Key == property.Key);
                properties[index] = properties[index].UpdateValue(property.Value);
            }
            return new PropertyGroup(properties);
        }

        /// <summary>
        /// 複数のプロパティを追加または更新し、新しいインスタンスを返します。
        /// 同一キーを持つプロパティが存在しない場合は、プロパティを追加します。
        /// 同一キーを持つプロパティがすでに存在している場合は、そのプロパティの値を更新します。
        /// </summary>
        /// <param name="properties">新規プロパティリスト</param>
        /// <returns>プロパティが更新されたプロパティグループインスタンス</returns>
        public PropertyGroup AddOrUpdateProperties(IReadOnlyList<Property> properties)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var editingProperties = new List<Property>(this.properties);
            foreach (var property in properties)
            {
                if (!HasKey(property.Key))
                {
                    editingProperties.Add(property);
                }
                else
                {
                    var index = editingProperties.FindIndex(x => x.Key == property.Key);
                    editingProperties[index] = editingProperties[index].UpdateValue(property.Value);
                }
            }
            return new PropertyGroup(editingProperties);
        }

        /// <summary>
        /// すべてのプロパティの値を文字列のリストとして取得します。
        /// </summary>
        /// <returns>すべてのプロパティの値リスト（文字列）</returns>
        public List<string> GetPropertyValueList()
        {
            return properties.Select(x => x.Value.AsString()).ToList();
        }
    }
}
