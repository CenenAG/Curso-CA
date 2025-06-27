using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users;
using MediatR;

namespace CleanArchitecture.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var existingUser = await _userRepository.IsUserExistsAsync(email, cancellationToken);
        if (existingUser)
        {
            return Result.Failure<Guid>(UserErrors.AlreadyExists);
        }

        var nombre = new Nombre(request.Nombre);
        var apellido = new Apellido(request.Apellido);
        var passwordHash = new PasswordHash(BCrypt.Net.BCrypt.HashPassword(request.Password));

        var user = User.Create(
            nombre,
            apellido,
            email,
            passwordHash);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id.Value);
    }
}
