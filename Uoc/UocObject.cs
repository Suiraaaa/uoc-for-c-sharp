using System;
using Uoc.Analyze;
using Uoc.Analyze.Playback;
using Uoc.Chart.Notes;
using Uoc.Chart.Notes.Definition;
using Uoc.Chart.Property;

namespace Uoc
{
    /// <summary>
    /// UOCファイルに含まれるすべての情報を保持するクラス
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
        /// 譜面プロパティ
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
        /// ノートグループプロファイルコレクション
        /// </summary>
        public NoteGroupProfileCollection NoteGroupProfileCollection => noteGroupProfileCollection;

        /// <summary>
        /// 譜面再生データを作成します。
        /// </summary>
        /// <param name="analysisSetting">解析設定</param>
        /// <returns>譜面再生データ</returns>
        public ChartPlaybackData CreateChartPlaybackData(AnalysisSetting analysisSetting)
        {
            return ChartPlaybackData.Create(noteDefCollection, noteGroupDefCollection, noteProfileCollection, noteGroupProfileCollection, analysisSetting, chartPropertyGroup.GetTpb());
        }
    }
}
