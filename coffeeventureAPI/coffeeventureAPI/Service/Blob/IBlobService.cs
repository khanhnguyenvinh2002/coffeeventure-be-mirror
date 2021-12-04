using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Service.Blob
{
    public interface IBlobService
    {
        Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName);

    }
}
