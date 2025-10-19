using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Data;

public class UsersContext(DbContextOptions<UsersContext> options) : DbContext(options)
{
}