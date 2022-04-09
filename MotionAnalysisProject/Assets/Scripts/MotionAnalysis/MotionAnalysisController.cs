using Graph;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MotionAnalysis
{
    public class MotionAnalysisController : MonoBehaviour
    {
        [SerializeField] private string _fileName = default;
        [SerializeField] private GraphController _graphControllerPrefab = default;
        [SerializeField] private double _compressThreshold = 0.99;
        [SerializeField] private float _motionMaxSecond = 2;

        private List<GraphController> _graphControllers = new List<GraphController>();

        private GraphData<float>[] _graphData = default;
        private GraphData<float>[] _commpressedData = default;
        private GraphData<float>[] _differentiatedData = default;
        private GraphData<float>[] _secondDifferentiatedData = default;

        private void Awake()
        {
            _graphData = GraphData<float>.LoadDataFromCSV(Path.Combine(Application.streamingAssetsPath, _fileName));

            // 値の変化がないデータは除く
            _graphData = _graphData
                .Where(data => (data.YData.Max() - data.YData.Min()) > 0)
                .ToArray();

            for (int i = 0; i < 4; i++)
            {
                var graphController = Instantiate(_graphControllerPrefab, transform);
                var canvas = graphController.GetComponentInChildren<Canvas>();
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                graphController.transform.position = Vector3.up * i * -120;
                _graphControllers.Add(graphController);
            }
            transform.position = new Vector3(250, 175, -350);
        }

        private void Start()
        {
            Exec();

            this.ObserveEveryValueChanged(_ => _compressThreshold)
                .Subscribe(_ => Exec())
                .AddTo(gameObject);

            this.ObserveEveryValueChanged(_ => _motionMaxSecond)
                .Subscribe(_ => Exec())
                .AddTo(gameObject);
        }

        /// <summary>
        /// 実験を実行
        /// </summary>
        private void Exec()
        {
            var allPoints = new List<int>();

            _commpressedData = new GraphData<float>[_graphData.Length];
            _differentiatedData = new GraphData<float>[_graphData.Length];
            _secondDifferentiatedData = new GraphData<float>[_graphData.Length];

            for (int i = 0; i < _graphData.Length; i++)
            {
                // 正規化
                var norm = _graphData[i].YData.Select(e => (double)e).ToArray().Normalize();
                // 階段関数に近似、圧縮
                var comp = norm.Compress(_compressThreshold);
                // 一階微分
                var diff = comp.Differentiate();
                // 二階微分
                var diff2 = diff.Differentiate();
                // 変曲点
                var points = diff2.FindInflectionPoints();
                allPoints.AddRange(points);

                _commpressedData[i] = new GraphData<float>($"{_graphData[i].Label}_comp", comp.Select(e => (float)e).ToArray(), GraphType.Line);
                _differentiatedData[i] = new GraphData<float>($"{_graphData[i].Label}_diff", diff.Select(e => (float)e).ToArray(), GraphType.Line);
                _secondDifferentiatedData[i] = new GraphData<float>($"{_graphData[i].Label}_diff2", diff2.Select(e => (float)e).ToArray(), GraphType.Line);                
            }

            var resultPoints = MotionAnalysisUtilities.SummarizePoints(allPoints.OrderBy(e => e).ToArray(), (int)_motionMaxSecond * 30);
            var pointData = new GraphData<float>($"Points", resultPoints, new float[resultPoints.Length], GraphType.Point);

            Debug.Log($"{_compressThreshold},{_motionMaxSecond},{resultPoints.Length}");

            _graphControllers[0].SetGraphData(_graphData);
            _graphControllers[1].SetGraphData(_commpressedData);
            //_graphControllers[2].SetGraphData(_differentiatedData);
            _graphControllers[2].SetGraphData(_secondDifferentiatedData);

            if (resultPoints.Length > 0)
            {
                var data = new float[resultPoints.Max() + 1];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = 0;
                }
                _graphControllers[3].SetGraphData(new GraphData<float>($"", resultPoints, data, GraphType.Point));
            }

        }
    }
}