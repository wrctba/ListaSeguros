using ListaSeguros.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListaSeguros.Data
{
    public class ListaSegurosContext : DbContext
    {
        public ListaSegurosContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Seguro> Seguros { get; set; }
    }
}
