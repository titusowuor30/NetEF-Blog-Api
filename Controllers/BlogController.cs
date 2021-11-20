using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetEF_Blog_Api.Data;
using NetEF_Blog_Api.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")] // api/todo
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogDbContext blogContext;

        public BlogController(BlogDbContext context)
        {   
            blogContext = context;
        }

         
        [HttpGet] //get all blogs from db
        public async Task<IActionResult> GetItems()
        {
            var items = await blogContext.Blogs.ToListAsync();
            return Ok(items);
        }

        [HttpPost]//create a new blog
        public async Task<IActionResult> CreateItem(BlogData data)
        {
            if(ModelState.IsValid)
            {
                await blogContext.Blogs.AddAsync(data);
                await blogContext.SaveChangesAsync();

                return CreatedAtAction("GetItem", new {data.id}, data);
            }

            return new JsonResult("Something went wrong") {StatusCode = 500};
        }

        [HttpGet("{id}")] //get a single blog
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await blogContext.Blogs.FirstOrDefaultAsync(x => x.id == id);

            if(item == null)
                return NotFound();

            return Ok(item);
        }
         // search blogs from db
         public async Task<IActionResult> Search(string query)
         {
             var q=from x in blogContext.Blogs select x;
             if(!string.IsNullOrEmpty(query)){
                 q=q.Where(x => x.title.Contains(query) || x.overview.Contains(query));
             }
           return Ok(await q.AsNoTracking().ToListAsync());
         }

        [HttpPut("{id}")]//update blog item
        public async Task<IActionResult> UpdateItem(int id, BlogData item)
        {
            if(id != item.id)
                return BadRequest();

            var existItem = await blogContext.Blogs.FirstOrDefaultAsync(x => x.id == id);

            if(existItem == null)
                return NotFound();

            existItem.title = item.title;
            existItem.overview = item.overview;
            existItem.content = item.content;
            
            // Implement the changes on the database level
            await blogContext.SaveChangesAsync();

            return NoContent();
        }
        
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var existItem = await blogContext.Blogs.FirstOrDefaultAsync(x => x.id == id);

            if(existItem == null)
                return NotFound();

            blogContext.Blogs.Remove(existItem);
            await blogContext.SaveChangesAsync();

            return Ok(existItem);
        }
    }
}