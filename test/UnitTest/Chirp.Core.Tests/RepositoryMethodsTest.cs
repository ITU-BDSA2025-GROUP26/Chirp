using System;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Identity;
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
        public void Author_By_Name()
        {
            var cheepDto = new CheepDto
            {
                Text = new string('a', 160),
                Author = "Daid",
                TimeStamp = System.DateTime.Now.ToString()
            };
            
            _repository.AddCheep(cheepDto);
            Author author = _repository.GetAuthorByName("Daid");
            
            
        }
        
        [Fact]
        public void Author_By_Email()
        {
            var cheepDto = new Chirp.Core.CheepDto
            {
                Text = new string('a', 160),
                Author = "Daid",
                TimeStamp = System.DateTime.Now.ToString()
            };
            
            _repository.AddCheep(cheepDto);
            Author author = _repository.GetAuthorByEmail("daid@itu.com");

            Assert.Equal(author.UserName, cheepDto.Author);
        }

        [Fact]
        public void new_author()
        {
            _repository.AddAuthor("John", "john@itu.com");
            _context.SaveChanges();
            Author author = _repository.GetAuthorByName("John");
            Assert.Equivalent(author.Email, "john@itu.com");
        }

        [Fact]
        public void new_cheep_existing_author()
        {
            _repository.AddAuthor("Buba", "buba@itu.com");
            _context.SaveChanges();
            
            var cheepDto = new CheepDto
            {
                Text = new string('a', 160),
                Author = "Buba",
                TimeStamp = System.DateTime.Now.ToString()
            };
            
            _repository.AddCheep(cheepDto);
            _context.SaveChanges();
            
            var cheeps = _repository.GetCheepsFromAuthor("Buba", 1, 1);
            CheepDto cheep = cheeps[0];

            Assert.Equal(cheepDto.Text, cheep.Text);
            Assert.Equal(cheepDto.Author, cheep.Author);
        }

        [Fact]
        public void new_cheep_new_author()
        {
            var cheepDto = new CheepDto
            {
                Text = new string('a', 160),
                Author = "Gump",
                TimeStamp = System.DateTime.Now.ToString()
            };

            _repository.AddCheep(cheepDto);
            _context.SaveChanges(); // 🔧

            var cheeps = _repository.GetCheepsFromAuthor("Gump", 1, 1);
            CheepDto cheep = cheeps[0];

            Author author = _repository.GetAuthorByName("Gump");
            
            Assert.Equal(cheepDto.Text, cheep.Text);
        }
    }
}

