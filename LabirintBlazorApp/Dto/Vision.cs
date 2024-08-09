using MudBlazor;

namespace LabirintBlazorApp.Dto
{
    public class Vision(int mazeWidth, int mazeHeight)
    {
        private int _mazeWidth = mazeWidth;
        private int _mazeHeight = mazeHeight;

        public int MyPositionX { get; private set; }
        public int MyPositionY { get; private set; }
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int FinishX { get; private set; }
        public int FinishY { get; private set; }

        public void SetPosition(int x, int y)
        {
            var visionRangeBase = 3;
            var visionRange = (visionRangeBase) * 2;

            MyPositionX = x;
            MyPositionY = y;
            StartX = Math.Max(1, MyPositionX - visionRange);
            FinishX = Math.Min(_mazeHeight - 1, MyPositionX + visionRange + 2);
            StartY = Math.Max(1, MyPositionY - visionRange);
            FinishY = Math.Min(_mazeWidth - 1, MyPositionY + visionRange + 2);
        }

        public int GetDrawX(int x)
        {
            return x - StartX + 1;
        }

        public int GetDrawY(int y)
        {
            return y - StartY + 1;
        }
    }
}
