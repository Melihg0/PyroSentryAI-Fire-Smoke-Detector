using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Services.Interfaces
{        public interface IAIAnalysisService
        {
            Task<List<DetectionResult>> DetectObjectsAsync(Image<Rgb24> image);
        }
        public class DetectionResult
        {
            // Tespit edilen nesnenin etiketi 
            public string Label { get; set; }

            // Tespit edilen nesnenin güven skoru (0-1 arası değer)
            public float Confidence { get; set; }

            // Tespit edilen nesnenin fotoğraf karesindeki yeri ve boyutu
            public RectangleF Box { get; set; }
        }
    }

