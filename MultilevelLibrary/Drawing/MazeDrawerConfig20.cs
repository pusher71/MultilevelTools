namespace MultilevelLibrary.Drawing
{
    public class MazeDrawerConfig20 : MazeDrawerConfig
    {
        public MazeDrawerConfig20() : base()
        {
            Cell = 20;
            Weight = 4;
            FontSize = 18;
            DistanceX = 48;
            DistanceY = 24;
            UpperDistance = 24;
            DrawRoofContour = false;
            DrawOverlaps = true;
            ShiftBottleTexture = false;
        }

        public override bool IsSolidColor(int item) => item < Utils.IndexCamera;
        public override bool IsTexture(int item, bool aboveStairs) => Utils.IsCamera(item) || Utils.IsWindow(item);
        public override bool IsArrow(int item) => item >= Utils.IndexStairs && item < Utils.IndexRoof;
        public override bool IsArrowCorner(int item) => Utils.IsRoof(item);
    }
}
