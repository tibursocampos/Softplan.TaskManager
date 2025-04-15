global using Bogus;

global using FluentAssertions;

global using FluentValidation.TestHelper;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Moq;

global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;

global using TaskManager.API.Controllers;
global using TaskManager.API.Middleware;
global using TaskManager.API.Models;
global using TaskManager.API.Tests.Fixtures;
global using TaskManager.API.Tests.Integration.Config;
global using TaskManager.Core.Entities;
global using TaskManager.Core.Interfaces;
global using TaskManager.Infra;
global using TaskManager.Core.Exceptions;
