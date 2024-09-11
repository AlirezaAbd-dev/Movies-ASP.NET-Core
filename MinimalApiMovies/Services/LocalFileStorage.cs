using System.Reflection.Metadata.Ecma335;

namespace MinimalApiMovies.Services {
    public class LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileStorage {
        public async Task<string> Store(string container, IFormFile file) {
            var extension = Path.GetExtension(file.FileName);
            var filename = $"{Guid.NewGuid()}{extension}";
            var folder = Path.Combine(env.WebRootPath, container);

            if( !Directory.Exists(folder) ) {
                Directory.CreateDirectory(( folder ));
            }

            var route = Path.Combine(folder, filename);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var content = ms.ToArray();
            await File.WriteAllBytesAsync(route, content);

            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            var host = httpContextAccessor.HttpContext.Request.Host;
            var url = $"{scheme}://{host}";
            var urlFile = Path.Combine(url, container, filename).Replace("\\", "/");

            return urlFile;
        }


        public Task Delete(string? route, string container) {
            if (string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }

            var filename = Path.GetFileName(route);
            var fileDirectory = Path.Combine(env.WebRootPath, container, filename);

            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }
    }
}
