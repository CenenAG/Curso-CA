using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Application.Paginations;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Users.GetUsersPagination;

public class GetUsersPaginationQueryHandler
: IQueryHandler<GetUsersPaginationQuery, PagedResults<User, UserId>>
{
    private readonly IPaginationRepository<User, UserId> _paginationRepository;

    public GetUsersPaginationQueryHandler(IPaginationRepository<User, UserId> paginationRepository)
    {
        _paginationRepository = paginationRepository;
    }

    public async Task<Result<PagedResults<User, UserId>>> Handle(
        GetUsersPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var predicateb = PredicateBuilder.New<User>(true);

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicateb = predicateb.Or(x => x.Nombre == new Nombre(request.Search));
            predicateb = predicateb.Or(x => x.Email == new Email(request.Search));
        }

        var pagedResultUsuarios = await _paginationRepository.GetPaginationAsync(
            predicate: predicateb,
            includes: p => p.Include(x => x.Roles!).ThenInclude(y => y.Permissions!),
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            orderBy: request.OrderBy!,
            isAscending: request.OrderAsc,
            cancellationToken: cancellationToken);

        return pagedResultUsuarios;
    }
}