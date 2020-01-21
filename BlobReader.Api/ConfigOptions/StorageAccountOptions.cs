using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobReader.Api.ConfigOptions
{
    public class StorageAccountOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
    }
}
