using AuthMs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;


namespace AuthMs.DataB
{
    public class DataBaseContext : DbContext

    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) 
        { 

        }

        public DbSet<Credential> Credentials { get; set; }  // Credentials es el nombre de la tabla
    }
}
