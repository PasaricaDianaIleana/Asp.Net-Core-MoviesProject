using Microsoft.EntityFrameworkCore;
using MvcMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovies.Data
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options) : base(options){}
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reviews> Reviews { get; set; }

    }
}

