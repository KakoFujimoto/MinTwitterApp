using Microsoft.AspNetCore.Http;
using MinTwitterApp.Services;
using SixLabors.ImageSharp.Formats;


namespace MinTwitterApp.Tests.Common;

public class FakeImageFormatDetector : ImageFormatDetector
{
    public new IImageFormat DetectFormat(IFormFile file) => new FakeImageFormat();
    public new bool IsSupportedFormat(IImageFormat format) => true;

    private class FakeImageFormat : IImageFormat
    {
        public string Name => "Fake";
        public string DefaultMimeType => "image/fake";
        public IEnumerable<string> MimeTypes => new[] { "image/fake" };
        public IEnumerable<string> FileExtensions => new[] { ".fake" };
    }
}
