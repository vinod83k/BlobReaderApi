using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using BlobReader.Api.ConfigOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlobReader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : Controller
    {
        private readonly StorageAccountOptions _storageAccountOptions;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly BlobClient _blobClient;

        public BlobsController(IOptions<StorageAccountOptions> storageAccountOptionsAccessor)
        {
            _storageAccountOptions = storageAccountOptionsAccessor.Value;

            // Create a BlobServiceClient object which will be used to create a container client
            _blobServiceClient = new BlobServiceClient(_storageAccountOptions.ConnectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_storageAccountOptions.ContainerName);
            _blobClient = _containerClient.GetBlobClient(_storageAccountOptions.BlobName);
        }

        // GET api/blobs
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    var response = await _blobClient.DownloadToAsync(stream);
                    stream.Position = 0;//resetting stream's position to 0

                    using (var sr = new StreamReader(stream))
                    {
                        string resBody = sr.ReadToEnd();
                        return Ok(resBody);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        
        // POST api/blobs
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(value));
                    await _blobClient.UploadAsync(stream);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }


    }
}