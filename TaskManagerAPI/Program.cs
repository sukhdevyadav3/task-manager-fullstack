using Microsoft.AspNetCore.Authentication.JwtBearer;  // Required for JWT Bearer authentication.
using Microsoft.EntityFrameworkCore;  // Required to use EF Core and connect to SQL Server.
using Microsoft.IdentityModel.Tokens;  // For working with JWT tokens and signing keys.
using Microsoft.OpenApi.Models;  // Used to configure Swagger documentation.
using System.Text;  // For encoding/decoding the JWT signing key.
using TaskManagerAPI.Data;  // Namespace for the TaskManagerAPI Data layer, including DbContext.

var builder = WebApplication.CreateBuilder(args);  // Create the WebApplication builder to configure services.

/// Add Controllers for the app, enabling API functionality.
builder.Services.AddControllers();

// ✅ Configure CORS (Cross-Origin Resource Sharing) to allow specific origins and methods.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Allow specific origins that can make requests to the API.
        policy.WithOrigins(
            "http://127.0.0.1:5500",  // Local development frontend.
            "http://localhost:5500",   // Another local frontend origin.
            "http://localhost:7246",   // Another local frontend origin.
            "https://taskmanager-frontend-production.up.railway.app"  // Production frontend origin.
        )
        .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.).
        .AllowAnyHeader()  // Allow any headers in requests.
        .AllowCredentials();  // Allow credentials (cookies, authentication).
    });
});

// ✅ JWT Authentication Setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");  // Retrieve JWT settings from configuration.
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);  // Get the JWT secret key from configuration and convert to bytes.

// Add authentication services using JWT Bearer token authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Default scheme is JWT Bearer.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // Default challenge is JWT Bearer.
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;  // Disable HTTPS requirement for local development (set to true in production).
    options.SaveToken = true;  // Save the token in the authentication context.
    options.TokenValidationParameters = new TokenValidationParameters  // Set up validation parameters for JWT.
    {
        ValidateIssuer = true,  // Validate that the token was issued by a trusted issuer.
        ValidateAudience = true,  // Validate that the token is intended for a valid audience.
        ValidateLifetime = true,  // Validate that the token is not expired.
        ValidateIssuerSigningKey = true,  // Validate the signing key to ensure the token is valid.
        ValidIssuer = jwtSettings["Issuer"],  // Set the valid issuer of the token.
        ValidAudience = jwtSettings["Audience"],  // Set the valid audience for the token.
        IssuerSigningKey = new SymmetricSecurityKey(key)  // Use the secret key for signing and validating the token.
    };
});

// ✅ Add EF Core and configure the SQL Server connection using connection string from configuration.
builder.Services.AddDbContext<TaskItemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerConnectionString")));

// ✅ Configure Swagger for API documentation with JWT token support.
builder.Services.AddEndpointsApiExplorer();  // Enable API exploration for Swagger.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManagerAPI", Version = "v1" });  // Set Swagger API title and version.

    // Configure the JWT bearer security scheme for Swagger UI.
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "Bearer",  // Security scheme type is Bearer.
        BearerFormat = "JWT",  // Specify the format as JWT.
        Name = "Authorization",  // Set the header name as "Authorization".
        In = ParameterLocation.Header,  // Specify that the token will be passed in the HTTP header.
        Type = SecuritySchemeType.Http,  // Type of security scheme is HTTP.
        Description = "Enter 'Bearer {your JWT token}'",  // Description for the user on how to use JWT.
        Reference = new OpenApiReference
        {
            Id = "Bearer",  // Reference ID for the security definition.
            Type = ReferenceType.SecurityScheme  // The type of reference is a security scheme.
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);  // Add the security definition to Swagger.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }  // Require the Bearer token in Swagger requests.
    });
});

// ✅ Build the application.
var app = builder.Build();

// ✅ Middleware Pipeline Configuration.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Enable Swagger in development environment.
    app.UseSwaggerUI();  // Enable Swagger UI for browsing the API documentation.
}

app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS.

app.UseCors("AllowFrontend");  // Apply the CORS policy to allow requests from specified origins.

app.UseAuthentication();  // Enable JWT authentication middleware.
app.UseAuthorization();  // Enable authorization middleware.

app.MapControllers();  // Map the controller actions to the appropriate endpoints.

app.Run();  // Run the application.
