using System;
using Moq;
using opplatApplication.Controllers;
using Xunit;

namespace OpplatApplicationTest
{
    public class LicenciaTest
    {
        [Fact]
        public void CargarLicenciaTest()
        {
            // Arrange
            var context = SetupContexto.GetContextoBase();
            // var m = new Mock<IHostingEnviroment>();
            // var controller = new LicenciaController(, context);

            // Act
            //cierre.Execute();

            // Assert                
            // Assert.Equal(125, context.Set<Disponibilidad>().FirstOrDefault(d => d.Cuenta.Nombre == "Ganancia Peluqueria").Saldo);
            // Assert.Equal(25, context.Set<Disponibilidad>().FirstOrDefault(d => d.Cuenta.Nombre == "Salario Peluqueria").Saldo);
            // Assert.Equal(55.5m, context.Set<Disponibilidad>().FirstOrDefault(d => d.Cuenta.Nombre == "Inversion Peluqueria").Saldo);
        }
    }
}
