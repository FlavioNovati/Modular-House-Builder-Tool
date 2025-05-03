using UnityEditor;
using UnityEngine;

namespace Tool.ModularHouseBuilder
{
    [System.Serializable]
    public enum ModuleType
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
        public static ModuleType ModuleTypeFromInt(int index)
        {
            int value = 0;
            if(index != 0)
                value = (int)Mathf.Pow(2, index);

            return (ModuleType)value;
        }

        public static string ToFolderName(this ModuleType module)
        {
            switch(module)
            {
                case ModuleType.WALL:
                    return "Walls";
                case ModuleType.FLOOR:
                    return "Floors";
                case ModuleType.FLOOR_DECORATOR:
                    return "Floor_Decorators";
                case ModuleType.WALL_DECORATOR:
                    return "Wall_Decorators";
                case ModuleType.DOOR:
                    return "Doors";
                case ModuleType.DOOR_FRAME:
                    return "Door_Frames";
                case ModuleType.WINDOW:
                    return "Windows";
                case ModuleType.WINDOW_FRAME:
                    return "Window_Frames";
                case ModuleType.ROOF:
                    return "Roofs";

                default:
                    return "<ERROR WHILE GETTING FOLDER NAME>";
            }
        }

        public static string ToAssetName(this ModuleType module)
        {
            switch (module)
            {
                case ModuleType.WALL:
                    return "Wall";
                case ModuleType.FLOOR:
                    return "Floor";
                case ModuleType.FLOOR_DECORATOR:
                    return "Floor_Decorator";
                case ModuleType.WALL_DECORATOR:
                    return "Wall_Decorator";
                case ModuleType.DOOR:
                    return "Door";
                case ModuleType.DOOR_FRAME:
                    return "Door_Frame";
                case ModuleType.WINDOW:
                    return "Window";
                case ModuleType.WINDOW_FRAME:
                    return "Window_Frame";
                case ModuleType.ROOF:
                    return "Roof";

                default:
                    return "<ERROR WHILE GETTING ASSET NAME>";
            }
        }

        public static string ToFormattedName(this ModuleType module)
        {
            switch (module)
            {
                case ModuleType.WALL:
                    return "Wall\t\t\t\t";
                case ModuleType.FLOOR:
                    return "Floor\t\t\t";
                case ModuleType.FLOOR_DECORATOR:
                    return "Floor_Decorator\t\t";
                case ModuleType.WALL_DECORATOR:
                    return "Wall_Decorator\t\t";
                case ModuleType.DOOR:
                    return "Door\t\t\t";
                case ModuleType.DOOR_FRAME:
                    return "Door_Frame\t\t";
                case ModuleType.WINDOW:
                    return "Window\t\t\t";
                case ModuleType.WINDOW_FRAME:
                    return "Window_Frame\t\t";
                case ModuleType.ROOF:
                    return "Roof\t\t\t";

                default:
                    return "<ERROR WHILE GETTING FORMATTED ASSET NAME>";
            }
        }

        public static string ToDescription(this ModuleType module)
        {
            switch (module)
            {
                case ModuleType.WALL:
                    return "Wall";
                case ModuleType.FLOOR:
                    return "Floor";
                case ModuleType.FLOOR_DECORATOR:
                    return "Floor object that will snap on a floor surface";
                case ModuleType.WALL_DECORATOR:
                    return "Wall object that will snap on a wall surface";
                case ModuleType.DOOR:
                    return "Door object that will snap on a door frame object";
                case ModuleType.DOOR_FRAME:
                    return "Door frame";
                case ModuleType.WINDOW:
                    return "Window object that will snap on a window frame";
                case ModuleType.WINDOW_FRAME:
                    return "Window frame";
                case ModuleType.ROOF:
                    return "Roof";

                default:
                    return "<ERROR WHILE GETTING DESCRIPTION>";
            }
        }
        
        public static Color ToColor(this ModuleType module)
        {
            switch (module)
            {
                case ModuleType.WALL:
                    return new Color(1, 1, 0.56f, 1);
                case ModuleType.FLOOR:
                    return new Color(1, 0.56f, 0.56f, 1);
                case ModuleType.FLOOR_DECORATOR:
                    return new Color(0.58f, 1, 0.56f, 1);
                case ModuleType.WALL_DECORATOR:
                    return new Color(0.56f, 0.98f, 1, 1);
                case ModuleType.DOOR:
                    return new Color(0.62f, 0.56f, 1, 1);
                case ModuleType.DOOR_FRAME:
                    return new Color(0.32f, 0.31f, 0.57f, 1);
                case ModuleType.WINDOW:
                    return new Color(1, 0.56f, 0.89f, 1);
                case ModuleType.WINDOW_FRAME:
                    return new Color(0.68f, 0.31f, 0.56f, 1);
                case ModuleType.ROOF:
                    return new Color(0.74f, 1, 0.85f, 1);

                default:
                    return Color.red;
            }
        }

        public static Texture ToTexture(this ModuleType module)
        {
            string path = "Assets/Scripts/HouseBuilder_Tool/HouseBuilder_Art/Modules_Art/";

            Texture moduleTexture;
            moduleTexture = (Texture)AssetDatabase.LoadAssetAtPath($"{path}/ModuleArt_{module.ToAssetName()}.png", typeof(Texture));

            return moduleTexture;
        }
    }
}
