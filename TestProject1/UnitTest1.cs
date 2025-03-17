namespace TestProject1;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
    }
    
    [Theory]
    [InlineData(1)]
    public void Test2(int x)
    {
    }
}