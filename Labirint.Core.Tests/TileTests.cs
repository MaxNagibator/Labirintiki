namespace Labirint.Core.Tests;

[TestFixture]
public class TileTests
{
    // TODO очень страшно, но работает. Можно было придумать что-нибудь получше или не проверять все варианты, но это перестраховка из-за бинарных операция
    [TestCase(Direction.None, Direction.Left, true)]
    [TestCase(Direction.None, Direction.Top, true)]
    [TestCase(Direction.None, Direction.Right, true)]
    [TestCase(Direction.None, Direction.Bottom, true)]
   
    [TestCase(Direction.Left, Direction.Top, true)]
    [TestCase(Direction.Left, Direction.Right, true)]
    [TestCase(Direction.Left, Direction.Bottom, true)]
    [TestCase(Direction.Left, Direction.Left, false)]
 
    [TestCase(Direction.Top, Direction.Left, true)]
    [TestCase(Direction.Top, Direction.Right, true)]
    [TestCase(Direction.Top, Direction.Bottom, true)]
    [TestCase(Direction.Top, Direction.Top, false)]
   
    [TestCase(Direction.Right, Direction.Left, true)]
    [TestCase(Direction.Right, Direction.Top, true)]
    [TestCase(Direction.Right, Direction.Bottom, true)]
    [TestCase(Direction.Right, Direction.Right, false)]
  
    [TestCase(Direction.Bottom, Direction.Left, true)]
    [TestCase(Direction.Bottom, Direction.Top, true)]
    [TestCase(Direction.Bottom, Direction.Right, true)]
    [TestCase(Direction.Bottom, Direction.Bottom, false)]
   
    [TestCase(Direction.Left | Direction.Top, Direction.Right, true)]
    [TestCase(Direction.Left | Direction.Top, Direction.Bottom, true)]
    [TestCase(Direction.Left | Direction.Top, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Top, Direction.Top, false)]
  
    [TestCase(Direction.Left | Direction.Right, Direction.Top, true)]
    [TestCase(Direction.Left | Direction.Right, Direction.Bottom, true)]
    [TestCase(Direction.Left | Direction.Right, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Right, Direction.Right, false)]
    
    [TestCase(Direction.Left | Direction.Bottom, Direction.Top, true)]
    [TestCase(Direction.Left | Direction.Bottom, Direction.Right, true)]
    [TestCase(Direction.Left | Direction.Bottom, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Bottom, Direction.Bottom, false)]
    
    [TestCase(Direction.Top | Direction.Right, Direction.Left, true)]
    [TestCase(Direction.Top | Direction.Right, Direction.Bottom, true)]
    [TestCase(Direction.Top | Direction.Right, Direction.Top, false)]
    [TestCase(Direction.Top | Direction.Right, Direction.Right, false)]
    
    [TestCase(Direction.Top | Direction.Bottom, Direction.Left, true)]
    [TestCase(Direction.Top | Direction.Bottom, Direction.Right, true)]
    [TestCase(Direction.Top | Direction.Bottom, Direction.Top, false)]
    [TestCase(Direction.Top | Direction.Bottom, Direction.Bottom, false)]
    
    [TestCase(Direction.Right | Direction.Bottom, Direction.Left, true)]
    [TestCase(Direction.Right | Direction.Bottom, Direction.Top, true)]
    [TestCase(Direction.Right | Direction.Bottom, Direction.Right, false)]
    [TestCase(Direction.Right | Direction.Bottom, Direction.Bottom, false)]
    
    [TestCase(Direction.Left | Direction.Top | Direction.Right, Direction.Bottom, false)]
    [TestCase(Direction.Left | Direction.Top | Direction.Bottom, Direction.Right, false)]
    [TestCase(Direction.Left | Direction.Right | Direction.Bottom, Direction.Top, false)]
    [TestCase(Direction.Top | Direction.Right | Direction.Bottom, Direction.Left, false)]
    
    [TestCase(Direction.Left | Direction.Top | Direction.Right | Direction.Bottom, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Top | Direction.Right | Direction.Bottom, Direction.Top, false)]
    [TestCase(Direction.Left | Direction.Top | Direction.Right | Direction.Bottom, Direction.Right, false)]
    [TestCase(Direction.Left | Direction.Top | Direction.Right | Direction.Bottom, Direction.Bottom, false)]
    public void CanAddWallTest(Direction existingWalls, Direction directionToAdd, bool expectedResult)
    {
        Tile tile = new()
        {
            Walls = existingWalls
        };

        bool result = tile.CanAddWall(directionToAdd);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
