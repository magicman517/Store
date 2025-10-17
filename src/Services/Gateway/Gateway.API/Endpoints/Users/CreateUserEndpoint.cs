using FastEndpoints;
using Gateway.Application.Dto.Users;
using Gateway.Application.Dto.Users.Requests;
using MassTransit;
using Messaging.Contracts;
using Messaging.Contracts.Users.CreateUser;

namespace Gateway.API.Endpoints.Users;

public class CreateUserEndpoint(IRequestClient<CreateUserContractRequest> requestClient) : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();

        Version(1);
        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var (okResponse, failResponse) = await requestClient.GetResponse<CreateUserContractResponse, ContractFailed>(
            new CreateUserContractRequest
            {
                Email = req.Email,
                Password = req.Password,
                FirstName = req.FirstName,
                LastName = req.LastName,
                MiddleName = req.MiddleName,
                Phone = req.Phone
            }, ct);

        if (okResponse.IsCompletedSuccessfully)
        {
            var response = await okResponse;
            await Send.CreatedAtAsync<GetUserByIdEndpoint>(new { response.Message.Id }, cancellation: ct);
        }
        else
        {
            var response = await failResponse;
            AddError(response.Message.Reason);
            await Send.ErrorsAsync(response.Message.StatusCode, ct);
        }
    }
}