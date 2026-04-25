// using CleanApiStarter.Api.Controllers;
// using CleanApiStarter.Application.Interfaces;
// using CleanApiStarter.Application.Models;
// using AutoFixture.Xunit3;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using NSubstitute;
// using Xunit;
//
// namespace CleanApiStarter.UnitTests.Controllers;
//
// public class WordsControllerTests
// {
//     private readonly IWordService _wordService;
//     private readonly WordsController _sut;
//
//     public WordsControllerTests()
//     {
//         _wordService = Substitute.For<IWordService>();
//         _sut = new WordsController(_wordService);
//     }
//
//     [Theory, AutoData]
//     public async Task CreateWord_ShouldReturnCreatedAtAction(CreateWordDto wordDto, Guid wordId)
//     {
//         // Arrange
//         _wordService.AddWordAsync(wordDto).Returns(wordId);
//
//         // Act
//         var result = await _sut.CreateWord(wordDto);
//
//         // Assert
//         var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
//         createdAtActionResult.ActionName.Should().Be(nameof(WordsController.GetWord));
//         createdAtActionResult.RouteValues!.Should().ContainKey("id").WhoseValue.Should().Be(wordId);
//         createdAtActionResult.Value.Should().Be(wordId);
//     }
//
//     [Theory, AutoData]
//     public async Task GetWord_WhenWordExists_ShouldReturnOkWithWord(Guid wordId, WordDto wordDto)
//     {
//         // Arrange
//         _wordService.GetWordByIdAsync(wordId).Returns(wordDto);
//
//         // Act
//         var result = await _sut.GetWord(wordId);
//
//         // Assert
//         var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
//         okResult.Value.Should().Be(wordDto);
//     }
//
//     [Theory, AutoData]
//     public async Task GetWord_WhenWordDoesNotExist_ShouldReturnNotFound(Guid wordId)
//     {
//         // Arrange
//         _wordService.GetWordByIdAsync(wordId).Returns((WordDto?)null);
//
//         // Act
//         var result = await _sut.GetWord(wordId);
//
//         // Assert
//         result.Result.Should().BeOfType<NotFoundResult>();
//     }
//
//     [Theory, AutoData]
//     public async Task GetAllWords_ShouldReturnOkWithAllWords(List<WordDto> words)
//     {
//         // Arrange
//         _wordService.GetAllWordsAsync().Returns(words);
//
//         // Act
//         var result = await _sut.GetAllWords();
//
//         // Assert
//         var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
//         okResult.Value.Should().Be(words);
//     }
//
//     [Theory, AutoData]
//     public async Task UpdateWord_WhenWordExists_ShouldReturnNoContent(Guid wordId, CreateWordDto updateDto)
//     {
//         // Arrange
//         _wordService.UpdateWordAsync(wordId, updateDto).Returns(true);
//
//         // Act
//         var result = await _sut.UpdateWord(wordId, updateDto);
//
//         // Assert
//         result.Should().BeOfType<NoContentResult>();
//     }
//
//     [Theory, AutoData]
//     public async Task UpdateWord_WhenWordDoesNotExist_ShouldReturnNotFound(Guid wordId, CreateWordDto updateDto)
//     {
//         // Arrange
//         _wordService.UpdateWordAsync(wordId, updateDto).Returns(false);
//
//         // Act
//         var result = await _sut.UpdateWord(wordId, updateDto);
//
//         // Assert
//         result.Should().BeOfType<NotFoundResult>();
//     }
//
//     [Theory, AutoData]
//     public async Task DeleteWord_WhenWordExists_ShouldReturnNoContent(Guid wordId)
//     {
//         // Arrange
//         _wordService.DeleteWordAsync(wordId).Returns(true);
//
//         // Act
//         var result = await _sut.DeleteWord(wordId);
//
//         // Assert
//         result.Should().BeOfType<NoContentResult>();
//     }
//
//     [Theory, AutoData]
//     public async Task DeleteWord_WhenWordDoesNotExist_ShouldReturnNotFound(Guid wordId)
//     {
//         // Arrange
//         _wordService.DeleteWordAsync(wordId).Returns(false);
//
//         // Act
//         var result = await _sut.DeleteWord(wordId);
//
//         // Assert
//         result.Should().BeOfType<NotFoundResult>();
//     }
// }