using System.Runtime.InteropServices;
using Ambev.DeveloperEvaluation.ORM;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace Ambev.DevelopersEvaluation.Tests.Shared;

public class DatabaseFixture : IAsyncLifetime
{
    private const string Image = "postgres:13";
    private readonly string _containerName;
    private readonly int _hostPort;
    private const string Username = "test_user";
    private const string Password = "test_password";
    private const string Database = "e2e_tests";
    
    private readonly DockerClient _dockerClient;
    public string ConnectionString { get; private set; }

    public DatabaseFixture()
    {
        _containerName = $"e2e_tests_db_{Guid.NewGuid():N}";
        _hostPort = GetRandomPort();
        
        _dockerClient = new DockerClientConfiguration(
                new Uri(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "npipe://./pipe/docker_engine"
                    : "unix:///var/run/docker.sock"))
            .CreateClient();
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            await RemoveExistingContainerAsync(_containerName);

            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = Image },
                null,
                new Progress<JSONMessage>());

            await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = Image,
                Name = _containerName,
                Env = new List<string>
                {
                    $"POSTGRES_USER={Username}",
                    $"POSTGRES_PASSWORD={Password}",
                    $"POSTGRES_DB={Database}"
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "5432/tcp", new List<PortBinding> { new PortBinding { HostPort = _hostPort.ToString() } } }
                    }
                }
            });

            ConnectionString = $"Host=localhost;Port={_hostPort};Database={Database};Username={Username};Password={Password}";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        await _dockerClient.Containers.StartContainerAsync(_containerName, new ContainerStartParameters());

        await WaitForDatabaseAsync();
    }
    
    private async Task WaitForDatabaseAsync()
    {
        var retry = 10;
        while (retry-- > 0)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();
                return;
            }
            catch
            {
                await Task.Delay(1_000); 
            }
        }

        throw new Exception("Database did not become ready in time.");
    }
    
    private async Task RemoveExistingContainerAsync(string containerName)
    {
        try
        {
            var containers =
                await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            var existingContainer = containers.FirstOrDefault(c => c.Names.Contains($"/{containerName}"));

            if (existingContainer != null)
            {
                Console.WriteLine($"Removing existing container: {containerName}");
                await _dockerClient.Containers.StopContainerAsync(existingContainer.ID, new ContainerStopParameters());
                await _dockerClient.Containers.RemoveContainerAsync(existingContainer.ID,
                    new ContainerRemoveParameters { Force = true });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while removing existing container: {ex.Message}");
        }
    }
    
    public async Task<DefaultContext> CreateE2EDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        var dbContext = new DefaultContext(options);
        
        await dbContext.Database.MigrateAsync();

        return dbContext;
    }
    
    private static int GetRandomPort() => new Random().Next(20000, 30000);
    
    public async Task DisposeAsync()
    {
        try
        {
            Console.WriteLine("Stopping and removing container...");
            await _dockerClient.Containers.StopContainerAsync(_containerName, new ContainerStopParameters());
            await _dockerClient.Containers.RemoveContainerAsync(_containerName,
                new ContainerRemoveParameters { Force = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while disposing container: {ex.Message}");
        }
        finally
        {
            _dockerClient.Dispose();
        }
    }
}