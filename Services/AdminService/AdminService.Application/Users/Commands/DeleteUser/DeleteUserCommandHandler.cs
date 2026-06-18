using AdminService.Application.Shared.Dtos.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AdminService.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteUserCommandHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("UserServiceClient");

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HTTP context is not available");
            }

            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("JWT token is missing or invalid");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var requestUri = "delete";
            var content = JsonContent.Create(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri)
            {
                Content = content
            };

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Failed to update user. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }
    }
}
