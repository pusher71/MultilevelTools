namespace MultilevelLibrary.Drawing
{
    public class MazeDrawerConfig50 : MazeDrawerConfig
    {
        public MazeDrawerConfig50() : base()
        {
            Cell = 50;
            Weight = 10;
            FontSize = 31;
            DistanceX = 30;
            DistanceY = 30;
            UpperDistance = 50;
            DrawRoofContour = true;
            DrawOverlaps = false;
            ShiftLocksKeysColor = true;
        }

        public override bool IsSolidColor(int item) => item < Utils.IndexStairs && item != Utils.IndexStairsP;
        public override bool IsTexture(int item, bool aboveStairs) => (!IsSolidColor(item) && item != Utils.IndexStairsP) || aboveStairs;
        public override bool IsArrow(int item) => false;
    }
}
