using FluentAssertions;
using Xunit;

public class PreprocessingScriptStoreTests
{
    [Fact(DisplayName = "Deve falhar ao criar quando o nome estiver vazio")]
    public void Criar_DeveFalhar_QuandoNomeVazio()
    {
        var resultado = PreprocessingScriptStore.Create("", "script", DateTime.UtcNow);

        resultado.IsFailure.Should().BeTrue("o nome não pode ser vazio");
        resultado.Error.Should().Be("Name cannot be empty");
    }

    [Fact(DisplayName = "Deve falhar ao criar quando o conteúdo do script estiver vazio")]
    public void Criar_DeveFalhar_QuandoConteudoVazio()
    {
        var resultado = PreprocessingScriptStore.Create("MeuScript", " ", DateTime.UtcNow);

        resultado.IsFailure.Should().BeTrue("o conteúdo do script não pode estar vazio");
        resultado.Error.Should().Be("Script content cannot be empty");
    }

    [Fact(DisplayName = "Deve criar com sucesso quando os dados forem válidos")]
    public void Criar_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var criadoEm = DateTime.UtcNow;
        var resultado = PreprocessingScriptStore.Create("MeuScript", "print('Olá')", criadoEm);

        resultado.IsSuccess.Should().BeTrue("os dados são válidos");
        resultado.Value.Name.Should().Be("MeuScript");
        resultado.Value.ScriptContent.Should().Be("print('Olá')");
        resultado.Value.CreatedAt.Should().Be(criadoEm);
    }
}
