using System;
using Uoc.Chart;
using Uoc.Chart.Notes;

namespace Uoc
{
    /// <summary>
    /// オブジェクト化されたUOCファイル情報を保持するクラス
    /// </summary>
    public class UocObject
    {
        private readonly ChartPropertyGroup chartPropertyGroup;
        private readonly NoteDefCollection noteDefCollection;
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NoteProfileCollection noteProfileCollection;
        private readonly NoteGroupProfileCollection noteGroupProfileCollection;

        internal UocObject(ChartPropertyGroup chartPropertyGroup, NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection, NoteGroupProfileCollection noteGroupProfileCollection)
        {
            this.chartPropertyGroup = chartPropertyGroup ?? throw new ArgumentNullException(nameof(chartPropertyGroup));
            this.noteDefCollection = noteDefCollection ?? throw new ArgumentNullException(nameof(noteDefCollection));
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.noteProfileCollection = noteProfileCollection ?? throw new ArgumentNullException(nameof(noteProfileCollection));
            this.noteGroupProfileCollection = noteGroupProfileCollection ?? throw new ArgumentNullException(nameof(noteGroupProfileCollection));
        }

        /// <summary>
        /// 譜面プロパティグループ
        /// </summary>
        public ChartPropertyGroup ChartPropertyGroup => chartPropertyGroup;

        /// <summary>
        /// ノート定義コレクション
        /// </summary>
        public NoteDefCollection NoteDefCollection => noteDefCollection;

        /// <summary>
        /// ノートグループ定義コレクション
        /// </summary>
        public NoteGroupDefCollection NoteGroupDefCollection => noteGroupDefCollection;

        /// <summary>
        /// ノートプロファイルコレクション
        /// </summary>
        public NoteProfileCollection NoteProfileCollection => noteProfileCollection;

        /// <summary>
        /// ノートプロファイルコレクション
        /// </summary>
        public NoteGroupProfileCollection NoteGroupProfileCollection => noteGroupProfileCollection;
    }
}
