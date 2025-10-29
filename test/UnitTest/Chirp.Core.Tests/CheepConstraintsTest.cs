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
    public class CheepConstraintsTest
    {
        private readonly CheepRepository _repository;
        private readonly ChirpDBContext _context;

        public CheepConstraintsTest()
        {
            var options = new DbContextOptionsBuilder<ChirpDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ChirpDBContext(options);
            _repository = new CheepRepository(_context);
        }

        [Fact]
        public void Cheep_160_Characters()
        {
            var cheepDto = new CheepDto
            {
                Text = new string('a', 160),
                Author = "Daid",
                AuthorEmail = "daid@itu.com",
                TimeStamp = DateTime.Now.ToString()
            };

            var exception = Record.Exception(() => _repository.AddCheep(cheepDto));
            Assert.Null(exception);
        }

        [Fact]
        public void Cheep_161_Characters()
        {
            var cheepDto = new CheepDto
            {
                Text = new string('a', 161),
                Author = "Daid",
                AuthorEmail = "daid@itu.com",
                TimeStamp = DateTime.Now.ToString()
            };

            Assert.Throws<ArgumentException>(() => _repository.AddCheep(cheepDto));
        }
    }
}
