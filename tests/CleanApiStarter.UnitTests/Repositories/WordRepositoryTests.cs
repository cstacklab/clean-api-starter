// using System.Data;
// using AutoFixture.Xunit3;
// using CleanApiStarter.Domain.Entities;
// using FluentAssertions;
// using CleanApiStarter.Infrastructure.Database;
// using CleanApiStarter.Infrastructure.Repositories;
// using NSubstitute;
// using Xunit;
//
// namespace CleanApiStarter.UnitTests.Repositories;
//
// public class WordRepositoryTests
// {
//     private readonly IDatabaseConnectionFactory _connectionFactory;
//     private readonly IDbConnection _connection;
//     private readonly WordRepository _sut;
//
//     public WordRepositoryTests()
//     {
//         _connection = Substitute.For<IDbConnection>();
//         _connectionFactory = Substitute.For<IDatabaseConnectionFactory>();
//         _connectionFactory.CreateConnection().Returns(_connection);
//
//         _sut = new WordRepository(_connectionFactory);
//     }
//
//     [Theory, AutoData]
//     public async Task AddWordAsync_ShouldOpenConnectionAndExecuteCommand(Word word)
//     {
//         // This test is a basic structure to show how to mock Dapper
//         // Full implementation would require more complex mocking of connection and command
//
//         // Arrange
//         var expectedId = word.Id;
//
//         // Act
//         await _sut.AddWordAsync(word);
//
//         // Assert
//         _connectionFactory.Received(1).CreateConnection();
//     }
//
//     [Theory, AutoData]
//     public async Task GetWordByIdAsync_ShouldOpenConnectionAndExecuteQuery(Guid id)
//     {
//         // This test is a basic structure to show how to mock Dapper
//         // Full implementation would require more complex mocking of connection and query
//
//         // Arrange
//
//         // Act
//         await _sut.GetWordByIdAsync(id);
//
//         // Assert
//         _connectionFactory.Received(1).CreateConnection();
//     }
//
//     // Additional tests would follow the same pattern
//     // For a real implementation, you would need to mock Dapper's extension methods
//     // which is more complex and may require a test database or more sophisticated mocking
// }