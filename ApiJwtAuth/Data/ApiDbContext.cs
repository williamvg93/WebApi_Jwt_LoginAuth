using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiJwtAuth.Data
{
    public class ApiDbContext(DbContextOptions<ApiDbContext> options) : IdentityDbContext<User>(options)
    {
    }
}