using Auth.Api.Controllers;
using Auth.Application.Commands;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Api;
using Microsoft.AspNetCore.Mvc;
using Auth.Api.Exceptions;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Auth.Api.Mappers.Extensions;

public static class LogOutMappers
{
    public static IActionResult ToErrorResponse(
        this LogOutCommand command,
        HttpContext context,
        ProblemDetailsFactory factory,
        Error error)
    {
        if (error == OptimisticUnitOfWorkErrors.UpdateConcurrency)
            return new ProblemResultFactory(factory, context).CreateUpdateConflict(resourceName: "User");

        if (error == UserErrors.IdNotFound)
            throw new AuthenticatedUserNotFoundException(command.UserId);
        
        throw new UnhandledApplicationErrorException(error);
    }
}