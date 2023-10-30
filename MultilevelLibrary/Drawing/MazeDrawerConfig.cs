namespace MultilevelLibrary.Drawing
{
    public abstract class MazeDrawerConfig
    {
        public int Step => Cell + Weight; //размер блока

        //автоопределяемые параметры
        public int Cell { get; set; } //размер ячейки
        public int Weight { get; set; } //толщина линий стен
        public int FontSize { get; set; } //размер шрифта
        public int DistanceX { get; set; } //расстояние между картинками по X
        public int DistanceY { get; set; } //расстояние между картинками по Y
        public int UpperDistance { get; set; } //расстояние между картинкой и верхом сетки (для текста)
        public bool DrawRoofContour { get; set; } //рисовать контур этажа-крыши
        public bool DrawOverlaps { get; set; } //рисовать межэтажные перекрытия
        public bool ShiftLocksKeysColor { get; set; } //смещать цветные замки и ключи

        //настраиваемые параметры
        public bool DrawKeyLayer { get; set; } //рисовать ключевые объекты
        public bool DrawDecorations { get; set; } //рисовать украшения

        public abstract bool IsSolidColor(int item); //элемент является сплошным цветом
        public abstract bool IsTexture(int item, bool aboveStairs); //элемент является текстурой
        public abstract bool IsArrow(int item); //элемент является стрелочным ведущим углом
    }
}
