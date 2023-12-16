using Microsoft.Extensions.DependencyInjection;
using EntityStorage.DependencyInjection;
using EntityStorage.Abstractions;
using EntityStorage.CLI.Entities;

var services = new ServiceCollection();

services.AddInMemoryEntityCollection<User>();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var collection = scope.ServiceProvider.GetService<IEntityCollection<User>>();

if (collection == null)
    return;

collection.Add(new User
{
    Id = 1,
    Email = "lars.magnus.nyland@gmail.com",
    Name = "Lars"
});