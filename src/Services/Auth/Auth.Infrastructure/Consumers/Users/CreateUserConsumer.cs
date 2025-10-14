using Auth.Application.Interfaces;
using MassTransit;
using Messaging.Contracts;
using Messaging.Contracts.Users.CreateUser;

namespace Auth.Infrastructure.Consumers.Users;

public class CreateUserConsumer(IUserManager userManager, IRoleManager roleManager) : IConsumer<CreateUserContractRequest>
{
    public async Task Consume(ConsumeContext<CreateUserContractRequest> context)
    {
        var isFirstUser = !await userManager.HasUsersAsync(context.CancellationToken);
        var roleToAssign = isFirstUser ? "Admin" : "User";

        var createUserResult = await userManager.CreateUserAsync(
            context.Message.Email,
            context.Message.Password,
            context.Message.FirstName,
            context.Message.LastName,
            context.Message.MiddleName,
            context.Message.Phone,
            context.CancellationToken);

        if (!createUserResult.IsSuccess)
        {
            await context.RespondAsync(new ContractFailed
            {
                Reason = createUserResult.Error,
                StatusCode = createUserResult.StatusCode
            });
            return;
        }

        var userId = createUserResult.Value;

        var roleExists = await roleManager.RoleExistsAsync(roleToAssign, context.CancellationToken);
        if (!roleExists)
        {
            await roleManager.CreateRoleAsync(roleToAssign, context.CancellationToken);
        }

        await roleManager.AddToRoleAsync(userId, roleToAssign, context.CancellationToken);

        await context.RespondAsync(new CreateUserContractResponse
        {
            Id = userId
        });
    }
}