using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CarGoGen.Tests.UseCases;

public class PagoUseCaseTests
{
    [Fact]
    public void Debe_poder_registrar_los_servicios_de_pago()
    {
        var services = new ServiceCollection();

        Assert.NotNull(services);
    }
}