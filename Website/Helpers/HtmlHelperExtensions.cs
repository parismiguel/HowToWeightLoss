using Microsoft.AspNetCore.Mvc.Rendering;

namespace Website.Helpers
{
    public enum FitType
    {
        CLip,
        Crop,
        Scale,
        Max
    }
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// https://cdn.buttercms.com/cY4EcqMVS7qCA3IA5apQ
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxSideSize"></param>
        /// <returns></returns>
        public static string GetResizedImageUrl(string originalUrl, int width, int height, FitType fit = FitType.Max)
        {
            if (string.IsNullOrWhiteSpace(originalUrl)) return originalUrl;

            var baseUrl = "https://fs.buttercms.com";
            var id = originalUrl.Substring(originalUrl.LastIndexOf('/'));
            string fitType;

            switch (fit)
            {
                case FitType.CLip:
                    fitType = "clip";
                    break;
                case FitType.Crop:
                    fitType = "crop";
                    break;
                case FitType.Scale:
                    fitType = "scale";
                    break;
                case FitType.Max:
                default:
                    fitType = "max";
                    break;
            }


            var resizeParams = $"resize=width:{width},height:{height},fit:{fitType}";

            return $"{baseUrl}/{resizeParams}/{id.Trim('/')}";
        }

        public static string GetResizedImageUrl(this IHtmlHelper hlp, string originalUrl, int width, int height, FitType fit = FitType.Max)
        {
            return GetResizedImageUrl(originalUrl, width, height, fit);
        }

        public static string GetResizedImageUrl(this IHtmlHelper hlp, string originalUrl, int maxSideSize, FitType fit = FitType.Max)
        {
            return GetResizedImageUrl(originalUrl, maxSideSize, maxSideSize, fit);
        }
    }
}
