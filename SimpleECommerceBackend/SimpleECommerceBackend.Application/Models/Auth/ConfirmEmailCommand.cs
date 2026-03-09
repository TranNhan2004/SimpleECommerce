using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth;

public record ConfirmEmailCommand(string Token) : IRequest<ConfirmEmailResult>;