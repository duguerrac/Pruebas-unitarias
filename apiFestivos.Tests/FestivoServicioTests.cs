using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Dominio.Entidades;

namespace apiFestivos.Tests
{
    public class FestivoServicioTests
    {
        private readonly Mock<IFestivoRepositorio> _repositorioMock;
        private readonly FestivoServicio _servicio;

        public FestivoServicioTests()
        {
            _repositorioMock = new Mock<IFestivoRepositorio>();
            _servicio = new FestivoServicio(_repositorioMock.Object);
        }

        // 1. Verificar que, si la fecha coincide con una festiva, EsFestivo devuelve true (Caso Positivo)
        [Fact]
        public async Task EsFestivo_FechaEsFestiva_DevuelveTrue()
        {
            // Arrange
            var festivos = new List<Festivo>
            {
                new Festivo { Dia = 25, Mes = 12, Nombre = "Navidad", IdTipo = 1 }
            };
            _repositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.EsFestivo(new DateTime(2024, 12, 25));

            // Assert
            Assert.True(resultado);
        }

        // 2. Probar una fecha que no esté en la lista de festivos y verificar que el resultado sea false
        [Fact]
        public async Task EsFestivo_FechaNoEsFestiva_DevuelveFalse()
        {
            var festivos = new List<Festivo>
            {
                new Festivo { Dia = 25, Mes = 12, Nombre = "Navidad", IdTipo = 1 }
            };
            _repositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            var resultado = await _servicio.EsFestivo(new DateTime(2024, 12, 26));

            Assert.False(resultado);
        }

        // 3. Verificar que, al dar un festivo con tipo 1, se retorne la fecha esperada
        [Fact]
        public async Task ObtenerAño_Tipo1_DevuelveFechaCorrecta()
        {
            var festivos = new List<Festivo>
            {
                new Festivo { IdTipo = 1, Nombre = "Año Nuevo", Mes = 1, Dia = 1 }
            };
            _repositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            var resultado = await _servicio.ObtenerAño(2024);

            Assert.Contains(resultado, f => f.Fecha == new DateTime(2024, 1, 1));
        }

        // 4. Verificar que un festivo movible (tipo 2) caiga en el lunes siguiente a la fecha inicial
        [Fact]
        public async Task ObtenerAño_Tipo2_FestivoMovible_DevuelveLunesSiguiente()
        {
            var festivos = new List<Festivo>
            {
                new Festivo { IdTipo = 2, Nombre = "Día de San José", Mes = 3, Dia = 19 }
            };
            _repositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            var resultado = await _servicio.ObtenerAño(2024);

            // Lunes siguiente al 19 de marzo
            Assert.Contains(resultado, f => f.Fecha == new DateTime(2024, 3, 25));
        }

        // 5. Verificar que un festivo relativo a Pascua (tipo 4) se desplace al lunes correcto
        [Fact]
        public async Task ObtenerAño_Tipo4_FestivoRelativoAPascua_DevuelveLunesCorrecto()
        {
            // Arrange
            var festivos = new List<Festivo>
    {
        new Festivo { IdTipo = 4, Nombre = "Ascensión del Señor", DiasPascua = 40 }
    };
            _repositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(festivos);

            // Act
            var resultado = await _servicio.ObtenerAño(2024);

            // Assert
            Assert.Contains(resultado, f => f.Fecha == new DateTime(2024, 5, 13));
        }

    }
}
