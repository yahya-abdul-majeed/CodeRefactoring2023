﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace CLabManager_API.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Lab> Labs { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Issue> Issues { get; set; }
    }
}
