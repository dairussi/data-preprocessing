using FluentAssertions;
using Xunit;

public class ProcessDataTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WithValidInput()
    {
        var processId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var result = ProcessData.Create(1, processId, createdAt);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProcessId.Should().Be(processId);
        result.Value.StatusProcess.Should().Be(StatusProcess.InProgress);
        result.Value.PreprocessingScriptStoreId.Should().Be(1);
    }

    [Fact]
    public void WithData_ShouldReturnFailure_WhenDataIsEmpty()
    {
        var process = ProcessData.Create(1, Guid.NewGuid(), DateTime.UtcNow).Value;
        var result = process.WithData("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("O resultado do processamento está vazio");
    }

    [Fact]
    public void WithData_ShouldReturnSuccess_WhenDataIsValid()
    {
        var process = ProcessData.Create(1, Guid.NewGuid(), DateTime.UtcNow).Value;
        var result = process.WithData("resultado");

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().Be("resultado");
        result.Value.StatusProcess.Should().Be(StatusProcess.Completed);
    }

    [Fact]
    public void WithError_ShouldUpdateStatusToFailed()
    {
        var process = ProcessData.Create(1, Guid.NewGuid(), DateTime.UtcNow).Value;
        var result = process.WithError("Erro ocorreu");

        result.IsSuccess.Should().BeTrue();
        result.Value.ErrorMessage.Should().Be("Erro ocorreu");
        result.Value.StatusProcess.Should().Be(StatusProcess.Failed);
    }

    [Fact]
    public void WithStatus_ShouldUpdateStatusCorrectly()
    {
        var process = ProcessData.Create(1, Guid.NewGuid(), DateTime.UtcNow).Value;
        var result = process.WithStatus(StatusProcess.Completed);

        result.IsSuccess.Should().BeTrue();
        result.Value.StatusProcess.Should().Be(StatusProcess.Completed);
    }
}
