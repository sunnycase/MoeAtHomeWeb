using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using MoeAtHome.Web.Options;

namespace MoeAtHome.Web.Services
{
    public class LyricsStorageService
    {
        private readonly DirectoryInfo _storageDir;

        public LyricsStorageService(IOptions<LyricsServiceOptions> options)
        {
            _storageDir = new DirectoryInfo(options.Value.StorageDirectory);
        }

        public Stream OpenRead(string fileName)
        {
            return File.OpenRead(Path.Combine(_storageDir.FullName, fileName));
        }

        public async Task Save(Stream source, string fileName)
        {
            using (var file = File.OpenWrite(Path.Combine(_storageDir.FullName, fileName)))
                await source.CopyToAsync(file);
        }
    }
}
