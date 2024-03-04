# Web API Authorization using JWT and API Key

This is a simple guide to demonstrate how to access secured Web APIs using both JSON Web Tokens (JWT), and API Key.

## Overview <a name="overview"></a>

Most Web APIs require some form of authorization to ensure only validated users and services can access the API. Authorization is also important to control what a user or service can do or access within the API.

This guide will focus on two popular methods of Authorization for Web APIs - JSON Web Tokens (JWT) and API Key.

## Authorization using JWT <a name="jwt"></a>

JSON Web Tokens (JWT) are an open, industry-standard (RFC 7519) method for representing claims securely between two parties. It is a very safe way to handle auth in a web application.

### Example usage:

Here is an example HTTP request using authorization with JWT:

    GET /api/values HTTP/1.1
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

The `Authorization` header field value format is `Bearer {JWT}` where `{JWT}` is the actual JWT token.

## Authorization using API Key <a name="api-key"></a>

An API Key is a simple encrypted string that identifies an application without a secret or any other information. API Keys are very versatile and can be used from source code without any trouble.

### Example usage:

Here is an example HTTP request using an API Key:

    GET /api/values HTTP/1.1
    Api-Key: 123abc456def

The `Api-Key` header field value format is `{API Key}` where `{API Key}` is the actual API key value.

### How to combine both the types of authentication
If we aim to provide dual authentication methods for our web APIs, we have the option of utilizing the AddPolicyScheme method. This method allows for the selection of a distinct AuthenticationScheme based on specific conditions. As illustrated in this repository, when a request includes an X-API-KEY header, it indicates that the API Key is the chosen authentication method for the incoming request, hence the appropriate handler is selected.

```
 services.AddAuthentication(options =>
 {
     options.DefaultAuthenticateScheme = PolicySelector;
     options.DefaultChallengeScheme = PolicySelector;
 })
 .AddPolicyScheme(PolicySelector, PolicySelector, options =>
     {
         options.ForwardDefaultSelector = context =>
         {
             if (context.Request.Headers.TryGetValue(AuthenticationHeader, out var apiKey))
             {
                 return ApiScheme;
             }
             else
                 return JwtScheme;
         };
     })
```

Note that for the API Key authentication, a separate custom Authentication Handler is added as shown below
```
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                       ILoggerFactory logger, 
                                       UrlEncoder encoder, 
                                       ISystemClock clock,
                                       IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        this.configuration = configuration;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthenticationHeader,out var apiKey))
        {
            return AuthenticateResult.Fail("Api Key is missing!!");
        }

        var key = configuration.GetValue<string>(ApiKeySectionName);

        if (key != apiKey)
        {
          return AuthenticateResult.Fail("Invalid Api Key!!");
        }

        var claims = new ClaimsIdentity(nameof(ApiKeyAuthenticationHandler));
        claims.AddClaim(new Claim(ClaimTypes.Role, "User"));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claims), Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```
To select the above handler add appropriate code as below
```
  .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiScheme, options =>
  {

  });
```
