global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;

global using CleanApiStarter.Application.Common.Models;
global using CleanApiStarter.Application.Features.Auth;
global using CleanApiStarter.Application.Features.Projects;
global using CleanApiStarter.Configuration;
global using CleanApiStarter.Domain.Entities;
global using CleanApiStarter.Infrastructure.Identity;
global using CleanApiStarter.Infrastructure.Persistence;
global using CleanApiStarter.Infrastructure.Repositories;

global using Google.Apis.Auth;

global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.IdentityModel.Tokens;
