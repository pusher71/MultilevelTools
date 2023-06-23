namespace MultilevelLibrary
{
    public static class Constants
    {
        public const int MAX_FLOOR_EDGE = 14; //максимальная сторона этажа
        public const int MAX_FLOOR_SQUARE = 8 * 14; //максимальная площадь этажа
        public const int MAX_MAZE_VOLUME = MAX_FLOOR_SQUARE * 100; //максимальный объём особняка
        public const int STYLES_COUNT = 14; //количество стилей
        public static readonly int[] STYLES_ORDER_INTERNAL = new int[STYLES_COUNT] { 8, 9, 2, 11, 7, 5, 6, 3, 12, 0, 4, 1, 10, 13 }; //общий порядок стилей
        public const int DIGGER_ENERGY = 5; //энергия копателя
        public const int ROOF_STYLE = 3; //номер стиля для выходной будки
        public const int STYLE_WITH_WINDOWS_THRESHOLD = 2; //номер стиля, начиная с которого расставляются окна
        public const int PLAYER_FLOOR = 0; //изначальный этаж игрока
        public const int ENEMY_FLOOR_NORMAL_THRESHOLD = 2; //минимальный этаж врага при нормальных ограничениях
        public const int WINDOW_CHANCE = 3; //шанс появления окна
        public const int HOLE_CHANCE = 5; //шанс появления дырки в полу
        public const int BRIDGE_CAMERAS_THRESHOLD = 3; //допуск размещения камер на мостах
        public const int SAFETY_INTENSITY = 8; //интенсивность безопасных мест
        public const int PLAYER_INTENSITY = 8; //интенсивность игрока
        public const int CAMERA_INTENSITY = 3; //интенсивность камеры
        public const int KEY_INTENSITY = 6; //интенсивность ключа
    }
}
