using MediatR;

namespace ChatBot.Application.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
