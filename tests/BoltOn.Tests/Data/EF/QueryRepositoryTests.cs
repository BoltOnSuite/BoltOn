using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoltOn.Tests.Data.EF.Fakes;
using BoltOn.Tests.Other;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoltOn.Tests.Data.EF
{
	[Collection("IntegrationTests")]
	public class QueryRepositoryTests : IClassFixture<EFQueryRepositoryFixture>
	{
		private readonly EFQueryRepositoryFixture _fixture;

		public QueryRepositoryTests(EFQueryRepositoryFixture fixture)
		{
			_fixture = fixture;
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetById_WhenRecordExists_ReturnsRecord()
		{
			// arrange
			var schoolDbContext = _fixture.ServiceProvider.GetService<SchoolDbContext>();

			// act
			var result = await _fixture.SubjectUnderTest.GetByIdAsync(1);

			// assert
			Assert.NotNull(result);
			Assert.Equal("a", result.FirstName);
			Assert.DoesNotContain(result, schoolDbContext.ChangeTracker.Entries<Student>().Select(s => s.Entity));
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetById_WhenRecordDoesNotExist_ReturnsNull()
		{
			// arrange

			// act
			var result = await _fixture.SubjectUnderTest.GetByIdAsync(3);

			// assert
			Assert.Null(result);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetByIdAsync_WhenRecordExists_ReturnsRecord()
		{
			// arrange

			// act
			var result = await _fixture.SubjectUnderTest.GetByIdAsync(1);

			// assert
			Assert.NotNull(result);
			Assert.Equal("a", result.FirstName);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetByIdAsync_WhenCancellationRequestedIsTrue_ThrowsOperationCanceledException()
		{
			// arrange
			var cancellationToken = new CancellationToken(true);

			// act
			var exception = await Record.ExceptionAsync(() => _fixture.SubjectUnderTest.GetByIdAsync(1, cancellationToken));

			// assert			
			Assert.NotNull(exception);
			Assert.IsType<OperationCanceledException>(exception);
			Assert.Equal("The operation was canceled.", exception.Message);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetAll_WhenRecordsExist_ReturnsAllTheRecords()
		{
			// arrange

			// act
			var result = (await _fixture.SubjectUnderTest.GetAllAsync()).ToList();

			// assert
			Assert.Equal(4, result.Count);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetAllAsync_WhenRecordsExist_ReturnsAllTheRecords()
		{
			// arrange

			// act
			var result = await _fixture.SubjectUnderTest.GetAllAsync();

			// assert
			Assert.Equal(4, result.Count());
		}

		[Fact, Trait("Category", "Integration")]
		public async Task GetAllAsync_WhenCancellationRequestedIsTrue_ThrowsOperationCanceledException()
		{
			// arrange
			var cancellationToken = new CancellationToken(true);

			// act
			var exception = await Record.ExceptionAsync(() => _fixture.SubjectUnderTest.GetAllAsync(cancellationToken));

			// assert			
			Assert.NotNull(exception);
			Assert.IsType<OperationCanceledException>(exception);
			Assert.Equal("The operation was canceled.", exception.Message);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task FindByWithoutIncludes_WhenRecordsExist_ReturnsRecordsThatMatchesTheFindByCriteria()
		{
			// arrange
			var schoolDbContext = _fixture.ServiceProvider.GetService<SchoolDbContext>();

			// act
			var result = (await _fixture.SubjectUnderTest.FindByAsync(f => f.Id == 2)).FirstOrDefault();

			// assert
			Assert.NotNull(result);
			Assert.Equal("x", result.FirstName);
			Assert.DoesNotContain(result, schoolDbContext.ChangeTracker.Entries<Student>().Select(s => s.Entity));
		}

		[Fact, Trait("Category", "Integration")]
		public async Task FindByWithIncludes_WhenRecordsExist_ReturnsRecordsThatMatchesTheCriteria()
		{
			// act
			var result = (await _fixture.SubjectUnderTest.FindByAsync(f => f.Id == 2, default, i => i.Addresses)).FirstOrDefault();

			// assert
			Assert.NotNull(result);
			Assert.Equal("x", result.FirstName);
			Assert.NotEmpty(result.Addresses);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task FindByAsyncWithIncludes_WhenRecordsExist_ReturnsRecordsThatMatchesTheCriteria()
		{
			// act
			var result = (await _fixture.SubjectUnderTest.FindByAsync(f => f.Id == 2, default, i => i.Addresses)).FirstOrDefault();

			// assert
			Assert.NotNull(result);
			Assert.Equal("x", result.FirstName);
			Assert.NotEmpty(result.Addresses);
		}

		[Fact, Trait("Category", "Integration")]
		public async Task FindByAsync_WhenCancellationRequestedIsTrue_ThrowsOperationCanceledException()
		{
			// arrange
			var cancellationToken = new CancellationToken(true);

			// act
			var exception = await Record.ExceptionAsync(() => _fixture.SubjectUnderTest.FindByAsync(f => f.Id == 2,
				cancellationToken, i => i.Addresses));

			// assert			
			Assert.NotNull(exception);
			Assert.IsType<OperationCanceledException>(exception);
			Assert.Equal("The operation was canceled.", exception.Message);
		}
	}
}
