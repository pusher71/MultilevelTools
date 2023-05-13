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
        public bool ShiftBottleTexture { get; set; } //смещать текстуру энергетика

        //настраиваемые параметры
        public bool DrawSafetyRoomsType { get; set; } //отображать тип безопасных комнат
        public bool DrawKeyLayer { get; set; } //рисовать ключевые объекты
        public bool DrawDecorations { get; set; } //рисовать украшения

        public abstract bool IsSolidColor(int item); //элемент является сплошным цветом
        public abstract bool IsTexture(int item, bool aboveStairs); //элемент является текстурой
        public abstract bool IsArrow(int item); //элемент является стрелочным ведущим углом
        public abstract bool IsArrowCorner(int item); //элемент является угловым стрелочным ведущим углом
    }
}
