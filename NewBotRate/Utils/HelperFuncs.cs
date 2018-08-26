using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NewBotRate.Utils
{
    public class HelperFuncs
    {
        public static string GetUpTime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        public static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
        public static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
            {
                yield return str.Substring((i < 0) ? 0 : i, Math.Min(maxChunkSize, str.Length - i));
            }
        }

        public static async Task<byte[]> DownloadFileBytes(string url)
        {
            try
            {
                var reqRes = await NewBotRate.Program.httpClient.GetByteArrayAsync(url);
                return reqRes;

            } catch (Exception e)
            {
                return Encoding.ASCII.GetBytes($"Something bad happened! {e.Message}");
            }
        }
    }
}
