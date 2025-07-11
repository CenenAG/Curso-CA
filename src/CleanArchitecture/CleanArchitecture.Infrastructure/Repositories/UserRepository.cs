using CleanArchitecture.Application.Paginations;
using CleanArchitecture.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository, IPaginationRepository<User, UserId>
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(Domain.Users.Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
        .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> IsUserExistsAsync(Domain.Users.Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
        .AnyAsync(user => user.Email == email, cancellationToken);
    }
}
