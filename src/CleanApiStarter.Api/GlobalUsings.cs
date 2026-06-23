// Global using directives

global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Encodings.Web;

global using CleanApiStarter.Api;
global using CleanApiStarter.Api.Common.Interfaces;
global using CleanApiStarter.Api.Common.Models;
global using CleanApiStarter.Api.Configuration;
global using CleanApiStarter.Api.Domain.Entities;
global using CleanApiStarter.Api.Features.Auth;
global using CleanApiStarter.Api.Features.Projects;
global using CleanApiStarter.Api.Features.Projects.Tasks;
global using CleanApiStarter.Api.Infrastructure;
global using CleanApiStarter.Api.Infrastructure.Identity;
global using CleanApiStarter.Api.Infrastructure.Persistence;
global using CleanApiStarter.Api.Infrastructure.Repositories;
global using CleanApiStarter.Api.Services;
global using CleanApiStarter.AspNetCoreDefaults;

global using FluentValidation;

global using Google.Apis.Auth;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
