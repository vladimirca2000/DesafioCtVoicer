using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces; // Necessário para IEmailService
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Entities;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.ValueObjects;

namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Manipulador para o comando CreateUserCommand.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService; // Adicionada injeção de dependência

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService) // Injeção de dependência no construtor
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService; // Atribuição
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Criar o Value Object Email a partir da string de entrada.
        var userEmail = Email.Create(request.Email);

        // 2. Verificar se o e-mail já está em uso
        var existingUser = await _userRepository.GetUserByEmailAsync(userEmail, cancellationToken);
        if (existingUser != null)
        {
            return Result<CreateUserResponse>.Failure($"Já existe um usuário com o e-mail '{request.Email}'.");
        }

        // 3. Criar a nova entidade de usuário
        var newUser = new User
        {
            Name = request.Name,
            Email = userEmail,
            IsActive = request.IsActive,
            CreatedBy = "System"
        };

        // 4. Adicionar o usuário ao repositório
        await _userRepository.AddAsync(newUser, cancellationToken);

        // 5. Salvar as mudanças na unidade de trabalho
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Enviar e-mail de boas-vindas
        await _emailService.SendEmailAsync(
            newUser.Email.Value,
            "Bem-vindo ao ChatBot!",
            $"Olá {newUser.Name},\n\nSeja muito bem-vindo(a) ao nosso sistema de chatbot!\n\nAtenciosamente,\nSua Equipe ChatBot"
        );

        // 7. Retornar a resposta de sucesso
        return Result<CreateUserResponse>.Success(new CreateUserResponse
        {
            UserId = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email.Value,
            CreatedAt = newUser.CreatedAt
        });
    }
}