using System.Drawing;

namespace Tool.ModularHouseBuilder
{
    public enum Module_Type
    {
        WALL = 0,
        FLOOR = 2,
        FLOOR_DECORATOR = 4,
        WALL_DECORATOR = 8,
        DOOR = 16,
        DOOR_FRAME = 32,
        WINDOW = 64,
        WINDOW_FRAME = 128,
        ROOF = 256,
    }

    public static class ModuleTypeUtils
    {
        public static string ToFolderName(this Module_Type module)
        {
            switch(module)
            {
                case Module_Type.WALL:
                    return "Walls";
                case Module_Type.FLOOR:
                    return "Floors";
                case Module_Type.FLOOR_DECORATOR:
                    return "Floor_Decorators";
                case Module_Type.WALL_DECORATOR:
                    return "Wall_Decorators";
                case Module_Type.DOOR:
                    return "Doors";
                case Module_Type.DOOR_FRAME:
                    return "Door_Frames";
                case Module_Type.WINDOW:
                    return "Windows";
                case Module_Type.WINDOW_FRAME:
                    return "Window_Frames";
                case Module_Type.ROOF:
                    return "Roofs";

                default: 
                    return "";
            }
        }

        public static string ToAssetName(this Module_Type module)
        {
            switch (module)
            {
                case Module_Type.WALL:
                    return "Wall";
                case Module_Type.FLOOR:
                    return "Floor";
                case Module_Type.FLOOR_DECORATOR:
                    return "Floor_Decorator";
                case Module_Type.WALL_DECORATOR:
                    return "Wall_Decorator";
                case Module_Type.DOOR:
                    return "Door";
                case Module_Type.DOOR_FRAME:
                    return "Door_Frame";
                case Module_Type.WINDOW:
                    return "Window";
                case Module_Type.WINDOW_FRAME:
                    return "Window_Frame";
                case Module_Type.ROOF:
                    return "Roof";

                default:
                    return "";
            }
        }

        public static Color ToColor(this Module_Type module)
        {
            switch (module)
            {
                case Module_Type.WALL:
                    return Color.White;
                case Module_Type.FLOOR:
                    return Color.White;
                case Module_Type.FLOOR_DECORATOR:
                    return Color.Gray;
                case Module_Type.WALL_DECORATOR:
                    return Color.Blue;
                case Module_Type.DOOR:
                    return Color.Brown;
                case Module_Type.DOOR_FRAME:
                    return Color.Chartreuse;
                case Module_Type.WINDOW:
                    return Color.Magenta;
                case Module_Type.WINDOW_FRAME:
                    return Color.Fuchsia;
                case Module_Type.ROOF:
                    return Color.Maroon;

                default:
                    return Color.Red;
            }
        }
    }
}
