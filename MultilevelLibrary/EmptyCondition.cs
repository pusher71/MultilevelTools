namespace MultilevelLibrary
{
    enum EmptyCondition
    {
        EMPTY, //просто пусто
        DEADEND, //тупиковая позиция (3 стены рядом)
        CAMERA, //позиция для камер (рядом 1 или 2 стены, и нет запрета)
        ABOVE_BORDER //рядом граница этажа
    }
}
