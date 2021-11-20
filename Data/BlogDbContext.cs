using Microsoft.EntityFrameworkCore;
using NetEF_Blog_Api.Models;

namespace NetEF_Blog_Api.Data
{
 public class BlogDbContext : DbContext
 {
     public virtual DbSet<BlogData> Blogs {get;set;}

     public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options){}
 }

}