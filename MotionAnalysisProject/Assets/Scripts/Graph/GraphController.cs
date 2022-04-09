using System;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Graph
{
    public class GraphController : MonoBehaviour
    {
        [SerializeField] private string _fileName = default;

        [SerializeField] private RawImage _graphImage = default;
        [SerializeField] private Text _labelText = default;
        [SerializeField] private Slider _xOffsetSlider = default;
        [SerializeField] private Slider _yOffsetSlider = default;
        [SerializeField] private Slider _xSizeSlider = default;
        [SerializeField] private Slider _ySizeSlider = default;

        private GraphData<float>[] _graphData = default;
        private Texture2D _texture = default;

        private int OffsetX { get { return (int)_xOffsetSlider.value; } }
        private int OffsetY { get { return (int)_yOffsetSlider.value; } }
        private float SizeX { get { return _xSizeSlider.value; } }
        private float SizeY { get { return _ySizeSlider.value; } }

        private readonly Color[] _colors =
{
            Color.blue,
            Color.green,
            Color.red,
            Color.yellow,
            Color.cyan,
            Color.magenta,
        };

        private int colorIndex = 0;

        private void Awake()
        {
            // どれかのスライダーの値が変化したら、グラフを更新
            Observable
                .CombineLatest(
                _xOffsetSlider.OnValueChangedAsObservable(),
                _yOffsetSlider.OnValueChangedAsObservable(),
                _xSizeSlider.OnValueChangedAsObservable(),
                _ySizeSlider.OnValueChangedAsObservable())
                .Where(_ => _graphData != null)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ =>
                {
                    UpdateGraph();
                    _xOffsetSlider.maxValue = _graphData[0].YData.Count() * _xSizeSlider.value;
                })
                .AddTo(gameObject);
        }

        private void Start()
        {
            // ファイル名が設定されていたら、読み込む
            if (!string.IsNullOrEmpty(_fileName))
            {
                var datas = GraphData<float>.LoadDataFromCSV(Path.Combine(Application.streamingAssetsPath, _fileName));

                // 値の変化がないデータは除く
                datas = datas
                    .Where(data => (data.YData.Max() - data.YData.Min()) > 0)
                    .ToArray();

                SetGraphData(datas);
            }

        }

        private void OnGUI()
        {
            if (_graphImage == null)
            {
                var rect = new Rect(0, 0, Screen.width, Screen.height);
                GUI.DrawTexture(rect, _texture, ScaleMode.StretchToFill);
            }
        }

        /// <summary>
        /// グラフに描画するデータを登録
        /// </summary>
        /// <param name="graphDatas">グラフデータ</param>
        public void SetGraphData(params GraphData<float>[] graphDatas)
        {
            _graphData = graphDatas;

            // 描画領域を決定
            if (_graphImage != null)
            {
                int width = (int)_graphImage.rectTransform.rect.width;
                int height = (int)_graphImage.rectTransform.rect.height;
                _texture = new Texture2D(width, height);
                _graphImage.texture = _texture;
            }
            else
            {
                _texture = new Texture2D(Screen.width, Screen.height);
            }

            UpdateGraph();
        }

        /// <summary>
        /// グラフを更新
        /// </summary>
        private void UpdateGraph()
        {
            _texture.Clear(Color.black);

            DrawGrid(_texture, 10, 1, Color.gray);

            colorIndex = 0;

            _labelText.text = "";
            foreach (var data in _graphData)
            {
                var color = _colors[colorIndex];
                DrawData(_texture, data, color);
                color.a = 0.8f;
                _labelText.text += $"<color=#{ColorUtility.ToHtmlStringRGB(color)}> -{data.Label}</color>  ";

                colorIndex = (colorIndex + 1) % _colors.Length;
            }
            _texture.Apply();
        }

        /// <summary>
        /// グラフにグリッドを描画
        /// </summary>
        /// <param name="texture">描画対象テクスチャ</param>
        /// <param name="xSize">1グリッドの横幅</param>
        /// <param name="ySize">1グリッドの縦幅</param>
        /// <param name="color">描画色</param>
        private void DrawGrid(Texture2D texture, float xSize, float ySize, Color color)
        {
            for (int y = -texture.height; y < texture.height; y++)
            {
                var start = new Vector2Int(0, (int)(y * ySize * SizeY) - OffsetY);
                var end = new Vector2Int(texture.width, (int)(y * ySize * SizeY) - OffsetY);

                texture.SetLine(start, end, color, step: 5);
            }

            for (int x = -texture.width; x < texture.width; x++)
            {
                var start = new Vector2Int((int)(x * xSize * SizeX) - OffsetX, 0);
                var end = new Vector2Int((int)(x * xSize * SizeX) - OffsetX, texture.height);

                texture.SetLine(start, end, color, step: 5);
            }
        }

        /// <summary>
        /// データの描画
        /// </summary>
        /// <param name="texture">描画対象</param>
        /// <param name="graphData">描画するデータ</param>
        /// <param name="color">描画色</param>
        private void DrawData(Texture2D texture, GraphData<float> graphData, Color color)
        {
            switch (graphData.GraphType)
            {
                case GraphType.Line:
                    for (int x_i = 0; x_i < graphData.XData.Count - 1; x_i++)
                    {
                        int x0 = (int)(graphData.XData[x_i] * SizeX) - OffsetX;
                        int x1 = (int)(graphData.XData[x_i + 1] * SizeX) - OffsetX;
                        int y0 = (int)(graphData.YData[graphData.XData[x_i]] * SizeY) + (texture.height / 2) - OffsetY;
                        int y1 = (int)(graphData.YData[graphData.XData[x_i + 1]] * SizeY) + (texture.height / 2) - OffsetY;

                        texture.SetLine(new Vector2Int(x0, y0), new Vector2Int(x1, y1), color);
                    }
                    break;

                case GraphType.Point:
                    for (int x_i = 0; x_i < graphData.XData.Count; x_i++)
                    {
                        int x0 = (int)(graphData.XData[x_i] * SizeX) - OffsetX;
                        int y0 = (int)(graphData.YData[graphData.XData[x_i]] * SizeY) + (texture.height / 2) - OffsetY;

                        texture.SetPoint(new Vector2Int(x0, y0), 7, color);
                    }
                    break;
            }
        }
    }
}