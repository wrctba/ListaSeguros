using ListaSeguros.Controllers;
using ListaSeguros.Data;
using ListaSeguros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ListaSeguros.Tests
{
    public class HomeControllerTest
    {
        private readonly ListaSegurosContext _context = null;
        private readonly HomeController _homeController = null;

        public HomeControllerTest()
        {

            if (_context != null)
            {
                _context.Database.EnsureDeleted();
                _context = null;
            }

            // Initialize DbContext in memory
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("ListaSeguros");
            _context = new ListaSegurosContext(optionsBuilder.Options);

            _context.Database.EnsureDeleted();

            _context.Seguros.Add(new Models.Seguro()
            {
                Id = 1,
                CpfCnpj = "123.456.789-12",
                TipoSeguro = Enum.TipoSeguro.Automovel,
                ObjetoSegurado = "AAA-1111"
            });
            _context.Seguros.Add(new Models.Seguro()
            {
                Id = 2,
                CpfCnpj = "12.345.678/0001-01",
                TipoSeguro = Enum.TipoSeguro.Vida,
                ObjetoSegurado = "123.456.789-01"
            });
            _context.Seguros.Add(new Models.Seguro()
            {
                Id = 3,
                CpfCnpj = "123.456.789-12",
                TipoSeguro = Enum.TipoSeguro.Residencial,
                ObjetoSegurado = "Logradouro, Numero - Complemento - Bairro - Cidade/UF"
            });
            _context.SaveChanges();

            // Create test subject
            _homeController = new HomeController(_context);
            
        }

        [Fact]
        public void Index()
        {
            // Act
            var result = _homeController.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Pesquisa>(result.Model);

        }

        [Fact]
        public void Index_Pesquisar_Id_Ok()
        {
            //Arrange
            Pesquisa pesquisa = new Pesquisa() { TipoPesquisa = Enum.TipoPesquisa.Id, Search = "1" };

            // Act
            var result = _homeController.Index(pesquisa) as Microsoft.AspNetCore.Mvc.RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Detail", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal(1, result.RouteValues["Id"]);
        }

        [Fact]
        public void Index_Pesquisar_Id_NotFound()
        {
            //Arrange
            Pesquisa pesquisa = new Pesquisa() { TipoPesquisa = Enum.TipoPesquisa.Id, Search = "999" };

            // Act
            var result = _homeController.Index(pesquisa) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Pesquisa>(result.Model);
            var ret = result.Model as Pesquisa;
            Assert.NotEmpty(ret.Resultado);
        }

        [Fact]
        public void Index_Pesquisar_Placa_Ok()
        {
            //Arrange
            Pesquisa pesquisa = new Pesquisa() { TipoPesquisa = Enum.TipoPesquisa.Placa, Search = "AAA-1111" };

            // Act
            var result = _homeController.Index(pesquisa) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Detail", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal(1, result.RouteValues["Id"]);
        }

        [Fact]
        public void Index_Pesquisar_Placa_NotFound()
        {
            //Arrange
            Pesquisa pesquisa = new Pesquisa() { TipoPesquisa = Enum.TipoPesquisa.Placa, Search = "BBB-2222" };

            // Act
            var result = _homeController.Index(pesquisa) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Pesquisa>(result.Model);
            var ret = result.Model as Pesquisa;
            Assert.NotEmpty(ret.Resultado);
        }

        [Fact]
        public void Detail_OK()
        {
            //Arrange
            
            // Act
            var result = _homeController.Detail(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Seguro>(result.Model);
            Seguro seguro = _context.Seguros.Find(1);
            Assert.Equal(seguro, result.Model);
        }

        [Fact]
        public void Detail_NotFound()
        {
            //Arrange

            // Act
            var result = _homeController.Detail(999) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void List()
        {
            //Arrange

            // Act
            var result = _homeController.List() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Seguro>>(result.Model);
            Assert.Equal(3, ((List<Seguro>)result.Model).Count);
        }

        [Fact]
        public void New()
        {
            //Arrange

            // Act
            var result = _homeController.New() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void New_Post()
        {
            //Arrange
            Seguro seguro = new Seguro() { Id = 4, TipoPessoa = Enum.TipoPessoa.PF, CpfCnpj = "111.111.111.11", TipoSeguro = Enum.TipoSeguro.Automovel, ObjetoSegurado = "XXX-9999" };

            // Act
            var result = _homeController.New(seguro) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal(_context.Seguros.Find(4), seguro);

        }

        [Fact]
        public void Edit()
        {
            //Arrange

            // Act
            var result = _homeController.Edit(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Seguro>(result.Model);
            Assert.Equal(1, ((Seguro)result.Model).Id);
        }

        [Fact]
        public void Edit_Post()
        {
            //Arrange
            Seguro seguro = _context.Seguros.Find(1);
            seguro.ObjetoSegurado = "ZZZ-9999";

            // Act
            var result = _homeController.Edit(1, seguro) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal(_context.Seguros.Find(1).ObjetoSegurado, seguro.ObjetoSegurado);

        }

        [Fact]
        public void Delete()
        {
            //Arrange
            
            // Act
            var result = _homeController.DeleteConfirmed(3) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);

        }
    }
}
