using System;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Chirp.Core.Models;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Core.Tests
{
    public class RepositoryMethodsTest
    {
        private readonly CheepRepository _repository;
        private readonly ChirpDBContext _context;

        public RepositoryMethodsTest()
        {
            var options = new DbContextOptionsBuilder<ChirpDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ChirpDBContext(options);
            _repository = new CheepRepository(_context);
        }

        [Fact]
        public Author_By_Name()
        {
            var cheepDto = new Chirp.Core.CheepDto
            {
                Text = new string('a', 160),
                Author = "Daid",
                AuthorEmail = "daid@itu.com",
                TimeStamp = System.DateTime.Now.ToString()
            };
            
            _repository.AddCheep(cheepDto);
            Chirp.Core.Models.Author author = _repository.GetAuthorByName("Daid");

            Xunit.Assert.Equivalent(author.Email, cheepDto.AuthorEmail);
            
        }


    }
}

