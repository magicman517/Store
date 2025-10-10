using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data;

public class AuthContext(DbContextOptions<AuthContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options);