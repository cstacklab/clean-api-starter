// using CleanApiStarter.Application.Interfaces;
// using CleanApiStarter.Application.Models;
// using CleanApiStarter.Application.Services;
//
// using CleanApiStarter.Domain.Entities;
//
// using NSubstitute;
//
// using Shouldly;
//
// using Xunit;
//
// namespace CleanApiStarter.UnitTests.Services;
//
// public class WordServiceTests
// {
//     private readonly IWordRepository _wordRepository;
//     private readonly WordService _sut;
//
//     public WordServiceTests()
//     {
//         _wordRepository = Substitute.For<IWordRepository>();
//         _sut = new WordService(_wordRepository);
//     }
//
//     [Theory, AutoData]
//     public async Task AddWordAsync_ShouldMapDtoToDomainAndCallRepository(CreateWordDto wordDto)
//     {
//         // Arrange
//         var expectedId = Guid.NewGuid();
//         _wordRepository.AddWordAsync(Arg.Any<Word>()).Returns(expectedId);
//
//         // Act
//         var result = await _sut.AddWordAsync(wordDto);
//
//         // Assert
//         result.ShouldBe(expectedId);
//         await _wordRepository.Received(1).AddWordAsync(Arg.Is<Word>(w =>
//             w.Text == wordDto.Text &&
//             w.Meaning == wordDto.Meaning &&
//             w.Synonyms.SequenceEqual(wordDto.Synonyms) &&
//             w.UsageExample == wordDto.UsageExample));
//     }
//
//     [Theory, AutoData]
//     public async Task GetWordByIdAsync_WhenWordExists_ShouldReturnMappedDto(Guid id, Word word)
//     {
//         // Arrange
//         _wordRepository.GetWordByIdAsync(id).Returns(word);
//
//         // Act
//         var result = await _sut.GetWordByIdAsync(id);
//
//         // Assert
//         result.Should().NotBeNull();
//         result!.Id.Should().Be(word.Id);
//         result.Text.Should().Be(word.Text);
//         result.Meaning.Should().Be(word.Meaning);
//         result.Synonyms.Should().BeEquivalentTo(word.Synonyms);
//         result.UsageExample.Should().Be(word.UsageExample);
//     }
//
//     [Theory, AutoData]
//     public async Task GetWordByIdAsync_WhenWordDoesNotExist_ShouldReturnNull(Guid id)
//     {
//         // Arrange
//         _wordRepository.GetWordByIdAsync(id).Returns((Word?)null);
//
//         // Act
//         var result = await _sut.GetWordByIdAsync(id);
//
//         // Assert
//         result.Should().BeNull();
//     }
//
//     [Theory, AutoData]
//     public async Task GetAllWordsAsync_ShouldReturnMappedDtos(List<Word> words)
//     {
//         // Arrange
//         _wordRepository.GetAllWordsAsync().Returns(words);
//
//         // Act
//         var result = await _sut.GetAllWordsAsync();
//
//         // Assert
//         result.Should().HaveCount(words.Count);
//         result.Select(dto => dto.Id).Should().BeEquivalentTo(words.Select(w => w.Id));
//     }
//
//     [Theory, AutoData]
//     public async Task UpdateWordAsync_WhenWordExists_ShouldUpdateAndReturnTrue(Guid id, CreateWordDto updateDto, Word existingWord)
//     {
//         // Arrange
//         existingWord.Id = id;
//         _wordRepository.GetWordByIdAsync(id).Returns(existingWord);
//         _wordRepository.UpdateWordAsync(Arg.Any<Word>()).Returns(true);
//
//         // Act
//         var result = await _sut.UpdateWordAsync(id, updateDto);
//
//         // Assert
//         result.Should().BeTrue();
//         await _wordRepository.Received(1).UpdateWordAsync(Arg.Is<Word>(w =>
//             w.Id == id &&
//             w.Text == updateDto.Text &&
//             w.Meaning == updateDto.Meaning &&
//             w.Synonyms.SequenceEqual(updateDto.Synonyms) &&
//             w.UsageExample == updateDto.UsageExample &&
//             w.UpdatedAt != null));
//     }
//
//     [Theory, AutoData]
//     public async Task UpdateWordAsync_WhenWordDoesNotExist_ShouldReturnFalse(Guid id, CreateWordDto updateDto)
//     {
//         // Arrange
//         _wordRepository.GetWordByIdAsync(id).Returns((Word?)null);
//
//         // Act
//         var result = await _sut.UpdateWordAsync(id, updateDto);
//
//         // Assert
//         result.Should().BeFalse();
//         await _wordRepository.DidNotReceive().UpdateWordAsync(Arg.Any<Word>());
//     }
//
//     [Theory, AutoData]
//     public async Task DeleteWordAsync_ShouldCallRepositoryAndReturnResult(Guid id, bool expectedResult)
//     {
//         // Arrange
//         _wordRepository.DeleteWordAsync(id).Returns(expectedResult);
//
//         // Act
//         var result = await _sut.DeleteWordAsync(id);
//
//         // Assert
//         result.Should().Be(expectedResult);
//         await _wordRepository.Received(1).DeleteWordAsync(id);
//     }
// }