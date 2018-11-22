using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthFoodAPI.Models;
using HealthFoodAPI.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace HealthFoodAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesItemsController : ControllerBase
    {
        private readonly HealthFoodAPIContext _context;
        private IConfiguration _configuration;

        public RecipesItemsController(HealthFoodAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/RecipesItems
        [HttpGet]
        public IEnumerable<RecipesItem> GetRecipesItem()
        {
            return _context.RecipesItem;
        }

        // GET: api/RecipesItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipesItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipesItem = await _context.RecipesItem.FindAsync(id);

            if (recipesItem == null)
            {
                return NotFound();
            }

            return Ok(recipesItem);
        }

        // PUT: api/RecipesItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipesItem([FromRoute] int id, [FromBody] RecipesItem recipesItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipesItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(recipesItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipesItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RecipesItems
        [HttpPost]
        public async Task<IActionResult> PostRecipesItem([FromBody] RecipesItem recipesItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RecipesItem.Add(recipesItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecipesItem", new { id = recipesItem.Id }, recipesItem);
        }

        // DELETE: api/RecipesItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipesItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipesItem = await _context.RecipesItem.FindAsync(id);
            if (recipesItem == null)
            {
                return NotFound();
            }

            _context.RecipesItem.Remove(recipesItem);
            await _context.SaveChangesAsync();

            return Ok(recipesItem);
        }

        private bool RecipesItemExists(int id)
        {
            return _context.RecipesItem.Any(e => e.Id == id);
        }

        // GET: api/Meme/Tags
        [Route("Category")]
        [HttpGet]
        public async Task<List<RecipesItem>> GetTags([FromQuery] string category)
        {
            var recipe = from m in _context.RecipesItem
                        select m; //get all the memes


            if (!String.IsNullOrEmpty(category)) //make sure user gave a tag to search
            {
                recipe = recipe.Where(s => s.Category.ToLower().Equals(category.ToLower())); // find the entries with the search tag and reassign
            }




            var returned = await recipe.ToListAsync();

            return returned;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm]RecipesImageItem recipe)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = recipe.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(recipe.Image.FileName, null, stream);
                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    RecipesItem recipeItem = new RecipesItem();
                    recipeItem.Title = recipe.Title;
                    recipeItem.Description = recipe.Description;
                    recipeItem.Category = recipe.Category;
                    recipeItem.Subcategory = recipe.Subcategory;
                    recipeItem.CookTime = recipe.CookTime;
                    recipeItem.TimeUnit = recipe.TimeUnit;
                    recipeItem.Size = recipe.Size;
                    recipeItem.Dietary = recipe.Dietary;
                    recipeItem.Link = recipe.Link;
                    recipeItem.PostName = recipe.PostName;
                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    recipeItem.Height = image.Height.ToString();
                    recipeItem.Width = image.Width.ToString();
                    recipeItem.Image = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    recipeItem.Uploaded = DateTime.Now.ToString();

                    _context.RecipesItem.Add(recipeItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {recipe.Title} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }

    }
}