using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using PyroSentryAI.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace PyroSentryAI.Services.Implementations
{
    public class AIAnalysisService : IAIAnalysisService, IDisposable
    {
        private readonly InferenceSession _session;
        private readonly string _modelPath;
        private readonly string[] _classLabels = new string[] { "smoke", "fire" };
        public AIAnalysisService()
        {
            try
            {
                _modelPath = System.IO.Path.Combine(AppContext.BaseDirectory, "best.onnx");
                var sessionOptions = new SessionOptions();
                sessionOptions.AppendExecutionProvider_CUDA(0);
                _session = new InferenceSession(_modelPath, sessionOptions);
                System.Diagnostics.Debug.WriteLine("✅ AI Analysis Service started successfully on GPU (CUDA).");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌❌❌ AI Analysis Service failed to start on GPU (CUDA): {ex.Message} ❌❌❌");
                var cpuSessionOptions = new SessionOptions
                {
                    // Paralel işlem sayısını sistemin işlemci çekirdek sayısına göre ayarlar.
                    IntraOpNumThreads = Environment.ProcessorCount,
                    // Modelin içindeki operasyonların nasıl paralelleştirileceğini optimize eder.
                    ExecutionMode = ExecutionMode.ORT_PARALLEL,
                    // Model yüklenirken yapılabilecek tüm optimizasyonları etkinleştirir.
                    GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
                };
                _session = new InferenceSession(_modelPath, cpuSessionOptions);
            }

        }
        public async Task<List<DetectionResult>> DetectObjectsAsync(Image<Rgb24> image)
        {
            List<DetectionResult> detectionResults = await Task.Run(() =>
            {


                const int TargetWidth = 640;
                const int TargetHeight = 640;
                var xRatio = (float)image.Width / TargetWidth;
                var yRatio = (float)image.Height / TargetHeight;

                using var resizedImage = image.Clone(ctx => ctx.Resize(TargetWidth, TargetHeight));
                var inputTensor = new DenseTensor<float>(new[] { 1, 3, TargetHeight, TargetWidth });

                for (int y = 0; y < TargetHeight; y++)
                {
                    for (int x = 0; x < TargetWidth; x++)
                    {
                        Rgb24 pixel = resizedImage[x, y];
                        inputTensor[0, 0, y, x] = pixel.R / 255.0f;
                        inputTensor[0, 1, y, x] = pixel.G / 255.0f;
                        inputTensor[0, 2, y, x] = pixel.B / 255.0f;
                    }
                }

                var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("images", inputTensor)
        };

                using var results = _session.Run(inputs);
                var outputTensor = results.FirstOrDefault()?.AsTensor<float>();

                if (outputTensor == null)
                {
                    return new List<DetectionResult>();
                }

                var detections = new List<DetectionResult>();
                for (int i = 0; i < outputTensor.Dimensions[2]; i++)
                {
                    float maxScore = 0;
                    int bestClassIndex = -1;
                    for (int j = 0; j < _classLabels.Length; j++)
                    {
                        var score = outputTensor[0, 4 + j, i];
                        if (score > maxScore)
                        {
                            maxScore = score;
                            bestClassIndex = j;
                        }
                    }

                    if (maxScore > 0.3f)
                    {
                        var x_center = outputTensor[0, 0, i];
                        var y_center = outputTensor[0, 1, i];
                        var width = outputTensor[0, 2, i];
                        var height = outputTensor[0, 3, i];

                        var box = new RectangleF(
                            (x_center - width / 2) * xRatio,
                            (y_center - height / 2) * yRatio,
                            width * xRatio,
                            height * yRatio);

                        detections.Add(new DetectionResult
                        {
                            Label = _classLabels[bestClassIndex],
                            Confidence = maxScore,
                            Box = box
                        });
                    }
                }
                return detections;
            });
            return detectionResults;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
