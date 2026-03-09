using MediatR;

namespace SimpleECommerceBackend.Application.Models.Auth.ConfirmEmail;

public record ConfirmEmailCommand(string Token) : IRequest<ConfirmEmailResult>;